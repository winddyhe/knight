using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Knight.Core;
using Knight.Framework.Net;
using UnityEngine;
using Knight.Framework;

namespace Knight.Hotfix.Core
{
    public class HotfixNetworkClient : THotfixSingleton<HotfixNetworkClient>
    {
        private NetworkSession                              mSession;
        private readonly Dict<int, Action<IHotfixResponse>> mRequestCallback = new Dict<int, Action<IHotfixResponse>>();
        private HotfixNetworkMessagePacker                  mMessagePacker;
        private readonly List<byte[]>                       mByteses         = new List<byte[]>() { new byte[1], new byte[0], new byte[0] };

        private HotfixNetworkClient()
        {
            this.mMessagePacker = new HotfixNetworkMessagePacker();
        }

        public NetworkSession Session
        {
            get { return mSession;  }
            set { mSession = value; }
        }
        
        public void Run(NetworkSession rSession, Packet rPacket)
        {
            if (this.mSession != rSession)
            {
                Debug.LogError("Network Session is not the same one.");
                return;
            }

            ushort nOpcode = rPacket.Opcode;
            byte nFlag = rPacket.Flag;
            
            NetworkOpcodeTypes rOpcodeTypeComponent = this.mSession.Parent.OpcodeTypes;
            Type rResponseType = rOpcodeTypeComponent.GetType(nOpcode);
            object rMessage = this.mMessagePacker.DeserializeFrom(rResponseType, rPacket.Bytes, Packet.Index, rPacket.Length - Packet.Index);
            Debug.LogError($"recv: {HotfixJsonParser.ToJsonNode(rMessage)}");

            // 非RPC消息走这里
            if ((nFlag & 0x01) == 0)
            {
                EventManager.Instance.Distribute(nOpcode, rMessage);
                return;
            }

            // Rpc回调消息
            IHotfixResponse rResponse = rMessage as IHotfixResponse;
            if (rResponse == null)
            {
                throw new Exception($"flag is response, but message is not! {nOpcode}");
            }
            Action<IHotfixResponse> rAction;
            if (!this.mRequestCallback.TryGetValue(rResponse.RpcId, out rAction))
            {
                return;
            }
            this.mRequestCallback.Remove(rResponse.RpcId);
            rAction(rResponse);
        }

        public Task<IHotfixResponse> Call(IHotfixRequest rRequest)
        {
            int nRpcId = ++NetworkSession.RpcId;
            var rTCS = new TaskCompletionSource<IHotfixResponse>();

            this.mRequestCallback[nRpcId] = (response) =>
            {
                try
                {
                    if (response.Error > NetworkErrorCode.ERR_Exception)
                    {
                        throw new RpcException(response.Error, response.Message);
                    }

                    rTCS.SetResult(response);
                }
                catch (Exception e)
                {
                    rTCS.SetException(new Exception($"Rpc Error: {rRequest.GetType().FullName}", e));
                }
            };

            rRequest.RpcId = nRpcId;
            this.Send(0x00, rRequest);
            return rTCS.Task;
        }

        public Task<IHotfixResponse> Call(IHotfixRequest rRequest, CancellationToken rCancellationToken)
        {
            int nRpcId = ++NetworkSession.RpcId;
            var rTCS = new TaskCompletionSource<IHotfixResponse>();

            this.mRequestCallback[nRpcId] = (rResponse) =>
            {
                try
                {
                    if (rResponse.Error > NetworkErrorCode.ERR_Exception)
                    {
                        throw new RpcException(rResponse.Error, rResponse.Message);
                    }
                    rTCS.SetResult(rResponse);
                }
                catch (Exception e)
                {
                    rTCS.SetException(new Exception($"Rpc Error: {rRequest.GetType().FullName}", e));
                }
            };

            rCancellationToken.Register(() => this.mRequestCallback.Remove(nRpcId));

            rRequest.RpcId = nRpcId;
            this.Send(0x00, rRequest);
            return rTCS.Task;
        }

        public void Send(byte nFlag, IHotfixMessage rMessage)
        {
            NetworkOpcodeTypes rOpcodeTypes = this.mSession.Parent.OpcodeTypes;
            ushort nOpcode = rOpcodeTypes.GetOpcode(rMessage.GetType());
            byte[] rBytes = this.mMessagePacker.SerializeToByteArray(rMessage);

            Send(nFlag, nOpcode, rBytes);
        }

        public void Send(byte nFlag, ushort nOpcode, byte[] rBytes)
        {
            this.mSession.Send(nFlag, nOpcode, rBytes);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight.Framework.Net
{
    public sealed class NetworkSession : IDisposable
    {
        private static long                                 mIdGenerator     = 1000;

        private AChannel                                    mChannel;
        private readonly Dictionary<int, Action<IResponse>> mRequestCallback = new Dictionary<int, Action<IResponse>>();
        private readonly List<byte[]>                       mByteses         = new List<byte[]>() { new byte[1], new byte[0], new byte[0] };
        private NetworkClient                               mParent;
        
        public long                                         Id;
        public int                                          Error;
        public bool                                         IsDisposed;
        public static int                                   RpcId           { get; set; }
        
        public NetworkSession(NetworkClient rNet, AChannel rChannel)
        {
            this.Id = mIdGenerator++;
            this.Error = 0;
            this.mChannel = rChannel;
            this.mRequestCallback.Clear();
            this.StartRecv();
        }

        public NetworkClient Parent
        {
            get { return this.mParent;  }
            set { this.mParent = value; }
        }
        
        public void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            long nID = this.Id;
            
            foreach (Action<IResponse> rAction in this.mRequestCallback.Values.ToArray())
            {
                rAction.Invoke(new ResponseMessage { Error = this.Error });
            }

            this.Error = 0;
            this.mChannel.Dispose();
            this.mParent.Remove(nID);
            this.mRequestCallback.Clear();
        }

        public IPEndPoint RemoteAddress
        {
            get
            {
                return this.mChannel.RemoteAddress;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return this.mChannel.ChannelType;
            }
        }

        private async void StartRecv()
        {
            while (true)
            {
                if (this.IsDisposed)
                {
                    return;
                }

                Packet rPacket;
                try
                {
                    rPacket = await this.mChannel.Recv();

                    if (this.IsDisposed)
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    continue;
                }

                try
                {
                    this.Run(rPacket);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        private void Run(Packet rPacket)
        {
            if (rPacket.Length < Packet.MinSize)
            {
                Debug.LogError($"message error length < {Packet.MinSize}, ip: {this.RemoteAddress}");
                this.mParent.Remove(this.Id);
                return;
            }

            byte nFlag = rPacket.Flag;
            ushort nOpcode = rPacket.Opcode;

            if (NetworkOpcodeTypes.IsClientHotfixMessage(nOpcode))
            {
                this.mParent.MessageDispatcher.Dispatch(this, rPacket);
                return;
            }

            // flag第一位为1表示这是rpc返回消息,否则交由MessageDispatcher分发
            if ((nFlag & 0x01) == 0)
            {
                this.mParent.MessageDispatcher.Dispatch(this, rPacket);
                return;
            }

            NetworkOpcodeTypes rOpcodeTypeComponent = this.mParent.OpcodeTypes;
            Type rResponseType = rOpcodeTypeComponent.GetType(nOpcode);
            object rMessage = this.mParent.MessagePacker.DeserializeFrom(rResponseType, rPacket.Bytes, Packet.Index, rPacket.Length - Packet.Index);
            //Log.Debug($"recv: {JsonHelper.ToJson(message)}");

            IResponse rResponse = rMessage as IResponse;
            if (rResponse == null)
            {
                throw new Exception($"flag is response, but message is not! {nOpcode}");
            }
            Action<IResponse> rAction;
            if (!this.mRequestCallback.TryGetValue(rResponse.RpcId, out rAction))
            {
                return;
            }
            this.mRequestCallback.Remove(rResponse.RpcId);

            rAction(rResponse);
        }

        public Task<IResponse> Call(IRequest rRequest)
        {
            int nRpcId = ++RpcId;
            var rTCS = new TaskCompletionSource<IResponse>();

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

        public Task<IResponse> Call(IRequest rRequest, CancellationToken rCancellationToken)
        {
            int nRpcId = ++RpcId;
            var rTCS = new TaskCompletionSource<IResponse>();

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

        public void Send(IMessage rMessage)
        {
            this.Send(0x00, rMessage);
        }

        public void Reply(IResponse rMessage)
        {
            if (this.IsDisposed)
            {
                throw new Exception("session已经被Dispose了");
            }

            this.Send(0x01, rMessage);
        }

        public void Send(byte nFlag, IMessage rMessage)
        {
            NetworkOpcodeTypes rOpcodeTypes = this.mParent.OpcodeTypes;
            ushort nOpcode = rOpcodeTypes.GetOpcode(rMessage.GetType());
            byte[] rBytes = this.mParent.MessagePacker.SerializeToByteArray(rMessage);

            Send(nFlag, nOpcode, rBytes);
        }

        public void Send(byte nFlag, ushort nOpcode, byte[] rBytes)
        {
            if (this.IsDisposed)
            {
                throw new Exception("session已经被Dispose了");
            }
            this.mByteses[0][0] = nFlag;
            this.mByteses[1] = BitConverter.GetBytes(nOpcode);
            this.mByteses[2] = rBytes;

            mChannel.Send(this.mByteses);
        }
    }
}

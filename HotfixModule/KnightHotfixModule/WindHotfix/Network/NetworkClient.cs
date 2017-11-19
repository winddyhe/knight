using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindHotfix.Core;
using Core;
using System.Threading;
using UnityEngine;
using Framework.Network;

namespace WindHotfix.Net
{
    public class NetworkClient : THotfixSingleton<NetworkClient>
    {
        private static uint                 RpcId;
        
        private SerializerPacker            mMessagePacker;   
        private Dict<uint, Action<object>>  mRequestCallback; 
        private List<byte[]>                mByteses;                                             

        public  SerializerPacker            MessagePacker       => mMessagePacker;
        public  Dict<uint, Action<object>>  RequestCallback     => mRequestCallback;

        private NetworkClient()
        {
        }

        public static void Initialize()
        {
            Instance.mMessagePacker = new SerializerPacker();
            Instance.mRequestCallback = new Dict<uint, Action<object>>();
            Instance.mByteses = new List<byte[]>() { new byte[0], new byte[0] };
        }

        public Task<TResponse> Call<TResponse>(Session rSession, ARequest rRequest, CancellationToken rCancellationToken) where TResponse : AResponse
        {
            rRequest.RpcId = ++RpcId;

            this.SendMessage(rSession, rRequest);

            var tcs = new TaskCompletionSource<TResponse>();
            this.mRequestCallback[RpcId] = (message) =>
            {
                try
                {
                    TResponse response = (TResponse)message;
                    if (response.Error > 100)
                    {
                        tcs.SetException(new RpcException(response.Error, response.Message));
                        return;
                    }
                    //Debug.Log($"recv: {response.ToString()}");
                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(new Exception($"Rpc Error: {typeof(TResponse).FullName}", e));
                }
            };
            rCancellationToken.Register(() => { this.mRequestCallback.Remove(RpcId); });
            return tcs.Task;
        }

        public Task<Response> Call<Response>(Session rSession, ARequest rRequest) where Response : AResponse
        {
            rRequest.RpcId = ++RpcId;
            this.SendMessage(rSession, rRequest);

            var tcs = new TaskCompletionSource<Response>();
            this.mRequestCallback[RpcId] = (message) =>
            {
                try
                {
                    Response response = (Response)message;
                    if (response.Error > 100)
                    {
                        tcs.SetException(new RpcException(response.Error, response.Message));
                        return;
                    }
                    //Debug.Log($"recv: {response.ToString()}");
                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(new Exception($"Rpc Error: {typeof(Response).FullName}", e));
                }
            };
            return tcs.Task;
        }

        public void Send(Session rSession, AMessage rMessage)
        {
            if (rSession.Id == 0)
            {
                throw new Exception("session已经被Dispose了");
            }
            this.SendMessage(rSession, rMessage);
        }

        public void Reply<Response>(Session rSession, Response rMessage) where Response : AResponse
        {
            if (rSession.Id == 0)
            {
                throw new Exception("session已经被Dispose了");
            }
            this.SendMessage(rSession, rMessage);
        }

        private void SendMessage(Session rSession, AMessage rMessage)
        {
            ushort nOpcode = OpcodeTypes.Instance.GetOpcode(rMessage.GetType());
            Debug.Log($"send: {nOpcode}, {rMessage.ToString()}");

            byte[] rMessageBytes = this.mMessagePacker.SerializeToByteArray(rMessage);
            rSession.SendMessage(nOpcode, rMessageBytes);
        }
    }
}

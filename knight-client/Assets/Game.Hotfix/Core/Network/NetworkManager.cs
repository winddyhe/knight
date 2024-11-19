using System;
using System.Collections.Generic;
using Knight.Core;
using System.Threading.Tasks;
using Knight.Framework.Network;
using Google.Protobuf;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class NetworkManager : TSingleton<NetworkManager>, INetworkMessageHandler
    {
        private Dictionary<uint, List<NetworkMsgProto>> mNetworkMsgHandlers;
        private ConcurrentQueue<NetworkMsgResponse> mClientMsgQueue;

        private ushort mAwaitProtoMsgIndex;

        private NetworkManager()
        {
        }

        public async Task Initialize()
        {
            this.mNetworkMsgHandlers = new Dictionary<uint, List<NetworkMsgProto>>();
            this.mClientMsgQueue = new ConcurrentQueue<NetworkMsgResponse>();
            this.mAwaitProtoMsgIndex = 0;

            NetworkTcpManager.Instance.Initialize(this);
            await NetworkTcpManager.Instance.ConnectTcpAsync(NetworkType.MainTcp, "127.0.0.1:8888");
        }

        public void Update(float fDeltaTime)
        {
            while (this.mClientMsgQueue.Count > 0)
            {
                if (this.mClientMsgQueue.TryDequeue(out var rNetworkMsgResponse))
                {
                    rNetworkMsgResponse.MsgHandler(rNetworkMsgResponse.ResponseMsg);
                }
            }
        }

        void INetworkMessageHandler.OnTcpReceived(uint nCMD, uint nACT, int nIndex, in ReadOnlySpan<byte> rBodyBytes)
        {
            var nMsgCode = (uint)(nCMD << 8 | nACT);

            if (this.mNetworkMsgHandlers.TryGetValue(nMsgCode, out var rServerMsgHandlerList))
            {
                foreach (var rServerMsgHandler in rServerMsgHandlerList)
                {
                    rServerMsgHandler.Index = nIndex;
                    // @TODO: 这里可以通过代码生成来进行创建消息对象
                    var rMessage = Activator.CreateInstance(rServerMsgHandler.ResponseMsgType) as IMessage;
                    rMessage.MergeFrom(rBodyBytes);
                    var rNetworkMsgResponse = new NetworkMsgResponse()
                    {
                        ResponseMsg = rMessage,
                        MsgHandler = rServerMsgHandler.MsgHandler
                    };
                    this.mClientMsgQueue.Enqueue(rNetworkMsgResponse);
                }
            }
        }

        public UniTask<TResponse> SendAsync<TResponse>(NetworkType rNetworkType, uint nCMD, uint nACT, IMessage rRequestMsg, int nTimeout) where TResponse : IMessage, new()
        {
            var nIndex = this.GetNextAwaitProtoMsgIndex();
            var rResponseTCS = new UniTaskCompletionSource<TResponse>();
            var rNetworkMsgProto = this.RegisterMessageHandler(nCMD, nACT, nIndex, typeof(TResponse), (rResponseMsg) =>
            {
                rResponseTCS.TrySetResult((TResponse)rResponseMsg);
            });
            NetworkTcpManager.Instance.SendAsync(rNetworkType, nCMD, nACT, nIndex, rRequestMsg).WrapErrors();
            return rResponseTCS.Task;
        }

        public void Send(NetworkType rNetworkType, uint nCMD, uint nACT, IMessage rRequestMsg)
        {
            NetworkTcpManager.Instance.SendAsync(rNetworkType, nCMD, nACT, 0, rRequestMsg).WrapErrors();
        }

        public NetworkMsgProto RegisterMessageHandler<TResponse>(uint nCMD, uint nACT, NetworkMsgHandleDelegate rMsgHandler) where TResponse : IMessage, new()
        {
            return this.RegisterMessageHandler(nCMD, nACT, 0, typeof(TResponse), rMsgHandler);
        }

        public NetworkMsgProto RegisterMessageHandler(uint nCMD, uint nACT, Type rResponseType, NetworkMsgHandleDelegate rMsgHandler)
        {
            return this.RegisterMessageHandler(nCMD, nACT, 0, rResponseType, rMsgHandler);
        }

        public void UnregisterMessageHandler(NetworkMsgProto rNetworkMsgProto)
        {
            var nMsgCode = (uint)(rNetworkMsgProto.CMD << 8 | rNetworkMsgProto.ACT);
            if (this.mNetworkMsgHandlers.TryGetValue(nMsgCode, out var rServerMsgHandlerList))
            {
                rServerMsgHandlerList.Remove(rNetworkMsgProto);
            }
        }

        private NetworkMsgProto RegisterMessageHandler(uint nCMD, uint nACT, ushort nIndex, Type rResponseType, NetworkMsgHandleDelegate rMsgHandler)
        {
            var nMsgCode = (uint)(nCMD << 8 | nACT);
            if (!this.mNetworkMsgHandlers.TryGetValue(nMsgCode, out var rServerMsgHandlerList))
            {
                rServerMsgHandlerList = new List<NetworkMsgProto>();
                this.mNetworkMsgHandlers.Add(nMsgCode, rServerMsgHandlerList);
            }
            var rNetworkMsgProto = new NetworkMsgProto()
            {
                CMD = nCMD,
                ACT = nACT,
                Index = nIndex,
                MsgHandler = rMsgHandler,
                ResponseMsgType = rResponseType
            };
            rServerMsgHandlerList.Add(rNetworkMsgProto);
            return rNetworkMsgProto;
        }

        private ushort GetNextAwaitProtoMsgIndex()
        {
            this.mAwaitProtoMsgIndex++;
            //如果索引为 0, 则将索引修改为 1, 0 是非等待消息
            if (this.mAwaitProtoMsgIndex == 0)
            {
                this.mAwaitProtoMsgIndex = 1;
            }
            return this.mAwaitProtoMsgIndex;
        }
    }
}

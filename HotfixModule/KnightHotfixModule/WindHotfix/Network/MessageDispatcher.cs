using System;
using Core;
using UnityEngine;
using Model;
using Framework.Network;

namespace WindHotfix.Net
{
    public class MessageDispatcher
    {
        public MessageDispatcher()
        {
        }

        public void Dispatch(ushort nOpcode, byte[] rMessageBytes, int nOffset)
        {
            Type rMessageType = OpcodeTypes.Instance.GetType(nOpcode);

            object message = NetworkClient.Instance.MessagePacker.DeserializeFrom(rMessageType, rMessageBytes, nOffset, rMessageBytes.Length - nOffset);
            Debug.Log($"recv: {nOpcode}, {message.ToString()}");

            AResponse response = message as AResponse;
            if (response != null)
            {
                // rpcFlag>0 表示这是一个rpc响应消息
                // Rpc回调有找不着的可能，因为client可能取消Rpc调用
                Action<object> rAction;
                if (!NetworkClient.Instance.RequestCallback.TryGetValue(response.RpcId, out rAction))
                {
                    return;
                }
                NetworkClient.Instance.RequestCallback.Remove(response.RpcId);
                rAction(message);
                return;
            }

            //@TODO: Dispatch No callback Message.
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Knight.Core;

namespace Knight.Framework.Net
{
    public class NetworkClientDispatcher : IMessageDispatcher
    {
        private readonly Dict<ushort, List<Action<NetworkSession, IResponse>>> mMessageHandlers = new Dict<ushort, List<Action<NetworkSession, IResponse>>>();

        public void Initialize()
        {
            this.mMessageHandlers.Clear();

            // TODO: 还需要优化类型Assembly
            List<Type> rTypes = new List<Type>();
            var rAllAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < rAllAssemblies.Length; i++)
            {
                rTypes.AddRange(rAllAssemblies[i].GetTypes());
            }

            foreach (Type rType in rTypes)
            {
                var rAllMethods = rType.GetMethods();
                for (int i = 0; i < rAllMethods.Length; i++)
                {
                    var rMessageHandlerAttr = rAllMethods[i].GetCustomAttribute<MessageHandlerAttribute>(false);
                    if (rMessageHandlerAttr == null)
                    {
                        continue;
                    }
                }
            }
        }

        public void Dispatch(NetworkSession rSession, Packet rPacket)
        {
            object rMessage;
            try
            {
                if (NetworkOpcodeTypes.IsClientHotfixMessage(rPacket.Opcode))
                {
                    // 处理热更新的消息分发
                    return;
                }
                NetworkOpcodeTypes rOpcodeTypes = rSession.Parent.OpcodeTypes;
                Type rResponseType = rOpcodeTypes.GetType(rPacket.Opcode);
                rMessage = rSession.Parent.MessagePacker.DeserializeFrom(rResponseType, rPacket.Bytes, Packet.Index, rPacket.Length - Packet.Index);
            }
            catch (Exception e)
            {
                // 出现任何解析消息异常都要断开Session，防止客户端伪造消息
                Debug.LogError(e);
                rSession.Error = NetworkErrorCode.ERR_PacketParserError;
                rSession.Parent.Remove(rSession.Id);
                return;
            }

            // 如果是帧同步消息,交给ClientFrameComponent处理
            //FrameMessage frameMessage = rMessage as FrameMessage;
            //if (frameMessage != null)
            //{
            //    Game.Scene.GetComponent<ClientFrameComponent>().Add(session, frameMessage);
            //    return;
            //}

            // 普通消息或者是Rpc请求消息
            //MessageInfo messageInfo = new MessageInfo(packet.Opcode, rMessage);
            //Game.Scene.GetComponent<MessageDispatherComponent>().Handle(session, messageInfo);
        }
    }
}

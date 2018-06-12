using System;
using System.Collections.Generic;
using UnityEngine;
using Knight.Core;
using Knight.Framework.Hotfix;

namespace Knight.Framework.Net
{
    public class NetworkClientDispatcher : IMessageDispatcher
    {
        private HotfixObject mHotfixDispatchObject;

        public NetworkClientDispatcher()
        {
            if (HotfixManager.Instance != null)
            {
                mHotfixDispatchObject = HotfixManager.Instance.Instantiate("Knight.Hotfix.Core.HotfixNetworkClientDispatcher");
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
                    if (mHotfixDispatchObject != null)
                    {
                        mHotfixDispatchObject.Invoke("Dispatch", rSession, rPacket);
                    }
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
            EventManager.Instance.Distribute(rPacket.Opcode, rMessage);
        }
    }
}

using Google.Protobuf;
using System;

namespace Knight.Framework.Network
{
    public delegate void NetworkMsgHandleDelegate(IMessage rResponseMessage);

    public class NetworkMsgProto
    {
        public uint CMD;
        public uint ACT;
        public int Index;
        public Type ResponseMsgType;
        public NetworkMsgHandleDelegate MsgHandler;
    }

    public class NetworkMsgResponse
    {
        public IMessage ResponseMsg;
        public NetworkMsgHandleDelegate MsgHandler;
    }
}

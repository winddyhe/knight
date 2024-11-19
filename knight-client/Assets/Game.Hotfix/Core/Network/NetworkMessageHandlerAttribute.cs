using System;

namespace Game
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NetworkMessageHandlerAttribute : Attribute
    {
        public uint CMD;
        public uint ACT;
        public Type ResponseType;

        public NetworkMessageHandlerAttribute(uint nCmd, uint nAct, Type rResponseType)
        {
            this.CMD = nCmd;
            this.ACT = nAct;
            this.ResponseType = rResponseType;
        }
    }
}

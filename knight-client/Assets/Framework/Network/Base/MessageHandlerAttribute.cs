using System;

namespace Knight.Framework.Net
{
    [AttributeUsage(AttributeTargets.Method)]
	public class MessageHandlerAttribute : Attribute
    {
        public ushort Opcode;

        public MessageHandlerAttribute(ushort nOpcode)
		{
            this.Opcode = nOpcode;
		}
	}
}
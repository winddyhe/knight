using System;

namespace Knight.Framework.Net
{
	public class MessageAttribute: Attribute
	{
		public ushort Opcode { get; }

		public MessageAttribute(ushort rOpcode)
		{
			this.Opcode = rOpcode;
		}
	}
}
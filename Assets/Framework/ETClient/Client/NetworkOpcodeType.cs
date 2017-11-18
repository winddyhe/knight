using System;
using Core;

namespace Model
{
	public class NetworkOpcodeType : TSingleton<NetworkOpcodeType>
	{
		private DoubleMap<ushort, Type> mOpcodeTypes = new DoubleMap<ushort, Type>();

        private NetworkOpcodeType()
        {
        }

		public void Initialize()
		{
			Type[] monoTypes = DllHelper.GetMonoTypes();
			foreach (Type monoType in monoTypes)
			{
				object[] attrs = monoType.GetCustomAttributes(typeof(MessageAttribute), false);
				if (attrs.Length == 0)
				{
					continue;
				}
				MessageAttribute messageAttribute = attrs[0] as MessageAttribute;
				if (messageAttribute == null)
				{
					continue;
				}
				this.mOpcodeTypes.Add(messageAttribute.Opcode, monoType);
			}
		}

		public ushort GetOpcode(Type type)
		{
			return this.mOpcodeTypes.GetKeyByValue(type);
		}

		public Type GetType(ushort opcode)
		{
			return this.mOpcodeTypes.GetValueByKey(opcode);
		}
	}
}
using System;
using System.Collections.Generic;
using Knight.Core;
using Knight.Framework.Hotfix;

namespace Knight.Framework.Net
{
    public class NetworkOpcodeTypes
    {
        private readonly DoubleMap<ushort, Type> mOpcodeTypes = new DoubleMap<ushort, Type>();

        public void Initialize()
        {
            List<Type> rTypes = new List<Type>();
            var rAllAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < rAllAssemblies.Length; i++)
            {
                rTypes.AddRange(rAllAssemblies[i].GetTypes());
            }
            if (HotfixManager.Instance != null)
            {
                rTypes.AddRange(HotfixManager.Instance.GetTypes());
            }

            foreach (Type rType in rTypes)
            {
                object[] rAttrs = rType.GetCustomAttributes(typeof(MessageAttribute), false);
                if (rAttrs.Length == 0)
                {
                    continue;
                }

                MessageAttribute rMessageAttribute = rAttrs[0] as MessageAttribute;
                if (rMessageAttribute == null)
                {
                    continue;
                }

                this.mOpcodeTypes.Add(rMessageAttribute.Opcode, rType); 
            }
        }

        public ushort GetOpcode(Type rType)
        {
            return this.mOpcodeTypes.GetKeyByValue(rType);
        }

        public Type GetType(ushort nOpcode)
        {
            return this.mOpcodeTypes.GetValueByKey(nOpcode);
        }

        public static bool IsClientHotfixMessage(ushort opcode)
        {
            return opcode > 10000;
        }
    }
}

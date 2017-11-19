using Core;
using Framework.Hotfix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Model;

namespace Framework.Network
{
    public class OpcodeTypes : TSingleton<OpcodeTypes>
    {
        public Dict<ushort, System.Type> Types;

        private OpcodeTypes()
        {
        }
        
        public void Initialize()
        {
            this.Types = new Dict<ushort, System.Type>();

            var rMsgTypes = HotfixManager.Instance.GetTypes();
            for (int i = 0; i < rMsgTypes.Length; i++)
            {
                var rAttrObjs = rMsgTypes[i].GetCustomAttributes(typeof(MessageAttribute), false);
                if (rAttrObjs == null || rAttrObjs.Length == 0) continue;

                var rMsgAttr = rAttrObjs[0] as MessageAttribute;
                if (rMsgAttr != null)
                {
                    var nOpcode = rMsgAttr.Opcode;
                    this.Types.Add((ushort)nOpcode, rMsgTypes[i]);
                }
            }
        }

        public System.Type GetType(ushort nOpcode)
        {
            if (this.Types == null) return null;
            Type rType = null;
            this.Types.TryGetValue(nOpcode, out rType);
            return rType;
        }

        public ushort GetOpcode(Type type)
        {
            if (this.Types == null) return 0;
            foreach (var rPair in this.Types)
            {
                if (rPair.Value == type)
                    return rPair.Key;
            }
            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.UI
{
    public class BindableMember<T> where T : MemberInfo
    {
        public readonly object  PropOwner;
        public readonly T       Member;
        public readonly Type    ViewModelType;

        public string  ViewModelTypeName
        {
            get { return this.ViewModelType.Name;   }
        }

        public string MemberName
        {
            get { return this.Member.Name;          }
        }

        public BindableMember(object rPropOwner, T rMember, Type rViewModelType)
        {
            this.PropOwner      = rPropOwner;
            this.Member         = rMember;
            this.ViewModelType  = rViewModelType;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", this.ViewModelType.ToString(), this.MemberName);
        }
    }
}

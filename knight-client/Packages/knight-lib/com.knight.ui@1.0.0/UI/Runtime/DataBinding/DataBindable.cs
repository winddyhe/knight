using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Knight.Framework.UI
{
    public class BindableViewModel<T1, T2> where T1 : Component where T2 : ViewModel
    {
        public T1 ViewObject;
        public T2 ViewModel;
        public string ViewModelName;

        public BindableViewModel()
        {
            this.ViewObject = null;
            this.ViewModel = null;
            this.ViewModelName = string.Empty;
        }
    }

    public class BindableMember<T> where T : MemberInfo
    {
        public object Owner;
        public T Member;
        public Type ObjectType;

        public string ObectTypeName => this.ObjectType.Name;
        public string MemberName => this.Member.Name;

        public BindableMember(object rOwner, T rMemberInfo, Type rObjectType)
        {
            this.Owner = rOwner;
            this.Member = rMemberInfo;
            this.ObjectType = rObjectType;
        }
    }

    public class BindableProperty
    {
        public object PropertyOwner;
        public string PropertyOwnerKey;
        public string PropertyName;
        public Type PropertyType;

        public BindableProperty(object rPropertyOwner, string rPropertyOwnerKey, string rPropertyName)
        {
            this.PropertyOwner = rPropertyOwner;
            this.PropertyOwnerKey = rPropertyOwnerKey;
            this.PropertyName = rPropertyName;
            this.PropertyType = this.PropertyOwner?.GetType()?.GetProperty(rPropertyName)?.PropertyType;
        }
    }

    public class BindableEvent
    {
        public Component Component { get; set; }
        public UnityEventBase UnityEvent { get; set; }
        public string Name { get; set; }
        public Type DeclaringType { get; set; }
        public Type ComponentType { get; set; }
    }
}
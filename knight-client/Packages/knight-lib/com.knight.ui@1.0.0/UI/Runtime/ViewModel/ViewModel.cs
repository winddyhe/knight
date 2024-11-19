using Google.Protobuf;
using Knight.Core;
using System;
using System.Collections.Generic;

namespace Knight.Framework.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModelInitializeAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ViewModelKeyAttribute : Attribute
    {
        public string Key;

        public ViewModelKeyAttribute(string rKey)
        {
            this.Key = rKey;
        }
    }

    public class ViewModelTypes : TypeSearchDefault<ViewModel>
    {
    }

    [TSIgnore]
    public abstract class ViewModel
    {
        public delegate void OnPropertyChangeHandler<T>(T rValue);

        protected Dictionary<string, List<Delegate>> mPropertyChangeHandlers = new Dictionary<string, List<Delegate>>();

        public void PropChanged<T>(string rPropName, T rValue)
        {
            if (this.mPropertyChangeHandlers.TryGetValue(rPropName, out var rDelegateList))
            {
                foreach (var rDelegate in rDelegateList)
                {
                    var rOnPropertyChangeHandler = rDelegate as OnPropertyChangeHandler<T>;
                    rOnPropertyChangeHandler?.Invoke(rValue);
                }
            }
        }

        public void RegisterPropertyChangeHandler<T>(string rPropName, OnPropertyChangeHandler<T> rOnPropertyChangeHandler)
        {
            if (!this.mPropertyChangeHandlers.TryGetValue(rPropName, out var rDelegateList))
            {
                rDelegateList = new List<Delegate> { rOnPropertyChangeHandler };
                this.mPropertyChangeHandlers.Add(rPropName, rDelegateList);
            }
            else
            {
                rDelegateList.Add(rOnPropertyChangeHandler);
            }
        }

        public void UnregisterPropertyChangeHandler<T>(string rPropName, OnPropertyChangeHandler<T> rOnPropertyChangeHandler)
        {
            if (this.mPropertyChangeHandlers.TryGetValue(rPropName, out var rDelegateList))
            {
                rDelegateList.Remove(rOnPropertyChangeHandler);
            }
        }

        public virtual void SyncData(IMessage rMessage)
        {
        }
    }
}

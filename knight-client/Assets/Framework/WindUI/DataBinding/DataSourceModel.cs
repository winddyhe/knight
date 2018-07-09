using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEngine.UI
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class DataBindingAttribute : Attribute
    {
    }

    public class DataSourceModel : MonoBehaviour
    {
        public string ModelClass;
    }
}
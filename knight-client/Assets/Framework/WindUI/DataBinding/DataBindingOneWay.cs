using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class DataBindingOneWay : MonoBehaviour
    {
        [Dropdown("xx")]
        public string ModelPath;
        public string ViewPath;

        public string[] xx;

        void Start()
        {
        }
        
        void Update()
        {
        }
    }
}
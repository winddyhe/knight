using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    public class InputFieldAssist : MonoBehaviour
    {
        public class OnChangeEvent : UnityEvent<string>
        {
        }
        
        public string           Content
        {
            get { return this.InputField?.text;  }
            set { if (this.InputField) this.InputField.text = value; }
        }

        public InputField       InputField;
        public OnChangeEvent    OnChangedFunc = new OnChangeEvent();
        
        private void Awake()
        {
            if (this.InputField == null)
            {
                this.InputField = this.GetComponent<InputField>();
            }
            this.InputField.onValueChanged.AddListener(this.OnValueChangedHandler);
        }
        
        private void OnValueChangedHandler(string rContent)
        {
            this.OnChangedFunc?.Invoke(rContent);
        }
    }
}
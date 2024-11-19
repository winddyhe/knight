using System;
using UnityEngine;

namespace Game.Test
{
    public class UITest1 : MonoBehaviour
    {
        public Action<string, dynamic> PropChangeHandler;

        void Start()
        {
            this.PropChangeHandler += this.SetValue;
        }

        void Update()
        {
            this.PropChangeHandler("Test1", 1000.0f);
        }

        private void SetValue(string s1, dynamic s2)
        {
            var rPos = this.transform.position;
            rPos.x = s2;
        }

        [ContextMenu("Test1")]
        public void Test1()
        {
            ViewModelTest test = new ViewModelTest();
            test.Age = 10;
            test.Name = "Test"; 
        }
    }
}

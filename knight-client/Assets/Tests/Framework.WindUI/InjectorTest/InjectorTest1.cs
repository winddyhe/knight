using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Test
{
    public class InjectorTest1 : MonoBehaviour
    {
        void Start()
        {
            var rClassA = new InjectorTest.ClassA();
            rClassA.OnPropChanged = () => 
            {
                Debug.LogError("hahahahahahaha inject success!!!");
            };
            rClassA.A = "xxxx";

            System.Console.WriteLine("xxx");
        }
    }
}
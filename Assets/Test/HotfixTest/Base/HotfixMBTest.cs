using UnityEngine;
using System.Collections;
using Core;

namespace Test
{
    public class HotfixMBTest : MonoBehaviour
    {
        void Start()
        {

        }
        
        void Update()
        {

        }

        public void Test1()
        {
            Debug.LogError("Test1 in U3D project..");
            Dict<int, string> d = new Dict<int, string>();
            for (int i = 0; i < 10; i++)
            {
                d.Add(i, i.ToString());
            }

            foreach (var rItem in d)
            {
                Debug.LogError(rItem.Key + ", " + rItem.Value);
            }
        }
    }
}
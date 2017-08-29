using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Awake()
        {
            GameObject.DontDestroyOnLoad(this);
        }
    }
}

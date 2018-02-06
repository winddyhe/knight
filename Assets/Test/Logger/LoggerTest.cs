using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class LoggerTest : MonoBehaviour
    {
        void Start()
        {
            Core.Logger.Info("xx1xxx");
            Core.Logger.Info("xx2xxx");
            Core.Logger.Info("xx3xxx");
        }
    }
}
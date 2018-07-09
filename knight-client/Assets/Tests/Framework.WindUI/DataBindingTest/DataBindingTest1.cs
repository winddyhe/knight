using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Test
{
    [DataBinding]
    public class DataBindingTest1
    {
        public string   A;

        public int      B;

        public float    C { get; set; }
    }

    [DataBinding]
    public class DataBindingTest2
    {
        public string   A;

        public int      B;

        public float    C { get; set; }
    }
}

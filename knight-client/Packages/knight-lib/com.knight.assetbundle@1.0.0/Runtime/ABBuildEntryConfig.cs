using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.Assetbundle
{
    [CreateAssetMenu(fileName = "ABBuildEntryConfig", menuName = "Assetbundle/ABBuildEntryConfig")]
    public class ABBuildEntryConfig : ScriptableObject
    {
        public List<ABBuildEntry> ABBuildEntries;
    }
}
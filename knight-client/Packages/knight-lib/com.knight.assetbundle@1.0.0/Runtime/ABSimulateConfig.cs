using UnityEngine;
using Knight.Core;

namespace Knight.Framework.Assetbundle
{
    /// <summary>
    /// AssetBundle 模拟配置
    /// </summary>
    [CreateAssetMenu(fileName = "ABSimulateConfig", menuName = "Assetbundle/ABSimulateConfig")]
    public class ABSimulateConfig : ScriptableObject
    {
        public bool IsDevelopMode;
        public bool IsHotfixDebugMode;
        public ABSimuluateType SimulateType;
    }
}
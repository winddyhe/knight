using NaughtyAttributes;
using System.Collections;

namespace UnityEngine.UI
{    
    [DefaultExecutionOrder(90)]
    [System.Serializable]
    public class ViewModelDataSource : MonoBehaviour
    {
        [InfoBox("ViewClass can not be null.", InfoBoxType.Error, "IsKeyNull")]
        public string   Key;
        public string   ViewModelPath;

        private bool IsKeyNull()
        {
            return string.IsNullOrEmpty(this.Key);
        }
    }
}
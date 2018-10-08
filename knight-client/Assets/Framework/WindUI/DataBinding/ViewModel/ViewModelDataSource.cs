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

        [Dropdown("ViewModelClasses")]
        public string   ViewModelPath;

        [HideInInspector]
        public string[] ViewModelClasses = new string[0];

        private bool IsKeyNull()
        {
            return string.IsNullOrEmpty(this.Key);
        }

        public void GetPaths()
        {
            this.ViewModelClasses = DataBindingTypeResolve.GetAllViewModels().ToArray();
        }
    }
}
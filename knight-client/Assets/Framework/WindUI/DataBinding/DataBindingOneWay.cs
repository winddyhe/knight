using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    public partial class DataBindingOneWay : MonoBehaviour
    {
        [Dropdown("ModelPaths")]
        public  string          CurModelPath;
        [ShowIf("IsShowCurViewPath")]
        public  string          CurViewPath;
        
        [ReadOnly]
        public  DataSourceModel CurDataSource;

        private void Awake()
        {
            this.GetAllModelPaths();
        }

        public void GetAllModelPaths()
        {
            this.ModelPaths = DataBindingTypeResolve.GetAllModelPaths(this.gameObject).ToArray();
        }
    }
}
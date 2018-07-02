using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    public class DataBindingOneWay : MonoBehaviour
    {
        [Dropdown("ModelPaths")]
        public  string          CurModelPath;
        public  string          CurViewPath;

        public  DataSourceModel CurDataSource;

        private string[]        ModelPaths;

        private void Awake()
        {
            var rModelPathList = new List<string>();
            var rAllDataSources = this.gameObject.GetComponentsInParent<DataSourceModel>(true);
            for (int i = 0; i < rAllDataSources.Length; i++)
            {
                var rClassName = rAllDataSources[i].ModelClass;
                rClassName = rClassName.Replace('.', '/');
                rModelPathList.Add(rClassName);
            }
            this.ModelPaths = rModelPathList.ToArray();
        }

        private void Update()
        {
        }
    }
}
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEngine.UI
{
    public class ModelDataItem
    {
        public string           ModelPath;

        public DataSourceModel  DataSource;
        public Type             ModelType;
        public Type             ModelVaribleType;
    }

    public class ViewDataItem
    {
        public string           ViewPath;

        public Component        ViewComp;
        public Type             ViewType;
        public Type             ViewVaribleType;
    }

    [ExecuteInEditMode]
    public partial class DataBindingOneWay : MonoBehaviour
    {
        [Dropdown("ModelPaths")]
        public  string          CurModelPath;
        [ShowIf("IsShowCurViewPath")]
        [Dropdown("ViewPaths")]
        public  string          CurViewPath;
        
        [ReadOnly]
        public  DataSourceModel CurDataSource;

        private void Awake()
        {
        }
    }
}
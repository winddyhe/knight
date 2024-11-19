using Knight.Core;
using NaughtyAttributes;
using System.Reflection;
using UnityEngine;

namespace Knight.Framework.UI
{
    [ExecuteInEditMode]
    public partial class RedPoint : MonoBehaviour
    {
        [SerializeField]
        [Dropdown("RedPointBindPaths")]
        [OnValueChanged("GetPaths")]
        protected string mBindPath;

        [SerializeField]
        protected bool mIsFormat;
        [SerializeField]
        [ShowIf("mIsFormat")]
        protected string mParam0;
        [SerializeField]
        [ShowIf("mIsFormat")]
        protected string mParam1;
        [SerializeField]
        [ShowIf("mIsFormat")]
        protected string mParam2;
        [SerializeField]
        [ShowIf("mIsFormat")]
        protected string mParam3;
        [SerializeField]
        [ShowIf("mIsFormat")]
        protected string mParam4;
        
        public string BindPath { get { return this.mBindPath; } set { this.mBindPath = value; this.OnParamChange(); } }
        public bool IsFormat { get { return this.mIsFormat; } set { this.mIsFormat = value; this.OnParamChange(); } }
        public string Param0 { get { return this.mParam0; } set { this.mParam0 = value; this.OnParamChange(); } }
        public string Param1 { get { return this.mParam1; } set { this.mParam1 = value; this.OnParamChange(); } }
        public string Param2 { get { return this.mParam2; } set { this.mParam2 = value; this.OnParamChange(); } }
        public string Param3 { get { return this.mParam3; } set { this.mParam3 = value; this.OnParamChange(); } }
        public string Param4 { get { return this.mParam4; } set { this.mParam4 = value; this.OnParamChange(); } }

        protected int mRedPointNumber;
        protected string mRealBindPath;

        private void Awake()
        {
#if UNITY_EDITOR
            this.GetPaths();
#endif
            this.mRealBindPath = string.Empty;
            this.mRedPointNumber = 0;
        } 

        private void OnEnable()
        {
            this.OnParamChange();
        }

        private void OnDisable()
        {
            var rRedPointNode = RedPointManager.Instance?.SearchRedPoint(this.mRealBindPath);
            if (rRedPointNode != null)
            {
                rRedPointNode.OnRedPointChanged -= this.UpdateRedPointNumber;
            }
        }

        private void OnParamChange()
        {
            var rRealBindPath = this.mBindPath;
            if (this.mIsFormat)
            {
                rRealBindPath = string.Format(this.mBindPath, this.mParam0, this.mParam1, this.mParam2, this.mParam3, this.mParam4);
            }
            if (rRealBindPath == this.mRealBindPath)
            {
                return;
            }

            this.mRealBindPath = rRealBindPath;
            var rRedPointNode = RedPointManager.Instance?.SearchRedPoint(rRealBindPath);
            this.mRedPointNumber = 0;
            if (rRedPointNode != null)
            {
                rRedPointNode.OnRedPointChanged -= this.UpdateRedPointNumber;
                rRedPointNode.OnRedPointChanged += this.UpdateRedPointNumber;
                this.mRedPointNumber = rRedPointNode.TotalNumber;
            }
            this.UpdateRedPointNumber(this.mRedPointNumber);
        }

        protected virtual void UpdateRedPointNumber(int nRedPointNumber)
        {
        }
    }

#if UNITY_EDITOR
    public partial class RedPoint
    {
        private string[] RedPointBindPaths = new string[0];

        public void GetPaths()
        {
            var rGameHotfixAssembly = System.Reflection.Assembly.Load("Game.Hotfix");
            if (rGameHotfixAssembly == null)
            {
                Debug.LogError("Cannot find assembly: Game.Hotfix.");
                return;
            }
            var rRedPointConfigType = rGameHotfixAssembly.GetType("Game.RedPointConfig");
            if (rRedPointConfigType == null)
            {
                Debug.LogError("Cannot find type: Game.RedPointConfig.");
                return;
            }

            var rAllFieldInfos = rRedPointConfigType.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);
            this.RedPointBindPaths = new string[rAllFieldInfos.Length];
            for (int i = 0; i < rAllFieldInfos.Length; i++)
            {
                this.RedPointBindPaths[i] = rAllFieldInfos[i].GetValue(null).ToString();
            }
        }

        [Button("Refresh")]
        public void Refresh()
        {
            this.GetPaths();
        }
    }
#endif
}

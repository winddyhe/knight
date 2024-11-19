using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Knight.Core;

namespace Knight.Framework.UI
{
    public class GameObjectActive : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private bool mIsActive;

        public bool IsActive
        {
            get
            {
                this.mIsActive = this ? this.gameObject.activeSelf : false;
                return this.mIsActive;
            }
            set
            {
                this.mIsActive = value;
                if (this)
                {
                    this.gameObject.SetActiveSafe(this.mIsActive);
                }
            }
        }

        public bool IsDeActive
        {
            get
            {
                this.mIsActive = this ? this.gameObject.activeSelf : false;
                return !this.mIsActive;
            }
            set
            {
                this.mIsActive = !value;
                if (this)
                {
                    this.gameObject.SetActiveSafe(this.mIsActive);
                }
            }
        }
    }
}

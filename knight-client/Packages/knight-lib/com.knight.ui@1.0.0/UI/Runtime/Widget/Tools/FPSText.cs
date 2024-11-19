using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Framework.UI
{
    public class FPSText : MonoBehaviour
    {
        private float mUpdateInterval = 1.0f;
        private float mAccum = 0.0f; 
        private float mFrames = 0;   
        private float mTimeleft;     
        private float mFps = 15.0f;  
        private float mLastSample;
        private float mGotIntervals = 0;
        private StringBuilder mStringBuilder;

        public TextMeshProUGUI FpsText;

        void Start()
        {
            this.mStringBuilder = new StringBuilder();
            this.mTimeleft = this.mUpdateInterval;
            this.mLastSample = Time.realtimeSinceStartup;
        }

        void Update()
        {
            ++this.mFrames;
            float newSample = Time.realtimeSinceStartup;
            float deltaTime = newSample - this.mLastSample;
            this.mLastSample = newSample;
            this.mTimeleft -= deltaTime;
            this.mAccum += 1.0f / deltaTime;

            if (this.mTimeleft <= 0.0f)
            {
                this.mFps = this.mAccum / this.mFrames;
                this.mTimeleft = this.mUpdateInterval;
                this.mAccum = 0.0f;
                this.mFrames = 0;
                ++this.mGotIntervals;
                if (this.mStringBuilder != null)
                {
                    float ms = 1000.0f / mFps;
                    this.mStringBuilder.Clear();
                    this.mStringBuilder.Append("FPS: ");
                    this.mStringBuilder.Append(mFps.ToString("F2"));
                    this.mStringBuilder.Append(", MS: ");
                    this.mStringBuilder.Append(ms.ToString("F2"));
                    this.FpsText.text = this.mStringBuilder.ToString();
                }
            }
        }
    }
}

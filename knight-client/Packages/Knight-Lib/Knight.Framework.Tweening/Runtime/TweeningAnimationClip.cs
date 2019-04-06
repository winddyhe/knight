using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight.Framework.Tweening
{
    public class TweeningAnimationClip
    {
        public float            CurTime;
        public float            TotalTime;

        public bool             IsLoop;
        public int              LoopCount;
        
        public Action<float>    UpdateAction;

        public void Update(float fDeltaTime)
        {
        }

        public void FixedUpdate(float fDeltaTime)
        {
        }
    }
}

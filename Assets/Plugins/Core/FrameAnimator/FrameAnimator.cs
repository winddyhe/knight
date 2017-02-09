//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class FrameAnimator : MonoBehaviour
    {
        public List<Sprite> sprites;

        public SpriteRenderer spriteRender;

        public float deltaTime;

        private float curTime;
        private int curIndex;

        void Update()
        {
            this.curTime += Time.unscaledDeltaTime;

            if (this.curTime > deltaTime)
            {
                curIndex++;

                if (curIndex >= sprites.Count)
                    curIndex = 0;

                spriteRender.sprite = sprites[curIndex];

                this.curTime = 0;
            }
        }
    }
}

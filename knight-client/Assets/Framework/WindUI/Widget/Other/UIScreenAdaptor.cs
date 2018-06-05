using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UIScreenAdaptor : MonoBehaviour
    {
        public float screenHeight;
        public float screenWidth;

        void Start()
        {
            UpdateCamera();
        }

        void UpdateCamera()
        {
            screenHeight = Screen.height;
            screenWidth = Screen.width;

            Rect r = new Rect();
            if (screenHeight / screenWidth > 0.61f)//更方的屏幕 480*800  
            {
                r.width = 1;
                r.height = (screenWidth * 0.61f) / screenHeight;
                r.x = 0;
                r.y = (1 - r.height) / 2f;
            }
            else if (screenHeight / screenWidth < 0.56f)//更长的屏幕480*854  
            {

                r.width = (screenHeight / 0.56f) / screenWidth;
                r.height = 1f;
                r.x = (1 - r.width) / 2f;
                r.y = 0;
            }
            else //在可适配区域不做处理480*800 - 480*854之间的  
            {
                r.width = 1;
                r.height = 1;
                r.x = 0;
                r.y = 0f;
            }
            GetComponent<Camera>().rect = r;
        }
    }
}


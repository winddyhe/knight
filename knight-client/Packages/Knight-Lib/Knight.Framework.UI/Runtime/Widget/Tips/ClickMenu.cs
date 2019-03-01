using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    public class ClickMenu : MonoBehaviour
    {
        private GameObject kNewMenu;

        private float mScaleWidth;
        private float mScaleHeight;

        private List<Vector2> mMenus;
        private Vector2 mLeftTop;
        private Vector2 mRightTop;
        private Vector2 mLeftBottom;
        private Vector2 mRightBottom;

        private RectTransform mMenuRect;
        private Canvas mCanvas;

        private Vector2 mLocalPosition;

        private static ClickMenu __instance;
        public static ClickMenu Instance { get { return __instance; } }

        private void Awake()
        {
            if (__instance == null)
                __instance = this;
        }

        private void Start()
        {
            mLeftTop = new Vector2(1, 0);
            mRightTop = new Vector2(0, 0);
            mRightBottom = new Vector2(0, 1);
            mLeftBottom = new Vector2(1, 1);

            mMenus = new List<Vector2>() { mLeftTop, mRightTop, mRightBottom, mLeftBottom };

            mCanvas = FindObjectOfType<Canvas>();

        }

        private void Update()
        {

        }


        private int CheckEdge(Vector3 rLocalPositon)
        {
            float mHalfCwidth = mCanvas.pixelRect.width / 2;
            float mHalfCHeight = mCanvas.pixelRect.height /2;

            if (rLocalPositon.x + mScaleWidth <= mHalfCwidth && rLocalPositon.y - mScaleHeight >= -mHalfCHeight)
            {
                return 2;

            }
            else if (rLocalPositon.x - mScaleWidth >= -mHalfCwidth && rLocalPositon.y - mScaleHeight >= -mHalfCHeight)
            {
                return 3;
            }
            else if (rLocalPositon.x - mScaleWidth >= -mHalfCwidth && rLocalPositon.y + mScaleHeight <= mHalfCHeight)
            {
                return 0;
            }
            else if (rLocalPositon.x + mScaleWidth <= mHalfCwidth && rLocalPositon.y + mScaleHeight <= mHalfCHeight)
            {
                return 1;

            }
            else
                return 4;

        }

        private void SetMenu(Vector2 rPivot, Vector2 rScaleSize)
        {
            mMenuRect.sizeDelta = rScaleSize;
            mMenuRect.anchorMin = new Vector2(0, 0);
            mMenuRect.anchorMax = new Vector2(0, 0);
            mMenuRect.localScale = new Vector3(1, 1, 1);
            mMenuRect.pivot = rPivot;

            mMenuRect.localPosition = mLocalPosition;
        }

        public GameObject CreateMenu(Vector3 rCurMousePositon, GameObject rNewObj,float fWidth,float fHeight)
        {
            kNewMenu = rNewObj;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(mCanvas.transform as RectTransform, rCurMousePositon, mCanvas.worldCamera, out mLocalPosition);

            mScaleWidth = fWidth * mCanvas.scaleFactor;
            mScaleHeight = fHeight * mCanvas.scaleFactor;

            int mIndex = this.CheckEdge(new Vector2(mLocalPosition.x, mLocalPosition.y));

            if (mMenuRect == null)
            {
                kNewMenu.transform.parent = mCanvas.transform;

                mMenuRect = kNewMenu.GetComponent<RectTransform>();
            }
            else
            {
                kNewMenu.SetActive(true);
            }

            if (mIndex < mMenus.Count)
                this.SetMenu(mMenus[mIndex], new Vector2(mScaleWidth, mScaleHeight));

            return kNewMenu;
        }

        public void CloseMenu()
        {
            if(kNewMenu != null)
                kNewMenu.SetActive(false);
        }

    }

}

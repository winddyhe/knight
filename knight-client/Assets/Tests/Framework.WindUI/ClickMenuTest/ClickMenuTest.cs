using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knight.Core;
using Knight.Framework;
using UnityEngine.UI;

namespace Game.Test
{
    public class ClickMenuTest : MonoBehaviour
    {

        GameObject mTestMenu;

        void Start()
        {
            mTestMenu = new GameObject();
            mTestMenu.AddComponent<Image>();
        }

        void Update()
        {
            if (Input.GetMouseButton(1))
                ClickMenu.Instance.CreateMenu(Input.mousePosition, mTestMenu, 200, 300);

            if (Input.GetMouseButtonDown(0))
                ClickMenu.Instance.CloseMenu();

        }
    }
}

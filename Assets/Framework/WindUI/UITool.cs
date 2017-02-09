//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Framework.WindUI
{
    public static class UITool
    {
        /// <summary>
        /// 向一个UI结点添加孩子结点
        /// </summary>
        public static GameObject AddChild(this Transform rParent, GameObject rPrefabGo, string rLayerName = "UI")
        {
            if (rParent == null || rPrefabGo == null) return null;
    
            GameObject rTargetGo = GameObject.Instantiate(rPrefabGo);
            rTargetGo.name = rPrefabGo.name;
            rTargetGo.transform.SetParent(rParent, false);
            rTargetGo.transform.localScale = Vector3.one;
            rTargetGo.SetLayer(rLayerName);
    
            return rTargetGo;
        }
    
        /// <summary>
        /// 递归设置一个节点的层
        /// </summary>
        public static void SetLayer(this GameObject rGo, string rLayerName)
        {
            if (rGo == null) return;
    
            SetLayer(rGo.transform, rLayerName);
        }
    
        /// <summary>
        /// 设置一个GameObject的层
        /// </summary>
        public static void SetLayer(Transform rParent, string rLayerName)
        {
            if (rParent == null) return;
    
            rParent.gameObject.layer = LayerMask.NameToLayer(rLayerName);
    
            for (int i = 0; i < rParent.transform.childCount; i++)
            {
                var rTrans = rParent.transform.GetChild(i);
                SetLayer(rTrans, rLayerName);
            }
        }
    }
}
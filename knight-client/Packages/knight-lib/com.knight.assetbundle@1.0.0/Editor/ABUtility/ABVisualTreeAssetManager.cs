using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knight.Framework.Assetbundle.Editor
{
    public class ABVisualTreeAssetManager
    {
        private static VisualTreeAsset mABBuilderWindowTreeAsset;
        public static VisualTreeAsset ABBuilderWindowTreeAsset
        {
            get
            {
                if (mABBuilderWindowTreeAsset == null)
                    mABBuilderWindowTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.knight.assetbundle/Editor/UXML/ABBuilderWindow.uxml");
                return mABBuilderWindowTreeAsset;
            }
        }

        private static VisualTreeAsset mABBuilderConfigPanelAsset;
        public static VisualTreeAsset ABBuilderConfigPanelAsset
        {
            get
            {
                if (mABBuilderConfigPanelAsset == null)
                    mABBuilderConfigPanelAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.knight.assetbundle/Editor/UXML/ABBuilderConfigPanel.uxml");
                return mABBuilderConfigPanelAsset;
            }
        }

        private static VisualTreeAsset mABBuilderPanelAsset;
        public static VisualTreeAsset ABBuilderPanelAsset
        {
            get
            {
                if (mABBuilderPanelAsset == null)
                    mABBuilderPanelAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.knight.assetbundle/Editor/UXML/ABBuilderPanel.uxml");
                return mABBuilderPanelAsset;
            }
        }

        private static VisualTreeAsset mABBuildEntryTreeAsset;
        public static VisualTreeAsset ABBuildEntryTreeAsset
        {
            get
            {
                if (mABBuildEntryTreeAsset == null)
                    mABBuildEntryTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.knight.assetbundle/Editor/UXML/ABBuildEntry.uxml");
                return mABBuildEntryTreeAsset;
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Knight.Framework.Assetbundle.Editor
{
    public class ABBuilderWindow : EditorWindow
    {
        private int mPanelIndex = 0;
        private ABBuilderConfigPanel mABBuilderConfigPanel;
        private StyleColor mButtonColor;

        [MenuItem("Tools/Assetbundle/ABBuilderWindow")]
        public static void ShowWindow()
        {
            var rWindow = EditorWindow.GetWindow<ABBuilderWindow>();
            rWindow.titleContent = new GUIContent("ABBuilderWindow");
        }

        private void OnEnable()
        {
            ABPlatform.Instance.Initialize();

            var rVisualTreeXML = ABVisualTreeAssetManager.ABBuilderWindowTreeAsset;
            rVisualTreeXML.CloneTree(this.rootVisualElement);

            var rLabelConfigButton = this.rootVisualElement.Q<Button>("LabelConfigButton");
            rLabelConfigButton.clicked += this.OnLabelConfigButtonClicked;

            var rBuilderButton = this.rootVisualElement.Q<Button>("BuilderButton");
            rBuilderButton.clicked += this.OnBuilderButtonClicked;

            // 创建 ABBuilderConfigPanel
            this.mABBuilderConfigPanel = new ABBuilderConfigPanel();
            var rLabelConfigPanel = this.rootVisualElement.Q<VisualElement>("LabelConfigPanel");
            this.mABBuilderConfigPanel.Build(rLabelConfigPanel);

            // 创建ABBuilderPanel
            var rABBuilderPanel = new ABBuilderPanel();
            var rBuilderPanel = this.rootVisualElement.Q<VisualElement>("BuilderPanel");
            rABBuilderPanel.Build(rBuilderPanel);

            // 设置默认选中 0
            this.mButtonColor = rLabelConfigButton.style.backgroundColor;
            this.OnTabSelected(0);
        }

        private void OnLabelConfigButtonClicked()
        {
            this.OnTabSelected(0);
        }

        private void OnBuilderButtonClicked()
        {
            this.OnTabSelected(1);
        }

        private void OnTabSelected(int nIndex)
        {
            var rLabelConfigButton = this.rootVisualElement.Q<Button>("LabelConfigButton");
            var rBuilderButton = this.rootVisualElement.Q<Button>("BuilderButton");
            var rLabelConfigPanel = this.rootVisualElement.Q<VisualElement>("LabelConfigPanel");
            var rBuilderPanel = this.rootVisualElement.Q<VisualElement>("BuilderPanel");

            this.mPanelIndex = nIndex;
            if (nIndex == 0)
            {
                rLabelConfigButton.style.backgroundColor = Color.gray;
                rBuilderButton.style.backgroundColor = this.mButtonColor;

                rLabelConfigPanel.style.display = DisplayStyle.Flex;
                rBuilderPanel.style.display = DisplayStyle.None;
            }
            else if (nIndex == 1)
            {
                rBuilderButton.style.backgroundColor = Color.gray;
                rLabelConfigButton.style.backgroundColor = this.mButtonColor;

                rBuilderPanel.style.display = DisplayStyle.Flex;
                rLabelConfigPanel.style.display = DisplayStyle.None;
            }
        }
    }
}


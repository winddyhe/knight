using Knight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Knight.Framework.UI.Editor
{
    [InitializeOnLoad]
    public class ViewPrefabAutoSave
    {
        static ViewPrefabAutoSave()
        {
            PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceApplied; 
        }

        private static void OnPrefabInstanceApplied(GameObject instance)
        {
            var rViewControlllerDataSource = instance.GetComponent<ViewControllerDataSource>();
            if (rViewControlllerDataSource == null) return;
            AutoSave(rViewControlllerDataSource);
        }

        public static void AutoSave(ViewControllerDataSource rViewControlllerDataSource)
        {
            rViewControlllerDataSource.GetPaths();
            var rViewCodeGenerator = new DataBindingView_CodeGenerator(
                $"Assets/Game.Hotfix/Game/GUI/View/{rViewControlllerDataSource.ViewName}.cs",
                rViewControlllerDataSource.gameObject,
                rViewControlllerDataSource.ViewControllerName);
            rViewCodeGenerator.CodeGenerate();
            rViewCodeGenerator.Save();

            var rAllFancyScrollRectCellItems = rViewControlllerDataSource.gameObject.GetComponentsInChildren<FancyScrollRectCellItem>(true);
            var rFancyScrollRectCodeGeneratorList = new List<DataBindingListTemplateCellBridge>();
            foreach (var rFancyScrollRectCellItem in rAllFancyScrollRectCellItems)
            {
                var rTemplateType = TypeResolveManager.Instance.GetType(rFancyScrollRectCellItem.DataSource.TemplatePath);
                if (rTemplateType == null) continue;

                var rFancyScrollRectCellItemCodeGenerator = new DataBindingListTemplateCellBridge(
                    $"Assets/Game.Hotfix/Game/GUI/View/{rTemplateType.Name}CellBridge.cs",
                    rFancyScrollRectCellItem.gameObject,
                    rViewControlllerDataSource.ViewName);
                rFancyScrollRectCellItemCodeGenerator.CodeGenerate();
                rFancyScrollRectCellItemCodeGenerator.Save();
                rFancyScrollRectCodeGeneratorList.Add(rFancyScrollRectCellItemCodeGenerator);
            }

            AssetDatabase.Refresh();
            rViewCodeGenerator.AddViewMonobehaviour(rViewControlllerDataSource.gameObject, rViewControlllerDataSource.ViewName);
            foreach (var rFancyScrollRectCodeGenerator in rFancyScrollRectCodeGeneratorList)
            {
                rFancyScrollRectCodeGenerator.AddViewMonobehaviour(rFancyScrollRectCodeGenerator.GameObject, $"{rFancyScrollRectCodeGenerator.ViewModelType.Name}CellBridge");
            }
        }
    }
}

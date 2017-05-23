//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Core.Editor
{
    public class ExcelReaderWindow : EditorWindow
    {
        private ExcelReader mExcelReader;

        private Vector2     mScrollPos = new Vector2(210, 600);

        [MenuItem("Tools/AssetBundle/Export Excel Config")]
        private static void Init()
        {
            var rWindow = EditorWindow.GetWindow<ExcelReaderWindow>();
            rWindow.Show();
        }

        private void OnEnable()
        {
            this.Initialize();
        }
        
        private void Initialize()
        {
            mExcelReader = new ExcelReader();
            mExcelReader.Load();
        }

        private void OnGUI()
        {
            if (mExcelReader == null || mExcelReader.ExcelFormatConfig == null) return;

            using (var space = new EditorGUILayout.VerticalScope())
            {
                mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
                var rFormatConfig = mExcelReader.ExcelFormatConfig;
                for (int i = 0; i < rFormatConfig.Count; i++)
                {
                    using(var space1 = new EditorGUILayout.HorizontalScope("TextField"))
                    {
                        EditorGUILayout.LabelField(rFormatConfig[i].ExcelName, GUILayout.MaxWidth(150));
                        EditorGUILayout.LabelField("|", GUILayout.Width(15));
                        EditorGUILayout.LabelField(rFormatConfig[i].SheetName, GUILayout.MaxWidth(100));
                        EditorGUILayout.LabelField("|", GUILayout.Width(15));
                        EditorGUILayout.LabelField(rFormatConfig[i].PrimaryKey, GUILayout.MaxWidth(60));

                        if (GUILayout.Button("导出", GUILayout.Width(60)))
                        {
                            mExcelReader.Export(rFormatConfig[i]);
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.Space();
                if (GUILayout.Button("全部导出"))
                {
                    mExcelReader.ExportAll();
                    EditorUtility.DisplayDialog("Tips", "导出Excel完成！", "OK");
                }
                EditorGUILayout.Space();
            }
        }
    }
}

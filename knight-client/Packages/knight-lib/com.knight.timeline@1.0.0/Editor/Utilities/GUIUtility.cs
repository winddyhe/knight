using System.Collections;
using UnityEngine;

namespace Knight.Framework.Timeline.Editor
{
    public static class GUIUtility
    {
        private static Hashtable s_TextGUIContents;
        static GUIUtility()
        {

            s_TextGUIContents = new Hashtable();
        }
        public static string[] GetNameAndTooltipString(string nameAndTooltip)
        {
            string[] array = new string[3];
            string[] array2 = nameAndTooltip.Split('|');
            switch (array2.Length)
            {
                case 0:
                    array[0] = "";
                    array[1] = "";
                    break;
                case 1:
                    array[0] = array2[0].Trim();
                    array[1] = array[0];
                    break;
                case 2:
                    array[0] = array2[0].Trim();
                    array[1] = array[0];
                    array[2] = array2[1].Trim();
                    break;
                default:
                    Debug.LogError("Error in Tooltips: Too many strings in line beginning with '" + array2[0] + "'");
                    break;
            }

            return array;
        }
        public static GUIContent TextContent(string textAndTooltip)
        {
            if (textAndTooltip == null)
            {
                textAndTooltip = "";
            }

            string key = textAndTooltip;
            GUIContent gUIContent = (GUIContent)s_TextGUIContents[key];
            if (gUIContent == null)
            {
                string[] nameAndTooltipString = GetNameAndTooltipString(textAndTooltip);
                gUIContent = new GUIContent(nameAndTooltipString[1]);
                if (nameAndTooltipString[2] != null)
                {
                    gUIContent.tooltip = nameAndTooltipString[2];
                }

                s_TextGUIContents[key] = gUIContent;
            }

            return gUIContent;
        }
    }
}

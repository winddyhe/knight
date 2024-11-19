using System.Collections.Generic;
using UnityEngine;

namespace Knight.Core
{
    [CreateAssetMenu(menuName = "Localization/LocalizationConfig", fileName = "LocalizationConfig")]
    public class LocalizationConfig : ScriptableObject
    {
        /// <summary>
        /// Default language type when application start.
        /// </summary>
        public MultiLanguageType DefaultLanguageType;
        /// <summary>
        /// Default font name when application start.
        /// </summary>
        public string DefaultFontName;
    }
}

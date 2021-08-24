using UnityEngine;

namespace TedLab
{
    [CreateAssetMenu(menuName = "TedLab/Create LocalizationData", fileName = "TedLab_Localization")]
    public class LocalizationData : ScriptableObject
    {
        /// <summary>
        /// Resourceフォルダからのパス
        /// </summary>
        public string FilePath;
    }
}
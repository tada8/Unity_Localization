using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TedLab
{
    public class LocalizationTextUI : MonoBehaviour
    {
        /// <summary>
        /// UI.Textへの参照
        /// </summary>
        [SerializeField] Text refText = null;

        /// <summary>
        /// TMPro.TextMeshProUGUIへの参照
        /// </summary>
        [SerializeField] TextMeshProUGUI refTextPro = null;

        /// <summary>
        /// CSVでのキー名
        /// </summary>
        [SerializeField] string key = null;

        void Awake()
        {
            UpdateText();
        }

        void UpdateText()
        {
            UpdateText(LocalizationManager.Instance.CurrentLangaugeData);
        }

        /// <summary>
        /// 管理側からの反映
        /// </summary>
        /// <param name="langData"></param>
        public void UpdateText(LocalizationManager.LangaugeData langData)
        {
            string localizedText = null;
            if( langData.Texts.TryGetValue(key, out localizedText) == false ){
                Debug.LogError("指定されたキーのローカライズが見つかりません : " + key);
                localizedText = "Not Found Localized Text.";
            }
            if(string.IsNullOrEmpty(localizedText) == false ){
                if(refText != null) refText.text = localizedText;
                if(refTextPro != null) refTextPro.text = localizedText;
            }
        }
    }
}

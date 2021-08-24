using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace TedLab
{
    public class LocalizationManager : SingletonMonoBehaviour<LocalizationManager>
    {
        public LangaugeData CurrentLangaugeData => m_currentLangaugeData;

        /// <summary>
        /// ローカライズデータ
        /// </summary>
        [SerializeField] LocalizationData localizationData = null;

        /// <summary>
        /// 常駐オプション(別オブジェクトにぶら下げる場合はfalse)
        /// </summary>
        [SerializeField] bool dontDestroyOnLoad = true;

        /// <summary>
        /// デフォルト言語
        /// </summary>
        [SerializeField] string defaultLangaugeName = "";

        /// <summary>
        /// Application.systemLanguageで上書き(CSVでの言語名をenumと合わせる必要あり)
        /// </summary>
        [SerializeField] bool overrideSystemLangauge = false;

        // 言語ごとのテキスト
        public class LangaugeData
        {
            public LangaugeData(string name, int capacity){
                LangaugeName = name;
                Texts = new Dictionary<string, string>(capacity);
            }
            public string LangaugeName;
            public Dictionary<string, string> Texts;
        }

        // 全言語のデータ
        public class LocalizationBuildData
        {
            public LocalizationBuildData(int capacity){
                Localizations = new Dictionary<string, LangaugeData>(capacity);
            }
            public Dictionary<string, LangaugeData> Localizations;
        }

        LocalizationBuildData m_buildData;
        LangaugeData m_currentLangaugeData = null;

        protected override void Awake()
        {
            base.Awake();

            BuildLocalizationData();

            if(overrideSystemLangauge){
                var language = Application.systemLanguage;
                SetLangauge(language.ToString());
            }
            else if(string.IsNullOrEmpty(defaultLangaugeName) == false){
                SetLangauge(defaultLangaugeName);
            }
            if(dontDestroyOnLoad){
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// 言語変更
        /// </summary>
        /// <param name="langaugeName"></param>
        public void SetLangauge(string langaugeName)
        {
            if(m_buildData == null){
                Debug.LogError("ローカライズのビルドデータが存在しません");
                return;
            }
            if(m_currentLangaugeData != null && m_currentLangaugeData.LangaugeName == langaugeName){
                Debug.LogWarning("現在と同じ言語を設定されたのでスキップ");
                return;
            }
            LangaugeData langData = null;
            if( m_buildData.Localizations.TryGetValue(langaugeName, out langData) == false){
                Debug.LogError("ローカライズする言語が見つかりません : " + langaugeName);
                return;
            }
            m_currentLangaugeData = langData;

            // update all text
            {
                var textComps = GameObject.FindObjectsOfType<LocalizationTextUI>();
                foreach(var textComp in textComps){
                    textComp.UpdateText(m_currentLangaugeData);
                }
            }
        }

        void BuildLocalizationData()
        {
            if(localizationData == null){
                Debug.LogError("ローカライズデータが未設定です");
                return;
            }
            var csvData = LoadCsv(localizationData.FilePath);
            if(csvData == null){
                Debug.LogError("ローカライズ用CSVファイルの読み込みに失敗しました : " + localizationData.FilePath);
                return;
            }
            if(csvData.data.Count <= 1){
                Debug.LogError("ローカライズ用CSVファイルには1つ以上のデータを含む必要があります");
                return;
            }
            if(csvData.data[0].Length <= 1){
                Debug.LogError("ローカライズ用CSVファイルには1つ以上のデータを含む必要があります");
                return;
            }

            var lineNum = csvData.data.Count;
            var columnNum = csvData.data[0].Length;
            var localization = new LocalizationBuildData(columnNum);

            for(int column=1; column<columnNum; ++column)
            {
                var languageName = csvData.data[0][column];
                var langData = new LangaugeData(languageName, lineNum);
                for(int line=1; line<lineNum; ++line)
                {
                    string key = csvData.data[line][0];
                    var str = csvData.data[line][column];
                    langData.Texts.Add(key, str);
                }
                localization.Localizations.Add(languageName, langData);
            }

            m_buildData = localization;
        }

        // csv
        class CsvData
        {
            public CsvData(int capa = 1024){
                data = new List<string[]>(capa);
            }

            public List<string[]> data;
        }

        static readonly char[] csvSeparatorDefault = {','};

        CsvData LoadCsv(string path, char[] separator = null)
        {
            var csvText = Resources.Load(path) as TextAsset;
            var reader = new StringReader(csvText.text);
            var current_separator = separator != null ? separator : csvSeparatorDefault;
            var csvData = new CsvData();

            while(reader.Peek() != -1)
            {
                var line = reader.ReadLine();
                csvData.data.Add(line.Split(current_separator));
            }
            return csvData;
        }
    }
}

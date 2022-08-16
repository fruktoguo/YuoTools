using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using YuoTools;

public class LanguageManager : SingletonMono<LanguageManager>
{
    public LanType lanType;

    public LanType DefLanType
    {
        get => LanType.Chinese;
    }

    public override void Awake()
    {
        base.Awake();
        lanType = (LanType)PlayerPrefs.GetInt("LanguageType", 0);
        LoadData();
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("LanguageType", (int)lanType);
    }

    public Dictionary<string, string> language = new Dictionary<string, string>();
    public Language LanTemp;

    private string FilePath(string fileName)
    {
        return $"{Application.dataPath}/StreamingAssets/Language/{fileName}.json";
    }

    [Button]
    private void LoadData()
    {
        if (!File.Exists(FilePath(lanType.ToString())))
        {
            return;
        }
        //LanTemp = JsonUtility.FromJson<Language>(File.ReadAllText(FilePath(lanType.ToString())));
        LanTemp = YuoJson.Load<Language>($"Language/{lanType}.json");
        language.Clear();
        foreach (var item in LanTemp.datas)
        {
            language.Add(item.text, item.translation);
        }
    }

    [Button]
    private void SaveData()
    {
        LanTemp.LanguageType = lanType.ToString();
        YuoJson.Save($"Language/{lanType}.json", LanTemp);
        //string str = JsonUtility.ToJson(LanTemp);
        //FileStream fileStream = new FileStream(FilePath(lanType.ToString()), FileMode.Create, FileAccess.Write);//创建写入文件
        //StreamWriter sw = new StreamWriter(fileStream);
        //sw.WriteLine(str);//开始写入值
        //sw.Close();
        //fileStream.Close();
    }

    public enum LanType
    {
        Chinese = 0,
        Japanese = 1,
    }

    [Serializable]
    public class Language
    {
        public string LanguageType;
        public List<LanguageData> datas = new List<LanguageData>();
    }

    [Serializable]
    public class LanguageData
    {
        public string text;
        public string translation;
    }

    public List<string> temp = new List<string>();
}

namespace YuoTranslation
{
    public static class GameConstEx
    {
        public static string ForTranslation(this string str)
        {
            if (LanguageManager.Instance.language.ContainsKey(str))
                return LanguageManager.Instance.language[str];
            if (!LanguageManager.Instance.temp.Contains(str))
            {
                LanguageManager.Instance.LanTemp.datas.Add(new LanguageManager.LanguageData() { text = str, translation = str });
                LanguageManager.Instance.temp.Add(str);
            }
            return str;
        }
    }
}
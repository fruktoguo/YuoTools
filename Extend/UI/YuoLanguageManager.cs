using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using YuoTools.ECS;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend.UI
{
    public class YuoLanguageManager : YuoComponentGet<YuoLanguageManager>
    {
        public YuoLanguageData data = new();

        public string GetLanguage(string key)
        {
            if (data.Text.TryGetValue(key, out var value))
            {
                return value;
            }

            return key;
        }

        public void ResetDefLanguagePack()
        {
        }

        public async Task<string> CreateLanguageData(string languageType)
        {
            var save = World.Main.GetComponent<SaveManagerComponent>();
            var path = save.GetFilePath($"Language/{languageType}.json");
            var yuoLanguageData = new YuoLanguageData();
            var template = Resources.Load<YuoLanguageTemplate>("YuoLanguageTemplate");

            foreach (var item in template.Text)
            {
                yuoLanguageData.Text.Add(item, item);
            }

            foreach (var item in template.Image)
            {
                yuoLanguageData.Image.Add(item, null);
            }

            foreach (var item in template.Sound)
            {
                yuoLanguageData.Sound.Add(item, null);
            }

            foreach (var item in template.Video)
            {
                yuoLanguageData.Video.Add(item, null);
            }

            await FileHelper.WriteAllText(path, JsonConvert.SerializeObject(yuoLanguageData));

            return path;
        }

        public async Task LoadLanguageData(string languageType)
        {
            var save = World.Main.GetComponent<SaveManagerComponent>();
            var path = save.GetFilePath($"Language/{languageType}.json");
            var json = await FileHelper.ReadAllText(path);
            data = JsonConvert.DeserializeObject<YuoLanguageData>(json);
        }
    }

    public static class YuoLanguageEx
    {
        public static string Language(this string key)
        {
            return YuoLanguageManager.Instance.GetLanguage(key);
        }
    }

    public class YuoLanguageData
    {
        public Dictionary<string, string> Text = new();
        public Dictionary<string, string> Image = new();
        public Dictionary<string, string> Sound = new();
        public Dictionary<string, string> Video = new();
    }
}
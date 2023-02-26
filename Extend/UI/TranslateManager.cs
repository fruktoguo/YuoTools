using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using YuoTools;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace Tools
{
    [AutoAddToMain]
    public class TranslateManager : YuoComponentGet<TranslateManager>
    {
        public Dictionary<string, string> KeyPath = new();

        private const string LanguageConfigImage = "language_config_image";
        private const string LanguageConfigSound = "language_config_sound";
        [FilePath] [ShowInInspector] private string _configPath = "";

        public void LoadBasePathFile()
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<TextAsset>(LanguageConfigImage);

                handle.WaitForCompletion();

                image = JsonConvert.DeserializeObject<Dictionary<string, string>>(handle.Result.text);
            }
            catch
            {
                Debug.LogError("没有找到图像配置文件");
            }

            try
            {
                var handle = Addressables.LoadAssetAsync<TextAsset>(LanguageConfigSound);

                handle.WaitForCompletion();

                image = JsonConvert.DeserializeObject<Dictionary<string, string>>(handle.Result.text);
            }
            catch
            {
                Debug.LogError("没有找到音频配置文件");
            }

            try
            {
                var handle = Addressables.LoadAssetAsync<TextAsset>($"language_{Language}");

                handle.WaitForCompletion();

                text = JsonConvert.DeserializeObject<Dictionary<string, string>>(handle.Result.text);
            }
            catch
            {
                Debug.LogError("没有找到对应的文本配置文件");
            }


            text ??= new Dictionary<string, string>();
            image ??= new Dictionary<string, string>();
            sound ??= new Dictionary<string, string>();

            "本地化组件加载完成".Log();
        }

        public void Save()
        {
            if (!_configPath.IsNullOrSpace())
            {
                FileHelper.WriteAllText(_configPath, JsonConvert.SerializeObject(image));
            }
        }

        public string Language = "zh-cn";

        public Dictionary<string, string> text = new Dictionary<string, string>();
        public Dictionary<string, string> image = new Dictionary<string, string>();
        public Dictionary<string, string> sound = new Dictionary<string, string>();

        public string LoadText(string id)
        {
            if (text.TryGetValue(id, out string t))
            {
                return t;
            }
            else
            {
                return null;
            }
        }

        public Sprite LoadSprite(string id)
        {
            if (image.TryGetValue(id, out string path))
            {
                path = $"Textures/{Language}/{path}/{id}";
                return Resources.Load<Sprite>(path);
            }
            else
            {
                return null;
            }
        }
    }

//awake
    public class TranslateManagerAwakeSystem : YuoSystem<TranslateManager>, IAwake
    {
        protected override void Run(TranslateManager component)
        {
            component.LoadBasePathFile();
        }
    }

    //destroy
    public class TranslateManagerDestroySystem : YuoSystem<TranslateManager>, IExitGame
    {
        protected override void Run(TranslateManager component)
        {
            component.Save();
        }
    }
}
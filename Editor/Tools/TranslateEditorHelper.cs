using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Tools;
using UnityEditor;
using UnityEngine;
using YuoTools.Extend.Helper;

namespace YuoTools.Editor.Tools
{
    public class TranslateEditorHelper : OdinEditorWindow
    {
        [MenuItem("Tools/TranslateEditorHelper")]
        private static void OpenWindow()
        {
            var window = GetWindow<TranslateEditorHelper>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 1000);
            window.Show();
        }

        [Button("生成")]
        [HorizontalGroup("1", width: 50)]
        public void Init()
        {
            //获取场景中所有的物体
            var allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            allTranslate.Clear();
            //判断物体上是否挂载了TranslateComponent组件
            foreach (var obj in allGameObjects)
            {
                if (obj.TryGetComponent<TranslateComponent>(out var translate))
                {
                    allTranslate.Add(translate.gameObject, translate.ID);
                }
            }

            Serialize();
        }

        [Button("序列化")]
        [HorizontalGroup("1", width: 50)]
        public void Serialize()
        {
             Dictionary<string, string> text = new Dictionary<string, string>();
             Dictionary<string, string> image = new Dictionary<string, string>();
             Dictionary<string, string> sound = new Dictionary<string, string>();
            foreach (var tran in allTranslate)
            {
              var com =  tran.Key.GetComponent<TranslateComponent>();
              switch (com.TranslateType)
              {
                  case TranslateType.None:
                      break;
                  case TranslateType.Text or TranslateType.TextMeshProUGUI :
                      text.Add(com.ID, "");
                      break;
                  case TranslateType.Image:
                      image.Add(com.ID, "");
                      break;
                  case TranslateType.Sound:
                      break;
                  case TranslateType.Video:
                      break;
                  default:
                      throw new ArgumentOutOfRangeException();
              }
            }

            copyPanel = "";
            copyPanel += ConvertJsonString(JsonConvert.SerializeObject(image));
            copyPanel += ConvertJsonString(JsonConvert.SerializeObject(text));
        }

        [Button("加载")]
        [HorizontalGroup("1", width: 50)]
        public void LoadConfig()
        {
            config = FileHelper.ReadAllText(ConfigPath);
        }

        [Button("保存")]
        [HorizontalGroup("1", width: 50)]
        public void SaveConfig()
        {
            FileHelper.WriteAllText(ConfigPath, config);
        }

        private const string ConfigPath = "Assets/AddressableResources/language/language_image.json";

        [Button]
        [HorizontalGroup("1", width: 50)]
        public void CreateFolder()
        {
            LoadConfig();
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(config);
            List<string> paths = new List<string>();
            foreach (var item in data.Values)
            {
                //add no repeat
                if (!paths.Contains(item))
                {
                    paths.Add(item);
                }
            }

            mCreateFolder(paths, Language);
            mCreateFolder(paths, "en");
        }

        void mCreateFolder(List<string> oldPaths, string lan)
        {
            //copy list
            var newPaths = new List<string>(oldPaths);
            for (var index = 0; index < newPaths.Count; index++)
            {
                newPaths[index] = $"Assets/Resources/Textures/{lan}/{newPaths[index]}";
                if (!newPaths[index].EndsWith("/"))
                {
                    newPaths[index] += "/";
                }
                Debug.Log(newPaths[index]);
            }

            foreach (var path in newPaths)
            {
                FileHelper.CheckOrCreateDirectoryPath(path);
            }
        }

        public string Language = "zh-cn";

        string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }

            return str;
        }

        [HorizontalGroup("1")]
        public Dictionary<GameObject, string> allTranslate = new Dictionary<GameObject, string>();

        [HorizontalGroup()] [MultiLineProperty(Lines = 999)] [LabelWidth(60)]
        public string copyPanel = "";

        [HorizontalGroup()] [MultiLineProperty(Lines = 999)] [LabelWidth(60)]
        public string config = "";
    }
}
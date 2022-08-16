using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YuoTools.Extend.UI
{
    public static class SpawnUICode
    {
        static string UITag = "C_";
        static string GeneralUITag = "G_";
        static string ChildUITag = "D_";

        public static void SpawnCode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string UIName = gameObject.name;
            string strDlgName = $"View_{UIName}Component";

            string strFilePath = Application.dataPath + "/YuoUI/View";

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine();
            //引入命名空间
            foreach (var space in ComponentNameSpace)
            {
                strBuilder.AppendLine(space);
            }

            //设置命名空间
            strBuilder.AppendLine("namespace YuoTools.UI\n{");

            strBuilder.AppendLine(
                $"\tpublic static partial class ViewType \n\t{{ \n\t\tpublic const string {UIName} = \"{UIName}\";\n\t}}");
            strBuilder.AppendLine($"\tpublic partial class {strDlgName} : UIComponent \n\t{{");
            List<string> types = new List<string>();
            List<string> typenames = new List<string>();
            //获取所有组件
            List<Transform> trans = FindAll(gameObject.transform);

            //获取通用组件
            List<Transform> generalTemps = new List<Transform>();
            List<Transform> generals = new List<Transform>();

            //获取子组件
            List<Transform> childTemps = new List<Transform>();
            List<Transform> childs = new List<Transform>();

            foreach (var item in trans)
            {
                if (item.name.StartsWith(GeneralUITag))
                {
                    generals.Add(item);
                    generalTemps.AddRange(FindAll(item));
                }

                if (item.name.StartsWith(ChildUITag))
                {
                    childs.Add(item);
                    childTemps.AddRange(FindAll(item));
                }
            }

            //将通用组件从trans中移除
            foreach (var item in generalTemps)
            {
                trans.Remove(item);
            }

            //将子组件移除
            foreach (var item in childTemps)
            {
                trans.Remove(item);
            }

            foreach (var item in trans)
            {
                if (item == gameObject.transform) continue;

                string tag = "C_";

                if (item.name.StartsWith(tag))
                {
                    string name = item.name.Replace(tag, "");
                    var _types = GetTypes(item);
                    if (_types.Count == 0)
                    {
                        _types.Add("RectTransform");
                    }

                    foreach (var type in _types)
                    {
                        typenames.Add($"{type}_{name}");
                        types.Add($"{type}");
                        string get = "{\n\t\t\tget\n\t\t\t{\n\t\t\t\tif (" + $"m{type}_{name}" +
                                     $" == null)\n\t\t\t\t\tm{type}_{name} = rectTransform.Find(\"" +
                                     GetRelativePath(item, gameObject.transform) +
                                     $"\").GetComponent<{type}>();\n\t\t\t\treturn " + $"m{type}_{name}" +
                                     ";\n\t\t\t}\n\t\t}";

                        strBuilder.AppendLine($"\n\t\tprivate {type} m{type}_{name};");

                        strBuilder.AppendLine($"\n\t\tpublic {type} {type}_{name}\n\t\t{get}");
                    }
                }
            }

            foreach (var general in generals)
            {
                Debug.Log(general);
                string tag = "G_";
                SpawnGeneralUICode(general.gameObject);
                string name = general.name.Replace(tag, "");
                string type = $"View_{name}Component";
                string get = "{\n\t\t\tget" +
                             "\n\t\t\t{" +
                             "\n\t\t\t\tif (" + $"mGeneral_{name} == null)" +
                             "\n\t\t\t\t{" +
                             $"\n\t\t\t\t\tmGeneral_{name} = Entity.AddChild<{type}>();" +
                             $"\n\t\t\t\t\tmGeneral_{name}.rectTransform = rectTransform.Find(" +
                             $"\"{GetRelativePath(general, gameObject.transform)}\"" + ") as RectTransform;" +
                             "\n\t\t\t\t}" +
                             $"\n\t\t\t\treturn " + $"mGeneral_{name}" + ";\n\t\t\t}\n\t\t}";

                strBuilder.AppendLine($"\n\t\tprivate {type} mGeneral_{name};");

                strBuilder.AppendLine($"\n\t\tpublic {type} General_{name}\n\t\t{get}");
            }

            foreach (var child in childs)
            {
                Debug.Log(child);
                string tag = "D_";
                SpawnChildUICode(child.gameObject);
                string name = child.name.Replace(tag, "");
                string type = $"View_{name}Component";
                string get = "{\n\t\t\tget" +
                             "\n\t\t\t{" +
                             "\n\t\t\t\tif (" + $"mChild_{name} == null)" +
                             "\n\t\t\t\t{" +
                             $"\n\t\t\t\t\tmChild_{name} = Entity.AddChild<{type}>();" +
                             $"\n\t\t\t\t\tmChild_{name}.rectTransform = rectTransform.Find(" +
                             $"\"{GetRelativePath(child, gameObject.transform)}\"" + ") as RectTransform;" +
                             "\n\t\t\t\t}" +
                             $"\n\t\t\t\treturn " + $"mChild_{name}" + ";\n\t\t\t}\n\t\t}";

                strBuilder.AppendLine($"\n\t\tprivate {type} mChild_{name};");

                strBuilder.AppendLine($"\n\t\tpublic {type} Child_{name}\n\t\t{get}");
            }

            strBuilder.AppendLine("\t}\r}");
            sw.Write(strBuilder.ToString());
            sw.Close();

            SpawnSystemCode(UIName);
            
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static void SpawnGeneralUICode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string UIName = gameObject.name.Replace(GeneralUITag, "");

            string strDlgName = $"View_{UIName}Component";

            string strFilePath = Application.dataPath + "/YuoUI/View/General";

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine();
            //引入命名空间
            foreach (var space in ComponentNameSpace)
            {
                strBuilder.AppendLine(space);
            }

            //设置命名空间
            strBuilder.AppendLine("namespace YuoTools.UI\n{");
            strBuilder.AppendLine($"\tpublic partial class {strDlgName} : UIComponent \n\t{{");
            List<string> types = new List<string>();
            List<string> typenames = new List<string>();
            foreach (var item in FindAll(gameObject.transform))
            {
                if (item == gameObject.transform) continue;


                if (item.name.StartsWith(UITag))
                {
                    string name = item.name.Replace(UITag, "");
                    foreach (var type in GetTypes(item))
                    {
                        typenames.Add($"{type}_{name}");
                        types.Add($"{type}");
                        string get = "{\n\t\t\tget\n\t\t\t{\n\t\t\t\tif (" + $"m{type}_{name}" +
                                     $" == null)\n\t\t\t\t\tm{type}_{name} = rectTransform.Find(\"" +
                                     GetRelativePath(item, gameObject.transform) +
                                     $"\").GetComponent<{type}>();\n\t\t\t\treturn " + $"m{type}_{name}" +
                                     ";\n\t\t\t}\n\t\t}";

                        strBuilder.AppendLine($"\n\t\tprivate {type} m{type}_{name};");

                        strBuilder.AppendLine($"\n\t\tprivate static {type} s{type}_{name};");

                        strBuilder.AppendLine($"\n\t\tpublic {type} {type}_{name}\n\t\t{get}");
                    }
                }
            }

            strBuilder.AppendLine($"\n\t\tpublic void UpdateView()");
            strBuilder.AppendLine("\t\t{");
            for (int i = 0; i < types.Count; i++)
            {
                if (GetValue(types[i]) == null)
                    continue;
                strBuilder.AppendLine($"\t\t\tif ({typenames[i]} != null)");
                strBuilder.AppendLine("\t\t\t{");
                strBuilder.AppendLine($"\t\t\t\tif (s{typenames[i]} != null)");
                strBuilder.AppendLine("\t\t\t\t{");
                foreach (var item in GetValue(types[i]))
                {
                    strBuilder.AppendLine($"\t\t\t\t\t{typenames[i]}{item} = s{typenames[i]}{item};");
                }

                strBuilder.AppendLine("\t\t\t\t}");
                strBuilder.AppendLine($"\t\t\t\ts{typenames[i]} = {typenames[i]};");
                strBuilder.AppendLine("\t\t\t}");
            }

            strBuilder.AppendLine("\t\t}");
            strBuilder.AppendLine("\t}\r}");
            sw.Write(strBuilder.ToString());
            sw.Close();
            SpawnSystemCode(UIName, general: true);
        }

        private static void SpawnChildUICode(GameObject gameObject)
        {
            if (null == gameObject) return;

            string UIName = gameObject.name.Replace(ChildUITag, "");

            string strDlgName = $"View_{UIName}Component";

            string strFilePath = Application.dataPath + "/YuoUI/View/Child";

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine();
            //引入命名空间
            foreach (var space in ComponentNameSpace)
            {
                strBuilder.AppendLine(space);
            }

            //设置命名空间
            strBuilder.AppendLine("namespace YuoTools.UI\n{");
            strBuilder.AppendLine($"\tpublic partial class {strDlgName} : UIComponent \n\t{{");
            List<string> types = new List<string>();
            List<string> typenames = new List<string>();
            foreach (var item in FindAll(gameObject.transform))
            {
                if (item == gameObject.transform) continue;


                if (item.name.StartsWith(UITag))
                {
                    string name = item.name.Replace(UITag, "");
                    foreach (var type in GetTypes(item))
                    {
                        typenames.Add($"{type}_{name}");
                        types.Add($"{type}");
                        string get = "{\n\t\t\tget\n\t\t\t{\n\t\t\t\tif (" + $"m{type}_{name}" +
                                     $" == null)\n\t\t\t\t\tm{type}_{name} = rectTransform.Find(\"" +
                                     GetRelativePath(item, gameObject.transform) +
                                     $"\").GetComponent<{type}>();\n\t\t\t\treturn " + $"m{type}_{name}" +
                                     ";\n\t\t\t}\n\t\t}";

                        strBuilder.AppendLine($"\n\t\tprivate {type} m{type}_{name};");

                        strBuilder.AppendLine($"\n\t\tpublic {type} {type}_{name}\n\t\t{get}");
                    }
                }
            }

            strBuilder.AppendLine("\t}\r}");
            sw.Write(strBuilder.ToString());
            sw.Close();
            SpawnSystemCode(UIName);
        }

        public static string[] GetValue(string type)
        {
            Dictionary<string, string[]> dic = new Dictionary<string, string[]>();
            dic.Add("Text", new string[] { ".text" });
            dic.Add("Image", new string[] { ".sprite", ".color" });
            dic.Add("Slider", new string[] { ".value" });
            dic.Add("Toggle", new string[] { ".isOn" });
            dic.Add("InputField", new string[] { ".text" });
            return dic.ContainsKey(type) ? dic[type] : null;
        }

        public static void SpawnSystemCode(string name, bool general = false)
        {
            string strFilePath = Application.dataPath + "/YuoUI/System";

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            string strDlgName = $"View_{name}System";
            strFilePath = strFilePath + "/" + strDlgName + ".cs";

            if (System.IO.File.Exists(strFilePath))
            {
                Debug.LogWarning($"{strDlgName}已存在");
                return;
            }

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();
            foreach (var space in SystemNameSpace)
            {
                strBuilder.AppendLine(space);
            }

            //设置命名空间
            strBuilder.AppendLine("namespace YuoTools.UI{");
            if (!general)
            {
                strBuilder.AppendLine(
                    $"public class View_{name}CreateSystem :YuoSystem<View_{name}Component>, IUICreate {{");
                strBuilder.AppendLine($"protected override void Run(View_{name}Component view) {{");
                strBuilder.AppendLine("            //关闭窗口的事件注册,名字不同请自行更");
                strBuilder.AppendLine(
                    "view.Button_Close.SetBtnClick(() => World.Main.GetComponent<UIManagerComponent>().Close(view.ViewName));");
                strBuilder.AppendLine(
                    " view.Button_Mask.SetBtnClick(() => World.Main.GetComponent<UIManagerComponent>().Close(view.ViewName));");

                strBuilder.AppendLine("}");
                strBuilder.AppendLine("}");

                strBuilder.AppendLine(
                    $"public class View_{name}OpenSystem :YuoSystem<View_{name}Component>, IUIOpen {{");
                strBuilder.AppendLine($"protected override void Run(View_{name}Component view) {{");
                strBuilder.AppendLine("}");
                strBuilder.AppendLine("}");

                strBuilder.AppendLine(
                    $"public class View_{name}CloseSystem :YuoSystem<View_{name}Component>, IUIClose {{");
                strBuilder.AppendLine($"protected override void Run(View_{name}Component view) {{");
                strBuilder.AppendLine("}");

                strBuilder.AppendLine("}");
                strBuilder.AppendLine("}");
            }
            else
            {
                strBuilder.AppendLine(
                    $"public class View_{name}ActiveSystem :YuoSystem<View_{name}Component>, IUIActive {{");
                strBuilder.AppendLine($"protected override void Run(View_{name}Component view) {{");
                strBuilder.AppendLine("view.UpdateView();");
                strBuilder.AppendLine("}");
                strBuilder.AppendLine("}");
                strBuilder.AppendLine("}");
            }

            sw.Write(strBuilder.ToString());
            sw.Close();
        }

        private static readonly Type[] SpawnType =
        {
            typeof(Button),
            typeof(Image),
            typeof(RawImage),
            typeof(Text),
            typeof(TextMeshProUGUI),
            typeof(Toggle),
            typeof(ToggleGroup),
            typeof(Dropdown),
            typeof(InputField),
            typeof(Slider),
            typeof(ScrollRect),
            typeof(YuoDropDown),
            typeof(ButtonSwitch)
        };

        private static readonly Dictionary<string, string> RemoveType = new()
        {
            { "Button", "Image" },
            { "Dropdown", "Image" },
            { "InputField", "Image" },
            { "Toggle", "Image" },
            { "ToggleGroup", "Image" },
            { "Slider", "Image" },
            { "ScrollRect", "Image" },

            { "YuoDropDown", "Button" },
            { "ButtonSwitch", "Button" },
        };

        /// <summary>
        /// 组件的命名空间
        /// </summary>
        private static readonly List<string> ComponentNameSpace = new()
        {
            "using UnityEngine;",
            "using System.Collections;",
            "using System.Collections.Generic;",
            "using TMPro;",
            "using UnityEngine.UI;",
        };

        /// <summary>
        /// 系统的命名空间
        /// </summary>
        private static readonly List<string> SystemNameSpace = new()
        {
            "using YuoTools.ECS;",
            "using YuoTools.Extend.Helper;",
            "using YuoTools.Main.Ecs;",
        };

        private static List<string> GetTypes(Transform transform)
        {
            List<string> ts = new List<string>();
            foreach (var item in SpawnType)
            {
                Get(item);
            }

            foreach (var item in RemoveType)
            {
                if (ts.Contains(item.Key)) ts.Remove(item.Value);
            }

            return ts;

            void Get(Type type)
            {
                var t = transform.GetComponent(type);
                if (t != null)
                {
                    ts.Add(t.GetType().Name);
                }
            }
        }

        private static string GetRelativePath(Transform child, Transform parent)
        {
            if (child == parent)
            {
                return parent.name;
            }

            var path = child.name;
            Transform nowParent = child.parent;
            while (nowParent != parent && nowParent != null)
            {
                path = nowParent.name + "/" + path;
                nowParent = nowParent.parent;
            }

            return path;
        }

        public static List<T> GetAllSelectComponent<T>() where T : Component
        {
            List<T> list = new List<T>();

#if UNITY_EDITOR
            foreach (var item in Selection.transforms)
            {
                foreach (var item_1 in FindAll(item))
                {
                    T t = item_1.GetComponent<T>();
                    if (t != null)
                    {
                        list.Add(t);
                    }
                }
            }
#endif
            return list;
        }

        private static List<Transform> FindAll(Transform transform)
        {
            List<Transform> list = new List<Transform>();
            list.Add(transform);
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).childCount > 0)
                {
                    list.AddRange(FindAll(transform.GetChild(i)));
                }
                else
                {
                    list.Add(transform.GetChild(i));
                }
            }

            return list;
        }
    }
}
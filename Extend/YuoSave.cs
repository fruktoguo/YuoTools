using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ET;
using Newtonsoft.Json;
using Sirenix.Utilities;
using UnityEngine;
using Sirenix.OdinInspector;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.ECS
{
    [Serializable]
    public partial class SaveManagerComponent : YuoComponent
    {
        [HorizontalGroup] public List<string> saveTypeKeys = new();
        [HorizontalGroup] public List<Type> SaveTypes = new();
        [HorizontalGroup] public List<YuoSaveAttribute> SaveTypeInfo = new();
        public string savePath = "";

        //--------逻辑--------

        [SerializeField] Dictionary<string, GamData> _allData = new();

        public GamData data = new();

        Type GetType(string key)
        {
            if (saveTypeKeys.Contains(key))
            {
                return SaveTypes[saveTypeKeys.IndexOf(key)];
            }

            return null;
        }

        string GetKey(Type type)
        {
            if (SaveTypes.Contains(type))
            {
                return saveTypeKeys[SaveTypes.IndexOf(type)];
            }

            return type.FullName;
        }

        void Add(string key, Type type)
        {
            if (!saveTypeKeys.Contains(key))
            {
                saveTypeKeys.Add(key);
                SaveTypes.Add(type);
            }
        }

        void Remove(string key)
        {
            if (saveTypeKeys.Contains(key))
            {
                SaveTypes.RemoveAt(saveTypeKeys.IndexOf(key));
                saveTypeKeys.Remove(key);
            }
        }

        JsonSerializerSettings settings = new();

        public void Init()
        {
            //设置日期格式
            //settings.DateFormatString = "yyyy-MM-dd";
            //忽略空值
            settings.NullValueHandling = NullValueHandling.Ignore;
            //缩进
            //settings.Formatting = Formatting.Indented;
            "开始初始化保存组件".Log();

            foreach (var type in World.Instance.GetAllComponents().Keys)
            {
                //注册组件
                var save = type.GetAttribute<YuoSaveAttribute>();
                if (save != null)
                {
                    if (save.haveName)
                    {
                        Add(save.SaveName, type);
                    }
                    else
                    {
                        Add(type.FullName, type);
                    }

                    SaveTypeInfo.Add(save);
                }
            }
        }

        public void SaveSceneData(int id)
        {
            if (!_allData.ContainsKey(ECS.SaveGroup.Save)) _allData.Add(ECS.SaveGroup.Save, new GamData());
            data = _allData[$"Save/{ECS.SaveGroup.Save}_{id}"];
            DicList<long, YuoComponent> entities = new();
            foreach (var entity in World.Scene.GetAllChild())
            {
                foreach (var component in entity.Components)
                {
                    if (SaveTypes.Contains(component.Key))
                    {
                        entities.AddItem(component.Value.Entity.EntityData.Id, component.Value);
                    }
                }
            }

            SaveEntities(entities);
            SaveFile();
        }

        public int SaveCount
        {
            get
            {
                int count = 0;
                foreach (var dir in FileHelper.GetChildDirectory(savePath))
                {
                    if (dir.StartsWith(savePath + "\\Save_"))
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public async ETTask<FileInfo> GetSaveInfo(int id)
        {
            var path = $"{savePath}/Save_{id}/Save.json";
            await FileHelper.CheckFilePathOrCreate(path);
            return await FileHelper.GetFileInfo(path);
        }

        public async ETTask<Texture2D> GetSaveImage(int id)
        {
            var path = $"{savePath}/Save_{id}/preview.png";
            if (FileHelper.CheckFilePath(path))
            {
                return await TextureHelper.LoadTexture(path);
            }

            return null;
        }

        public void SaveGroup(string groupName)
        {
            if (!_allData.ContainsKey(groupName)) _allData.Add(groupName, new GamData());
            data = _allData[groupName];
            DicList<long, YuoComponent> entities = new();
            foreach (var components in World.Instance.GetAllComponents())
            {
                if (SaveTypes.Contains(components.Key) &&
                    SaveTypeInfo[SaveTypes.IndexOf(components.Key)].GroupName == groupName)
                {
                    // $"保存类型 {components.Key.Name}".Log();
                    foreach (var component in components.Value)
                    {
                        entities.AddItem(component.Entity.EntityData.Id, component);
                    }
                }
            }

            SaveEntities(entities);
            SaveFile(groupName);
        }

        void SaveEntities(DicList<long, YuoComponent> entities)
        {
            foreach (var entity in entities)
            {
                var id = entity.Key;
                Dictionary<string, string> components;
                if (!data.entities.ContainsKey(entity.Key))
                {
                    components = new Dictionary<string, string>();
                    data.entities.Add(id, components);
                }
                else
                {
                    components = data.entities[id];
                }

                //保存前调用
                World.Instance.RunSystemOfTag<IOnSave>(World.Instance.GetEntity(id));

                foreach (YuoComponent component in entity.Value)
                {
                    string typeName = GetKey(component.Type);
                    if (!components.ContainsKey(typeName)) components.Add(typeName, null);

                    components[typeName] = SerializeComponent(component, component.Type);
                }
            }
        }

        void SaveFile(string groupName = ECS.SaveGroup.Save)
        {
            //保存文本文件到路径
            if (_allData.TryGetValue(groupName, out GamData gamData))
            {
                string text = JsonConvert.SerializeObject(gamData);
                //判断文件夹是否存在，不存在则创建
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                //保存文件
                File.WriteAllText(savePath + $"/{groupName}.json", text);
                $"{groupName}--保存成功--{text}".Log();
            }
        }

        async Task SaveFile(GamData gamData, int id)
        {
            //保存文本文件到路径
            string text = JsonConvert.SerializeObject(gamData);
            string path = savePath + $"/Save_{id}/Save.json";
            await FileHelper.CheckFilePathOrCreate(path);
            //保存文件
            await File.WriteAllTextAsync(path, text);
        }

        public void Load(string groupName = ECS.SaveGroup.Save)
        {
            LoadFile(groupName);
            if (_allData.ContainsKey(groupName))
            {
                data = _allData[groupName];
                LoadSceneData();
            }
        }

        public GamData LoadSceneData(int id)
        {
            string group = $"{ECS.SaveGroup.Save}_{id}/Save";
            LoadFile(group);
            if (_allData.ContainsKey(group))
            {
                return _allData[group];
            }

            return null;
        }

        void LoadFile(string groupName = ECS.SaveGroup.Save)
        {
            var file = savePath + $"/{groupName}.json";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
            }

            var text = File.ReadAllText(file);

            if (!_allData.ContainsKey(groupName))
                _allData.Add(groupName, null);
            _allData[groupName] = JsonConvert.DeserializeObject<GamData>(text) ?? new GamData();
        }

        string SerializeComponent(YuoComponent component, Type type)
        {
            return SaveTypeInfo[SaveTypes.IndexOf(type)].jsonType switch
            {
                SerializeType.JsonUtility => JsonUtility.ToJson(component),
                SerializeType.NewtonsoftJson => JsonConvert.SerializeObject(component, settings),
                _ => JsonUtility.ToJson(component)
            };
        }

        YuoComponent DeserializeComponent(string componentData, Type type)
        {
            return SaveTypeInfo[SaveTypes.IndexOf(type)].jsonType switch
            {
                SerializeType.JsonUtility =>
                    JsonUtility.FromJson(componentData, type) as YuoComponent,
                SerializeType.NewtonsoftJson =>
                    JsonConvert.DeserializeObject(componentData, type) as YuoComponent,
                _ => JsonUtility.FromJson(componentData, type) as YuoComponent,
            };
        }

        public void LoadSceneData()
        {
            if (data == null) return;
            foreach (var entityData in data.entities)
            {
                var entity = World.Instance.GetEntity(entityData.Key);
                if (entity == null)
                {
                    entity = World.Scene.AddChild(entityData.Key);
                    List<YuoComponent> components = new();
                    foreach (var componentData in entityData.Value)
                    {
                        var type = GetType(componentData.Key);
                        YuoComponent component = DeserializeComponent(componentData.Value, type);
                        // var component = JsonUtility.FromJson(componentData.Value, type) as YuoComponent;
                        entity.SetComponent(component);
                        components.Add(component);
                    }

                    //加载后调用
                    World.Instance.RunSystemOfTag<IOnLoadCreate>(entity);
                }
                else
                {
                    foreach (var componentData in entityData.Value)
                    {
                        try
                        {
                            var type = GetType(componentData.Key);
                            YuoComponent c = DeserializeComponent(componentData.Value, type);
                            entity.SetComponent(c);
                            // World.Instance.RunSystemOfTag<IOnLoad>(c);
                        }
                        catch (Exception e)
                        {
                            $"序列化错误 --- {e}".Log();
                        }
                    }

                    World.Instance.RunSystemOfTag<IOnLoad>(entity);
                }
            }
        }

        public async Task CreateData(int id)
        {
            GamData newData = new();
            newData.OtherDatas.Add(OtherDataKey.Days, "1");
            newData.OtherDatas.Add(OtherDataKey.SaveTime, "1");
            await SaveFile(newData, id);
        }

        public string GetFilePath(string path) => $"{savePath}/{path}";

        public class OtherDataKey
        {
            public const string Days = "天数";
            public const string SaveTime = "保存时间";
        }

        [Serializable]
        public class GamData
        {
            public Dictionary<string, string> OtherDatas = new();
            public Dictionary<long, Dictionary<string, string>> entities = new();
        }
    }

    public class SaveManagerSystem : YuoSystem<SaveManagerComponent>, IAwake
    {
        protected override void Run(SaveManagerComponent component)
        {
            component.Init();
            component.savePath = $"{Application.persistentDataPath}/GameSave".Log();

            // foreach (var dir in await FileHelper.GetAllDirectory(Application.persistentDataPath))
            // {
            //     Debug.Log(dir);
            // }


            World.Main.GetComponent<YuoInputComponent>().Add(new("Save")
            {
                key = KeyCode.Alpha1, OnDown = () =>
                {
                    component.SaveGroup(SaveGroup.Config);
                    "保存完成".Log();
                }
            });

            World.Main.GetComponent<YuoInputComponent>().Add(new("Load")
            {
                key = KeyCode.Alpha2, OnDown = () =>
                {
                    component.Load(SaveGroup.Config);
                    "读档完成".Log();
                }
            });
            component.Load(SaveGroup.Config);
        }
    }

    public class SaveManagerExitGameSystem : YuoSystem<SaveManagerComponent>, IExitGame
    {
        protected override void Run(SaveManagerComponent component)
        {
            component.SaveGroup(SaveGroup.Config);
        }
    }

    public static class SaveGroup
    {
        /// <summary>
        /// 游戏存档
        /// </summary>
        public const string Save = "Save";

        /// <summary>
        /// 游戏设置
        /// </summary>
        public const string Setting = "Setting";

        /// <summary>
        /// 配置文件
        /// </summary>
        public const string Config = "Config";

        /// <summary>
        /// 语言文件
        /// </summary>
        public const string Language = "Language";
    }

    /// <summary>
    /// 自动保存组件信息,需要继承YuoComponent才会被调用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class YuoSaveAttribute : Attribute
    {
        public readonly string SaveName;
        public readonly string GroupName = SaveGroup.Save;
        public readonly bool haveName;
        public readonly SerializeType jsonType = SerializeType.JsonUtility;


        /// <summary>
        /// 自动保存这个组件
        /// </summary>
        /// <param name="saveName">覆盖类型名字</param>
        /// <param name="jsonType"></param>
        public YuoSaveAttribute(string saveName, string GroupName = SaveGroup.Save,
            SerializeType jsonType = SerializeType.JsonUtility)
        {
            this.SaveName = saveName;
            this.GroupName = GroupName;
            this.haveName = true;
            this.jsonType = jsonType;
        }

        public YuoSaveAttribute(SerializeType jsonType) : this()
        {
            this.jsonType = jsonType;
        }

        public YuoSaveAttribute(string GroupName, SerializeType jsonType) : this()
        {
            this.GroupName = GroupName;
            this.jsonType = jsonType;
        }

        public YuoSaveAttribute()
        {
            haveName = false;
        }
    }

    public enum SerializeType
    {
        JsonUtility = 0,
        NewtonsoftJson = 1,
        SerializationUtility = 2,
    }

    /// <summary>
    /// 加载后调用
    /// </summary>
    public interface IOnLoad : ISystemTag
    {
    }

    /// <summary>
    /// 加载后添加实体时调用
    /// </summary>
    public interface IOnLoadCreate : ISystemTag
    {
    }

    /// <summary>
    /// 保存之前调用
    /// </summary>
    public interface IOnSave : ISystemTag
    {
    }
}
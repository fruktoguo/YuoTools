using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using YuoTools.ECS;

namespace YuoTools.Main.Ecs
{
    [DefaultExecutionOrder(int.MinValue)]
    public class World : MonoBehaviour
        // public class World : SerializedMonoBehaviour
    {
        public static World Instance { get; private set; }

        public static YuoEntity Main { get; private set; }

        private List<SystemBase> _allSystem;
        public Dictionary<long, YuoEntity> Entities { get; private set; }

        private MultiMap<Type, YuoComponent> _components;

        private MultiMap<Type, SystemBase> _systemsOfTag;

        private MultiMap<Type, SystemBase> _systemsOfComponent;

        public static YuoEntity Scene => Instance.AllScenes[0];

        public readonly List<YuoEntity> AllScenes = new();

        private readonly List<YuoEntity> _entityTrash = new();

        private readonly List<YuoComponent> _componentsTrash = new();

        public void Awake()
        {
            _allSystem = new();
            if (Instance != null)
            {
                _dontInit = true;
                Destroy(gameObject);
                return;
            }

            Entities = new();
            _components = new();
            _systemsOfTag = new();
            _systemsOfComponent = new();

            Instance = this;

            DontDestroyOnLoad(gameObject);

            //系统初始化必须放在所有初始化之前
            Initialization();

            //基本核心组件挂载的实体
            Main = new YuoEntity(0);

            Main.EntityName = "核心组件";

            AddComponent(Main, IDGenerate.Instance);

            //添加一个场景实体
            var scene = new YuoEntity(IDGenerate.GetID(IDGenerate.IDType.Scene, 0));

            scene.EntityName = "默认场景";
            AllScenes.Add(scene);

            scene.AddComponent<SceneComponent>();
        }

        private bool _dontInit;

        private bool _worldIsDestroy;

        public void OnDestroy()
        {
            if (_dontInit) return;

            _worldIsDestroy = true;
            RunSystemForAllEntity<IExitGame>();

            foreach (var entity in Entities.Values.ToList())
            {
                entity?.Dispose();
            }

            //清理一下
            Entities.Clear();
            _components.Clear();
            _systemsOfTag.Clear();
            _systemsOfComponent.Clear();
            _allSystem.Clear();

            //清除所有Scene
            AllScenes.Clear();
            Instance = null;
            //手动GC
            GC.Collect();
            "World Destroy".Log();
        }

        private void Update()
        {
            RunSystemForAllEntity(SystemType.Update);
            ClearTrash();
            foreach (var component in _startSystems)
            {
                RunSystem<IStart>(component);
            }

            _startSystems.Clear();
        }

        private void FixedUpdate()
        {
            RunSystemForAllEntity(SystemType.FixedUpdate);
            ClearTrash();
        }

        void ClearTrash()
        {
            if (_componentsTrash.Count == 0 && _entityTrash.Count == 0) return;

            foreach (var yuoComponent in _componentsTrash)
            {
                yuoComponent.Dispose();
            }

            foreach (var entity in _entityTrash)
            {
                entity.Dispose();
            }

            _entityTrash.Clear();
        }

        public static void DestroyEntity(YuoEntity entity)
        {
            Instance._entityTrash.Add(entity);
        }

        public static void DestroyComponent(YuoComponent component)
        {
            Instance._componentsTrash.Add(component);
        }

        /// <summary>
        /// 初始化实体
        /// </summary>
        /// <param name="entity"></param>
        public void RegisterEntity(YuoEntity entity)
        {
            if (!Entities.ContainsKey(entity.EntityData.Id))
            {
                Entities.Add(entity.EntityData.Id, entity);
            }
            else
                $"实体ID重复，请检查：{entity.EntityData.Id}".LogError();
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entity"></param>
        public void UnRegisterEntity(YuoEntity entity)
        {
            if (Entities.ContainsKey(entity.EntityData.Id))
            {
                Entities.Remove(entity.EntityData.Id);
            }
        }

        public void RemoveComponent(YuoEntity entity, YuoComponent componet)
        {
            _components[componet.Type].Remove(componet);
            //World销毁时,不调用Destroy,只调用ExitGame
            if (!_worldIsDestroy)
            {
                foreach (var system in _systemsOfComponent[componet.Type])
                {
                    if (system is IDestroy)
                    {
                        system.RunType = SystemType.Destroy;
                        system.m_Run(entity);
                    }

                    system.RemoveComponent(entity);
                }
            }

            componet.Dispose();
        }

        public void SetComponent(YuoComponent componet1, YuoComponent componet2)
        {
            YuoEntity entity = componet1.Entity;
            _components[componet1.Type].Remove(componet1);
            _components[componet1.Type].Add(componet2);

            foreach (var system in _systemsOfComponent[componet1.Type])
            {
                system.SetComponent(entity, componet1.Type, componet2);
            }

            RunSystemOfTag<ISwitchComponent>(componet2);
        }

        /// <summary>
        /// 将一个组件手动添加到实体上
        /// </summary>
        public void AddComponent(YuoEntity entity, YuoComponent componet)
        {
            Type type = componet.Type;
            if (!_components.ContainsKey(type))
            {
                _components.Add(type, new List<YuoComponent>());
            }

            if (!_components[type].Contains(componet))
            {
                _components[type].Add(componet);
                foreach (var system in _systemsOfComponent[type])
                {
                    if (system.AddComponent(entity))
                    {
                        if (system is IAwake)
                        {
                            system.RunType = SystemType.Awake;
                            system.m_Run(system.EntityCount - 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将一个组件手动添加到实体上
        /// </summary>
        public void AddComponent(YuoEntity entity, YuoComponent componet, Type type)
        {
            if (!_components.ContainsKey(type))
            {
                _components.Add(type, new List<YuoComponent>());
            }

            if (!_components[type].Contains(componet))
            {
                _components[type].Add(componet);
                //Run System
                foreach (var system in _systemsOfComponent[type])
                {
                    if (system.AddComponent(entity))
                    {
                        if (system is IAwake)
                        {
                            system.RunType = SystemType.Awake;
                            system.m_Run(system.EntityCount - 1);
                        }
                    }
                }

                _startSystems.Add(componet);
            }
        }

        private readonly List<YuoComponent> _startSystems = new();

        /// <summary>
        ///  初始化系统
        /// </summary>
        /// <param name="system"></param>
        /// <param name="type"></param>
        public void RegisterSystem(SystemBase system, Type type)
        {
            if (!_systemsOfComponent.ContainsKey(type))
            {
                _systemsOfComponent.Add(type, new List<SystemBase>());
            }

            if (!_systemsOfComponent[type].Contains(system)) _systemsOfComponent[type].Add(system);
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public YuoEntity GetEntity(long instanceId)
        {
            Entities.TryGetValue(instanceId, out YuoEntity entity);
            return entity;
        }

        public YuoEntity GetEntity(string entityName)
        {
            Entities.TryGetValue(entityName.GetHashCode(), out YuoEntity entity);
            return entity;
        }

        /// <summary>
        /// 尽量不修改父物体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parent"></param>
        public void SetParent(YuoEntity entity, YuoEntity parent)
        {
            entity.SetParent(parent);
        }

        /// <summary>
        /// 根据ID获取组件
        /// </summary>
        /// <param name="instanceId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>(long instanceId) where T : YuoComponent
        {
            return GetEntity(instanceId)?.GetComponent<T>();
        }

        public new List<YuoComponent> GetComponents<T>() where T : YuoComponent
        {
            if (_components.ContainsKey(typeof(T)))
            {
                return _components[typeof(T)];
            }

            return null;
        }

        readonly Dictionary<string, Type> _allComponentType = new();

        public Type GetComponentType(string typeName)
        {
            return _allComponentType.ContainsKey(typeName) ? _allComponentType[typeName] : null;
        }

        /// <summary>
        /// 初始化主要数据
        /// </summary>
        private void Initialization()
        {
            //获取所有程序集
            //List<Type> types = new();
            //types.AddRange(Assembly.GetCallingAssembly().GetTypes());
            // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var yuoTool = Assembly.Load("YuoTools");
            var assCs = Assembly.Load("Assembly-CSharp");
            LoadAssembly(yuoTool);
            LoadAssembly(assCs);
            SystemSort();
        }

        private static short GetOrder(Type type)
        {
            return type.GetCustomAttribute<SystemOrderAttribute>()?.Order ?? 0;
        }

        public void LoadAssembly(Assembly assembly)
        {
            LoadTypes(assembly.GetTypes());
        }

        public void LoadTypes(Type[] types)
        {
            try
            {
                List<SystemBase> systems = new();

                foreach (var type in types)
                {
                    //获取系统类型
                    var systemBase = CheckSystem<SystemBase>(type);
                    //获取组件类型
                    var yuoComponent = CheckComponent<YuoComponent>(type);

                    //检测System
                    if (systemBase != null)
                    {
                        systems.Add(systemBase);
                        systemBase.Type = type;
                    }

                    //检测Component
                    else if (yuoComponent)
                    {
                        _allComponentType.TryAdd(type.Name, type);

                        //注册组件
                        if (_components.ContainsKey(type))
                            $"类型已经存在--{type}".Log();
                        else
                        {
                            _components.Add(type, new List<YuoComponent>());
                        }
                    }
                }

                // $"初始化程序集 [{assembly.FullName.Replace(" [", "").Split(",")[0]}] 完成--已加载{systems.Count}个系统,{componentCount}个组件--共检索{types.Length}个类型"
                //     .Log();

                foreach (var system in systems)
                {
                    if (!_allSystem.Contains(system))
                    {
                        _allSystem.Add(system);
                    }
                }
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        public void SystemSort()
        {
            //从小到大排序
            _allSystem.Sort((a, b) => GetOrder(a.Type) - GetOrder(b.Type));

            _systemsOfComponent.Clear();

            _systemsOfTag.Clear();

            foreach (var system in _allSystem)
            {
                system.Clear();
                system.Init(this);
                foreach (var inter in system.Type.GetInterfaces())
                {
                    foreach (var iInter in inter.GetInterfaces())
                    {
                        //将Tag和系统进行关联
                        if (iInter == typeof(ISystemTag))
                        {
                            if (!_systemsOfTag.ContainsKey(inter))
                                _systemsOfTag.Add(inter, new List<SystemBase>());
                            _systemsOfTag[inter].Add(system);
                            //记录一下系统的Tag
                            system.systemTags.Add(inter);
                        }
                    }
                }
            }

            foreach (var systems in _systemsOfComponent.Values)
            {
                systems.Sort((a, b) => GetOrder(a.Type) - GetOrder(b.Type));
            }

            //重新注册组件系统
            foreach (var system in _allSystem)
            {
                foreach (var entity in Entities.Values)
                {
                    system.AddComponent(entity);
                }
            }
            
            "-所有系统排序完成-".Log();
        }

        public Dictionary<string, Type> GetAutoAddTo()
        {
            return _allComponentType;
        }

        /// <summary>
        ///  根据tag执行对应的系统(所有实体)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void RunSystemForAllEntity<T>() where T : ISystemTag
        {
            Type type = typeof(T);
            if (_systemsOfTag.ContainsKey(type))
            {
                foreach (var system in _systemsOfTag[type])
                {
                    system.RunType = type;
                    system.m_Run();
                }
            }
        }

        private void RunSystemForAllEntity(Type type)
        {
            if (_systemsOfTag.ContainsKey(type))
            {
                foreach (var system in _systemsOfTag[type])
                {
                    system.RunType = type;
                    system.m_Run();
                }
            }
        }

        /// <summary>
        ///  根据tag执行对应的系统(指定实体身上的指定组件)
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        private void RunSystemOfTag<T>(YuoComponent component) where T : ISystemTag
        {
            Type type = typeof(T);
            if (GetSystemOfComponent(component.Type, out var systems))
            {
                foreach (var system in systems)
                {
                    if (system is T)
                    {
                        system.RunType = type;
                        system.m_Run(component.Entity);
                    }
                }
            }
        }

        /// <summary>
        ///  根据tag执行对应的系统(指定实体身上的所有组件)
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        private void RunSystemOfTag<T>(YuoEntity entity) where T : ISystemTag
        {
            Type type = typeof(T);
            if (GetSystemOfTag(type, out var systems))
            {
                foreach (var system in systems)
                {
                    system.RunType = type;
                    system.m_Run(entity);
                }
            }
        }

        private void RunSystemOfTagType(Type tagType, YuoEntity entity)
        {
            if (GetSystemOfTag(tagType, out var systems))
            {
                foreach (var system in systems)
                {
                    system.RunType = tagType;
                    system.m_Run(entity);
                }
            }
        }

        private void RunSystemOfTagType(Type tagType, YuoComponent component)
        {
            if (GetSystemOfComponent(component.Type, out var systems))
            {
                foreach (var system in systems)
                {
                    if (system.systemTags.Contains(tagType))
                    {
                        system.RunType = tagType;
                        system.m_Run(component.Entity);
                    }
                }
            }
        }

        private void RunSystemOfTag<T>(List<YuoEntity> entities) where T : ISystemTag
        {
            Type type = typeof(T);
            if (GetSystemOfTag(type, out var systems))
            {
                foreach (var system in systems)
                {
                    system.RunType = type;
                    foreach (var entity in entities)
                    {
                        system.m_Run(entity);
                    }
                }
            }
        }

        /// <summary>
        /// 根据tag执行对应的system,只有和这个组件有关的system才会执行
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        public static void RunSystem<T>(YuoComponent component) where T : ISystemTag
        {
            Instance.RunSystemOfTag<T>(component);
        }

        /// <summary>
        ///  根据tag执行对应的system
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        public static void RunSystem<T>(YuoEntity entity) where T : ISystemTag
        {
            Instance.RunSystemOfTag<T>(entity);
        }

        public static void RunSystem<T>(List<YuoEntity> entities) where T : ISystemTag
        {
            Instance.RunSystemOfTag<T>(entities);
        }

        /// <summary>
        ///  根据tag类型执行对应的system
        /// </summary>
        /// <param name="tagType"></param>
        /// <param name="entity"></param>
        public static void RunSystem(Type tagType, YuoEntity entity)
        {
            Instance.RunSystemOfTagType(tagType, entity);
        }

        /// <summary>
        ///  根据tag类型执行对应的system,只有和这个组件有关的system才会执行
        /// </summary>
        /// <param name="tagType"></param>
        /// <param name="component"></param>
        public static void RunSystem(Type tagType, YuoComponent component)
        {
            Instance.RunSystemOfTagType(tagType, component);
        }

        public static void RunSystemForAll<T>() where T : ISystemTag
        {
            Instance.RunSystemForAllEntity<T>();
        }

        public static void RunSystemForAll(Type tagType)
        {
            Instance.RunSystemForAllEntity(tagType);
        }

        /// <summary>
        /// 通过Tag获取系统
        /// </summary>
        private bool GetSystemOfTag(Type type, out List<SystemBase> systems)
        {
            if (_systemsOfTag.ContainsKey(type))
            {
                systems = _systemsOfTag[type];
                return true;
            }

            systems = null;
            return false;
        }

        /// <summary>
        ///  通过组件获取系统
        /// </summary>
        private bool GetSystemOfComponent(Type type, out List<SystemBase> systems)
        {
            if (_systemsOfComponent.ContainsKey(type))
            {
                systems = _systemsOfComponent[type];
                return true;
            }

            systems = null;
            return false;
        }


        public MultiMap<Type, YuoComponent> GetAllComponents()
        {
            return _components;
        }

        public MultiMap<Type, SystemBase> GetAllSystemOfComponent => _systemsOfComponent;

        public MultiMap<Type, SystemBase> GetAllSystemOfTag => _systemsOfTag;

        public List<SystemBase> GetAllSystem => _allSystem;


        #region 反射获取组件

        static bool CheckComponent<Tp>(Type find) where Tp : class
        {
            var baseType = find.BaseType; //获取基类
            while (baseType != null) //获取所有基类
            {
                if (baseType.Name == typeof(Tp).Name) return true;
                baseType = baseType.BaseType;
            }

            return false;
        }

        private static Tp CheckSystem<Tp>(Type find) where Tp : class
        {
            var baseType = find.BaseType; //获取基类
            while (baseType != null) //获取所有基类
            {
                if (baseType.Name == typeof(Tp).Name)
                {
                    try
                    {
                        object obj = Activator.CreateInstance(find);
                        if (obj != null)
                        {
                            Tp info = obj as Tp;
                            return info;
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    break;
                }
                else
                {
                    baseType = baseType.BaseType;
                }
            }

            return null;
        }

        #endregion
    }

    public class SceneComponent : YuoComponent
    {
    }
}
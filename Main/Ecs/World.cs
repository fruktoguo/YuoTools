using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if UNITY_64
using UnityEngine;
#endif
using YuoTools.ECS;

namespace YuoTools.Main.Ecs
{
#if UNITY_64
    public class World : MonoBehaviour
#else
    public class World
#endif
    {
        public static World Instance { get; private set; }

        public static YuoEntity Main;

        private List<SystemBase> _allSystem;

        private Dictionary<long, YuoEntity> _entities;

        private MultiMap<Type, YuoComponent> components;

        private MultiMap<Type, SystemBase> systemsOfTag;

        private MultiMap<Type, SystemBase> systemsOfComponent;

        public static YuoEntity Scene => Instance.AllScenes[0];
        public readonly List<YuoEntity> AllScenes = new();

        public void Awake()
        {
            _allSystem = new();
            if (Instance != null)
            {
                _dontInit = true;
                Destroy(gameObject);
                return;
            }

            _entities = new();
            components = new();
            systemsOfTag = new();
            systemsOfComponent = new();

            Instance = this;

            DontDestroyOnLoad(gameObject);

            //系统初始化必须放在所有初始化之前
            Initialization();

            //基本核心组件挂载的实体
            Main = new YuoEntity(0);

            Main.EntityName = "核心组件";

            AddComponet(Main, IDGenerater.Instance);

            //添加一个场景实体
            var scene = new YuoEntity(IDGenerater.GetID(IDGenerater.IDType.Scene, 0));
            scene.EntityName = "默认场景";
            AllScenes.Add(scene);
            scene.AddComponent<SceneComponent>();
        }

        private bool _dontInit;

        public void OnDestroy()
        {
            if (_dontInit) return;

            RunSystemForAllEntity<IExitGame>();

            foreach (var entity in _entities.Values.ToList())
            {
                entity?.Dispose();
            }

            //清理一下
            _entities.Clear();
            components.Clear();
            systemsOfTag.Clear();
            systemsOfComponent.Clear();
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
        }

        private void FixedUpdate()
        {
            RunSystemForAllEntity(SystemType.FixedUpdate);
        }

        /// <summary>
        /// 初始化实体
        /// </summary>
        /// <param name="entity"></param>
        public void RegisterEntity(YuoEntity entity)
        {
            if (!_entities.ContainsKey(entity.EntityData.Id))
            {
                _entities.Add(entity.EntityData.Id, entity);
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
            if (_entities.ContainsKey(entity.EntityData.Id))
            {
                _entities.Remove(entity.EntityData.Id);
            }
        }

        public void RemoveComponet(YuoEntity entity, YuoComponent componet)
        {
            components[componet.Type].Remove(componet);
            foreach (var system in systemsOfComponent[componet.Type])
            {
                if (system is IDestroy)
                {
                    system.RunType = SystemType.Destroy;
                    system.m_Run(entity);
                }

                system.RemoveComponent(entity);
            }

            componet.Dispose();
        }

        public void SetComponet(YuoComponent componet1, YuoComponent componet2)
        {
            YuoEntity entity = componet1.Entity;
            components[componet1.Type].Remove(componet1);
            components[componet1.Type].Add(componet2);

            foreach (var system in systemsOfComponent[componet1.Type])
            {
                system.SetComponent(entity, componet1.Type, componet2);
            }

            RunSystemOfTag<ISwitchComponent>(componet2);
        }

        /// <summary>
        /// 将一个组件手动添加到实体上
        /// </summary>
        public void AddComponet(YuoEntity entity, YuoComponent componet)
        {
            Type type = componet.Type;
            if (!components.ContainsKey(type))
            {
                components.Add(type, new List<YuoComponent>());
            }

            if (!components[type].Contains(componet))
            {
                components[type].Add(componet);
                foreach (var system in systemsOfComponent[type])
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
        public void AddComponet(YuoEntity entity, YuoComponent componet, Type type)
        {
            if (!components.ContainsKey(type))
            {
                components.Add(type, new List<YuoComponent>());
            }

            if (!components[type].Contains(componet))
            {
                components[type].Add(componet);
                foreach (var system in systemsOfComponent[type])
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
        ///  初始化系统
        /// </summary>
        /// <param name="system"></param>
        /// <param name="type"></param>
        public void RegisteSystem(SystemBase system, Type type)
        {
            if (!systemsOfComponent.ContainsKey(type))
            {
                systemsOfComponent.Add(type, new List<SystemBase>());
            }

            if (!systemsOfComponent[type].Contains(system)) systemsOfComponent[type].Add(system);
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public YuoEntity GetEntity(long instanceId)
        {
            _entities.TryGetValue(instanceId, out YuoEntity entity);
            return entity;
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
            if (components.ContainsKey(typeof(T)))
            {
                return components[typeof(T)];
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
            //获取所有类型
            List<Type> types = new();
            //types.AddRange(Assembly.GetCallingAssembly().GetTypes());
            // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var yuoTool = Assembly.Load("YuoTools");
            var assCs = Assembly.Load("Assembly-CSharp");
            LoadAssembly(yuoTool);
            LoadAssembly(assCs);
        }

        public static short GetOrder(Type type)
        {
            return type.GetCustomAttribute<SystemOrderAttribute>()?.Order ?? 0;
        }

        public void LoadAssembly(Assembly assembly)
        {
            if (assembly == null) return;

            Type[] types = assembly.GetTypes();

            List<SystemBase> systems = new();

            int componentCount = 0;
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
                    if (components.ContainsKey(type))
                        $"类型已经存在--{type}".Log();
                    else
                    {
                        componentCount++;
                        components.Add(type, new List<YuoComponent>());
                    }
                }
            }

            ($"初始化程序集 [{assembly.FullName}] 完成--已加载{systems.Count}个系统,{componentCount}个组件").Log();

            foreach (var system in systems)
            {
                if (!_allSystem.Contains(system))
                {
                    _allSystem.Add(system);
                }
            }

            SystemSort();
        }

        void SystemSort()
        {
            _allSystem.Sort((a, b) => GetOrder(a.Type) - GetOrder(b.Type));

            systemsOfComponent.Clear();
            systemsOfTag.Clear();

            foreach (var item in _allSystem)
            {
                item.Init(this);
                foreach (var inter in item.Type.GetInterfaces())
                {
                    foreach (var iInter in inter.GetInterfaces())
                    {
                        //将Tag和系统进行关联
                        if (iInter == typeof(ISystemTag))
                        {
                            if (!systemsOfTag.ContainsKey(inter))
                                systemsOfTag.Add(inter, new List<SystemBase>());
                            systemsOfTag[inter].Add(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  根据tag执行对应的系统(所有实体)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RunSystemForAllEntity<T>() where T : ISystemTag
        {
            Type type = typeof(T);
            if (systemsOfTag.ContainsKey(type))
            {
                foreach (var system in systemsOfTag[type])
                {
                    system.RunType = type;
                    system.m_Run();
                }
            }
        }

        public void RunSystemForAllEntity(Type type)
        {
            if (systemsOfTag.ContainsKey(type))
            {
                foreach (var system in systemsOfTag[type])
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
        public void RunSystemOfTag<T>(YuoComponent component) where T : ISystemTag
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
        public void RunSystemOfTag<T>(YuoEntity entity) where T : ISystemTag
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

        public void RunSystemOfTagType(Type type, YuoEntity entity)
        {
            if (GetSystemOfTag(type, out var systems))
            {
                foreach (var system in systems)
                {
                    system.RunType = type;
                    system.m_Run(entity);
                }
            }
        }

        public void RunSystemOfTag<T>(List<YuoEntity> entitys) where T : ISystemTag
        {
            Type type = typeof(T);
            if (GetSystemOfTag(type, out var systems))
            {
                foreach (var system in systems)
                {
                    system.RunType = type;
                    foreach (var entity in entitys)
                    {
                        system.m_Run(entity);
                    }
                }
            }
        }

        public void RunSystemOfTag<T>(params YuoEntity[] entitys) where T : ISystemTag
        {
            Type type = typeof(T);
            if (GetSystemOfTag(type, out var systems))
            {
                foreach (var system in systems)
                {
                    system.RunType = type;
                    foreach (var entity in entitys)
                    {
                        system.m_Run(entity);
                    }
                }
            }
        }

        /// <summary>
        /// 通过Tag获取系统
        /// </summary>
        private bool GetSystemOfTag(Type type, out List<SystemBase> systems)
        {
            if (systemsOfTag.ContainsKey(type))
            {
                systems = systemsOfTag[type];
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
            if (systemsOfComponent.ContainsKey(type))
            {
                systems = systemsOfComponent[type];
                return true;
            }

            systems = null;
            return false;
        }


        public MultiMap<Type, YuoComponent> GetAllComponents()
        {
            return components;
        }

        public Dictionary<long, YuoEntity> GetAllEntity()
        {
            return _entities;
        }

        public MultiMap<Type, SystemBase> GetAllSystemOfComponent => systemsOfComponent;

        public MultiMap<Type, SystemBase> GetAllSystemOfTag => systemsOfTag;

        public List<SystemBase> GetAllSystem => _allSystem;


        #region 反射获取组件

        static bool CheckComponent<TP>(Type find) where TP : class
        {
            var baseType = find.BaseType; //获取基类
            while (baseType != null) //获取所有基类
            {
                if (baseType.Name == typeof(TP).Name) return true;
                baseType = baseType.BaseType;
            }

            return false;
        }

        private static TP CheckSystem<TP>(Type find) where TP : class
        {
            var baseType = find.BaseType; //获取基类
            while (baseType != null) //获取所有基类
            {
                if (baseType.Name == typeof(TP).Name)
                {
                    try
                    {
                        object obj = Activator.CreateInstance(find);
                        if (obj != null)
                        {
                            TP info = obj as TP;
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

    public class SystemOrderAttribute : Attribute
    {
        public short Order { get; }

        public SystemOrderAttribute(short order)
        {
            Order = order;
        }
    }

    public class SceneComponent : YuoComponent
    {
    }
}
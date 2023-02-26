using System;
using System.Collections.Generic;
using UnityEngine;
using YuoTools.ECS;

namespace YuoTools.Main.Ecs
{
    public partial class YuoEntity : IDisposable
    {
        public YuoEntity Parent { get; private set; }

        public EntityComponent EntityData { get; private set; }
        [SerializeField] public Dictionary<Type, YuoComponent> Components { get; private set; }
        [SerializeField] public MultiMap<Type, YuoComponent> ChildComponents { get; private set; }
        [SerializeField] public List<YuoEntity> Children { get; private set; }

        public YuoEntity()
        {
            Components = new Dictionary<Type, YuoComponent>();
            ChildComponents = new MultiMap<Type, YuoComponent>();
            Children = new List<YuoEntity>();
            //EntityComponent为基础组件,无法移除,不会显示在组件列表中,但当销毁时会自动移除
            //可以通过获取EntityComponent是否为null来判断Entity是否释放
            EntityData = new EntityComponent();
            EntityData.Entity = this;

            EntityData.Id = IDGenerate.GetID(this);

            World.Instance.AddComponent(this, EntityData);
            World.Instance.RegisterEntity(this);
            // $"Add Entity{EntityData.Id}".Log();
        }

        public YuoEntity(long id)
        {
            Components = new Dictionary<Type, YuoComponent>();
            ChildComponents = new MultiMap<Type, YuoComponent>();
            Children = new List<YuoEntity>();
            //EntityComponent为基础组件,无法移除,不会显示在组件列表中,但当销毁时会自动移除
            //可以通过获取EntityComponent是否为null来判断Entity是否释放
            EntityData = new EntityComponent();
            EntityData.Entity = this;

            EntityData.Id = id;

            World.Instance.AddComponent(this, EntityData);
            World.Instance.RegisterEntity(this);
            // $"Add Entity{EntityData.Id}".Log();
        }

        public YuoEntity(string name) : this(name.GetHashCode())
        {
            EntityName = name;
        }

        public T GetComponent<T>() where T : YuoComponent
        {
            return GetComponent(typeof(T)) as T;
        }

        /// <summary>
        /// 如果不存在该组件,则添加该组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetOrAddComponent<T>() where T : YuoComponent, new()
        {
            if (TryGetComponent<T>(out var component))
            {
                return component;
            }

            return AddComponent<T>();
        }

        public bool TryGetComponent<T>(out T component) where T : YuoComponent
        {
            component = GetComponent<T>();
            if (component == null)
                return false;
            return true;
        }

        public YuoComponent GetComponent(Type type)
        {
            if (IsDisposed) return null;
            return Components.ContainsKey(type) ? Components[type] : null;
        }

        public bool ContainsComponent(Type type)
        {
            return Components.ContainsKey(type);
        }

        public bool ContainsComponent<T>() where T : YuoComponent
        {
            return ContainsComponent(typeof(T));
        }

        public void SetComponent<T>(T component, Type componentType = null) where T : YuoComponent
        {
            if (componentType != null) component.SetComponentType(componentType);
            var componentTemp = GetComponent(component.Type);
            if (componentTemp == null)
            {
                component.Entity = this;
                Components.Add(component.Type, component);
                Parent?.ChildComponents.AddItem(component.Type, component);
                World.Instance.AddComponent(this, component);
            }
            else
            {
                Components[component.Type] = component;
                component.Entity = this;
                World.Instance.SetComponent(componentTemp, component);
            }
        }

        public YuoComponent AddComponent(Type type)
        {
            if (Components.ContainsKey(type)) return GetComponent(type);
            if (Activator.CreateInstance(type) is not YuoComponent component) return null;
            component.Entity = this;
            Components.Add(type, component);
            Parent?.ChildComponents.AddItem(type, component);
            World.Instance.AddComponent(this, component, type);

            return component;
        }

        public T AddComponent<T>() where T : YuoComponent, new()
        {
            if (GetComponent<T>() != null) return GetComponent<T>();
            T component = new T();
            component.Entity = this;
            var type = typeof(T);
            Components.Add(type, component);
            World.Instance.AddComponent(this, component);
            Parent?.AddChildComponent(type, component);
            return component;
        }

        void AddChildComponent(Type type, YuoComponent component)
        {
            if (component == null) return;
            ChildComponents.AddItem(type, component);
            if (!Children.Contains(component.Entity)) Children.Add(component.Entity);
        }

        /// <summary>
        /// 链接父组件,当执行子组件时也会调用父组件,用于组件继承
        /// </summary>
        public void Connect<T1>(YuoComponent child) where T1 : YuoComponent, new()
        {
            var type = typeof(T1);
            if (child == null || GetComponent<T1>() != null) return;
            child.ConnectedType = type;
            Components.Add(typeof(T1), child);
        }

        public void RemoveComponent<T>() where T : YuoComponent
        {
            RemoveComponent(GetComponent<T>());
        }

        public void RemoveComponent(Type type)
        {
            RemoveComponent(GetComponent(type));
        }

        public void RemoveComponent<T>(T component) where T : YuoComponent
        {
            if (component == null || !Components.ContainsKey(component.Type)) return;
            var connect = component.ConnectedType;
            Components.Remove(component.Type);
            World.Instance.RemoveComponent(this, component);
            if (Parent != null)
                Parent.RemoveChildComponent(component);
            if (connect != null) RemoveComponent(connect);
        }

        public T GetChild<T>(int index) where T : YuoComponent
        {
            var cs = ChildComponents[typeof(T)];
            return cs is {Count: > 0} ? cs[index] as T : null;
        }

        public List<T> GetChildren<T>() where T : YuoComponent
        {
            var children = new List<T>();
            if (ChildComponents.TryGetValue(typeof(T), out var cs))
            {
                foreach (var item in cs)
                {
                    children.Add(item as T);
                }
            }

            return children;
        }

        public T GetChild<T>() where T : YuoComponent
        {
            var cs = ChildComponents[typeof(T)];
            return cs is {Count: > 0} ? cs[0] as T : null;
        }

        public T AddChild<T>(long entityID = long.MinValue) where T : YuoComponent, new()
        {
            return AddChild(typeof(T), entityID) as T;
        }

        public T AddChild<T>(string name) where T : YuoComponent, new()
        {
            var com = AddChild(typeof(T), name.GetHashCode()) as T;
            if (com != null) com.Entity.EntityName = name;
            return com;
        }

        public YuoComponent AddChild(Type type, long entityID = long.MinValue)
        {
            var child = entityID != long.MinValue ? new YuoEntity(entityID) : new YuoEntity();
            child.Parent = this;
            var component = child.AddComponent(type);
            AddChildComponent(type, component);
            return component;
        }

        public YuoEntity AddChild(long entityID = long.MinValue)
        {
            var child = entityID != long.MinValue ? new YuoEntity(entityID) : new YuoEntity();
            child.Parent = this;
            var type = typeof(EntityComponent);
            AddChildComponent(type, child.EntityData);
            return child;
        }

        public YuoEntity AddChild(string entityName)
        {
            var child = new YuoEntity(entityName.GetHashCode());
            child.EntityName = entityName;
            child.Parent = this;
            var type = typeof(EntityComponent);
            AddChildComponent(type, child.EntityData);
            return child;
        }

        public void RemoveChild(YuoEntity entity)
        {
            if (!IsDisposed)
            {
                if (Children.Contains(entity))
                    Children.Remove(entity);
                foreach (var item in entity.Components)
                {
                    RemoveChildComponent(item.Value);
                    ChildComponents.RemoveItem(item.Key, item.Value);
                }
            }
        }

        private void RemoveChildComponent(YuoComponent component)
        {
            if (component == null) return;
            ChildComponents.RemoveItem(component.Type, component);
        }

        internal void SetParent(YuoEntity parent)
        {
            if (Parent != null)
            {
                Parent.RemoveChild(this);
            }

            Parent = parent;
        }

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 释放实体,不要在System中调用,如需释放请调用Destroy
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            if (Parent != null) Parent.RemoveChild(this);
            World.Instance.UnRegisterEntity(this);
            foreach (var item in Components.Values)
            {
                World.Instance.RemoveComponent(this, item);
                item.Dispose();
            }

            // foreach (var item in ChildComponents.Values)
            // {
            //     foreach (var entity in item)
            //     {
            //         entity.Dispose();
            //     }
            // }
            
            foreach (var yuoEntity in Children)
            {
                yuoEntity.Dispose();
            }

            Components.Clear();
            ChildComponents.Clear();
            ChildComponents = null;

            World.Instance.RemoveComponent(this, EntityData);
            EntityData.Dispose();
            EntityData = null;
            IsDisposed = true;
        }

        /// <summary>
        /// 扔进回收站,这一帧结束后会被回收
        /// </summary>
        public void Destroy()
        {
            World.DestroyEntity(this);
        }
    }
}
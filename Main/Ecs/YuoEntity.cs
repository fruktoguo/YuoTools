using System;
using System.Collections.Generic;
using YuoTools.ECS;

namespace YuoTools.Main.Ecs
{
    public partial class YuoEntity : IDisposable
    {
        public YuoEntity Parent { get; private set; }

        public EntityComponent EntityData { get; private set; }

        public Dictionary<Type, YuoComponent> Components { get; private set; }

        public MultiMap<Type, YuoComponent> ChildComponents { get; private set; }

        public List<YuoEntity> Children { get; private set; }


        public YuoEntity()
        {
            Components = new Dictionary<Type, YuoComponent>();
            ChildComponents = new MultiMap<Type, YuoComponent>();
            Children = new List<Main.Ecs.YuoEntity>();
            //EntityComponent为基础组件,无法移除,不会显示在组件列表中,但当销毁时会自动移除
            //可以通过获取EntityComponent是否为null来判断Entity是否释放
            EntityData = new EntityComponent();
            EntityData.Entity = this;
            EntityData.Id = IDGenerater.GetID(this);

            World.Instance.AddComponet(this, EntityData);
            World.Instance.RegisterEntity(this);
        }

        public YuoEntity(long id)
        {
            Components = new Dictionary<Type, YuoComponent>();
            ChildComponents = new MultiMap<Type, YuoComponent>();
            Children = new List<Main.Ecs.YuoEntity>();
            EntityData = new EntityComponent();
            EntityData.Entity = this;
            EntityData.Id = id;

            World.Instance.AddComponet(this, EntityData);
            World.Instance.RegisterEntity(this);
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
            if (Components.ContainsKey(type))
                return Components[type];
            return null;
        }

        public void SetComponent<T>(T component) where T : YuoComponent
        {
            var componentTemp = GetComponent(component.Type);
            if (componentTemp == null)
            {
                component.Entity = this;
                Components.Add(component.Type, component);
                World.Instance.AddComponet(this, component);
            }
            else
            {
                Components[component.Type] = component;
                component.Entity = this;
                World.Instance.SetComponet(componentTemp, component);
            }
        }

        public YuoComponent AddComponent(Type type)
        {
            if (Components.ContainsKey(type)) return GetComponent(type);
            if (Activator.CreateInstance(type) is not YuoComponent component) return null;
            component.Entity = this;
            Components.Add(type, component);
            World.Instance.AddComponet(this, component, type);
            return component;
        }

        public T AddComponent<T>() where T : YuoComponent, new()
        {
            if (GetComponent<T>() != null) return GetComponent<T>();
            T component = new T();
            component.Entity = this;
            var type = typeof(T);
            Components.Add(type, component);
            World.Instance.AddComponet(this, component);
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
            World.Instance.RemoveComponet(this, component);
            if (Parent != null)
                Parent.RemoveChildComponent(component);
            if (connect != null) RemoveComponent(connect);
        }

        private void RemoveChildComponent(YuoComponent component)
        {
            if (component == null) return;
            ChildComponents.RemoveItem(component.Type, component);
        }

        public T GetChild<T>(int index) where T : YuoComponent
        {
            var cs = ChildComponents[typeof(T)];
            return cs is { Count: > 0 } ? cs[index] as T : null;
        }

        public List<T> GetChilds<T>() where T : YuoComponent
        {
            if (ChildComponents.TryGetValue(typeof(T), out var cs))
            {
                return cs as List<T>;
            }

            return null;
        }

        public List<Main.Ecs.YuoEntity> GetAllChild() => Children;

        public T GetChild<T>() where T : YuoComponent
        {
            var cs = ChildComponents[typeof(T)];
            return cs is { Count: > 0 } ? cs[0] as T : null;
        }

        public T AddChild<T>(long entityID = long.MinValue) where T : YuoComponent, new()
        {
            return AddChild(typeof(T), entityID) as T;
        }

        public YuoComponent AddChild(Type type, long entityID = long.MinValue)
        {
            var child = entityID != long.MinValue ? new Main.Ecs.YuoEntity(entityID) : new Main.Ecs.YuoEntity();
            child.Parent = this;
            var component = child.AddComponent(type);
            AddChildComponent(type, component);
            return component;
        }

        public Main.Ecs.YuoEntity AddChild(long entityID = long.MinValue)
        {
            var child = entityID != long.MinValue ? new Main.Ecs.YuoEntity(entityID) : new Main.Ecs.YuoEntity();
            child.Parent = this;
            var type = typeof(EntityComponent);
            AddChildComponent(type, child.EntityData);
            return child;
        }

        bool _isDisposed = false;

        public void RemoveChild(Main.Ecs.YuoEntity entity)
        {
            if (!_isDisposed)
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

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            if (Parent != null) Parent.RemoveChild(this);
            World.Instance.UnRegisterEntity(this);
            foreach (var item in Components.Values)
            {
                World.Instance.RemoveComponet(this, item);
                item.Dispose();
            }

            foreach (var item in ChildComponents.Values)
            {
                foreach (var entity in item)
                {
                    entity.Dispose();
                }
            }

            Components.Clear();
            ChildComponents.Clear();
            ChildComponents = null;

            World.Instance.RemoveComponet(this, EntityData);
            EntityData.Dispose();
            EntityData = null;
            _isDisposed = true;
        }
    }
}
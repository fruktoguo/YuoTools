using System;

namespace YuoTools.Main.Ecs
{
    public partial class YuoComponent : IDisposable
    {
        [Newtonsoft.Json.JsonIgnore] public YuoEntity Entity { get; set; }
        [NonSerialized] internal Type ConnectedType;
        [Newtonsoft.Json.JsonIgnore] public virtual string Name => Type.Name;

        public bool IsDisposed { get; private set; }

        public YuoComponent()
        {
            Type = GetType();
        }

        internal void SetComponentType(Type type)
        {
            Type = type;
        }

        public void Connect<P>() where P : YuoComponent, new()
        {
            Entity.Connect<P>(this);
        }

        [Newtonsoft.Json.JsonIgnore] public Type Type { get; private set; }

        public T GetComponent<T>() where T : YuoComponent
        {
            return Entity.GetComponent<T>();
        }

        public bool TryGetComponent<T>(out T component) where T : YuoComponent
        {
            return Entity.TryGetComponent(out component);
        }

        /// <summary>
        /// 添加组件到实体
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns></returns>
        public T AddComponent<T>() where T : YuoComponent, new()
        {
            return Entity.AddComponent<T>();
        }

        /// <summary>
        /// 释放组件,不要在System中调用,如需释放请调用Destroy
        /// </summary>
        public void Dispose()
        {
            Entity = null;
            IsDisposed = true;
            ConnectedType = null;
        }

        /// <summary>
        /// 扔进回收站,这一帧结束后会被回收
        /// </summary>
        public void Destroy()
        {
            World.DestroyComponent(this);
        }
    }

    public class EntityComponent : YuoComponent
    {
        public override string Name => "核心组件";
        public long Id { get; set; }
    }

    public partial class YuoEntity
    {
        public long ID => EntityData.Id;

        string _entityName = null;

        public string EntityName
        {
            get
            {
                if (_entityName == null)
                {
                    return ID.ToString();
                }

                return _entityName;
            }
            set => _entityName = value;
        }

        public override string ToString()
        {
            return EntityName + (IsDisposed ? "(已释放)" : "");
        }
    }
}
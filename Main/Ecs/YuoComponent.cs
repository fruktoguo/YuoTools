using System;

namespace YuoTools.Main.Ecs
{
    public partial class YuoComponent : IDisposable
    {
        [Newtonsoft.Json.JsonIgnore] public YuoEntity Entity { get; set; }
        [NonSerialized] internal Type ConnectedType;

        public void Dispose()
        {
            Entity = null;
            ConnectedType = null;
        }

        public YuoComponent()
        {
            Type = GetType();
        }

        public void Connect<P>() where P : YuoComponent, new()
        {
            Entity.Connect<P>(this);
        }

        [Newtonsoft.Json.JsonIgnore] public Type Type { get; }

        public T GetComponent<T>() where T : YuoComponent
        {
            return Entity.GetComponent<T>();
        }

        public bool TryGetComponent<T>(out T component) where T : YuoComponent
        {
            return Entity.TryGetComponent(out component);
        }

        public T AddComponent<T>() where T : YuoComponent, new()
        {
            return Entity.AddComponent<T>();
        }
    }

    public class EntityComponent : YuoComponent
    {
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
    }
}
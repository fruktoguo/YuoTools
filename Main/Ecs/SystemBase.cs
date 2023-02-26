using System;
using System.Collections.Generic;

namespace YuoTools.Main.Ecs
{
    public abstract class SystemBase
    {
        public bool Enabled { get; set; } = true;

        public readonly List<YuoEntity> Entitys = new();

        public Dictionary<YuoEntity, int> EntitysIndex = new();

        public int EntityCount => Entitys.Count;

        public virtual string Name => Type.Name;
        public virtual string Group => "";

        public double TimeConsuming { get; private set; }

        protected SystemBase()
        {
#if UNITY_EDITOR
            _sw = new System.Diagnostics.Stopwatch();
#endif
        }

        private void SetTimeConsuming(double time)
        {
            TimeConsuming = time;
        }

#if UNITY_EDITOR
        private readonly System.Diagnostics.Stopwatch _sw;
#endif

        public abstract void Init(World world);

        public abstract Type[] InfluenceTypes();

        public Type Type { get; internal set; }

        public Type RunType;

        internal void m_Run()
        {
            int length = EntityCount;
            if (length == 0)
            {
                return;
            }
#if UNITY_EDITOR
            _sw.Restart();
#endif
            for (int i = 0; i < length; i++)
            {
                m_Run(i);
            }

#if UNITY_EDITOR
            _sw.Stop();
            SetTimeConsuming(_sw.Elapsed.TotalMilliseconds);
#endif
        }

        internal virtual void Clear()
        {
            Entitys.Clear();
            systemTags.Clear();
        }

        internal abstract void m_Run(int entityIndex);

        internal void m_Run(YuoEntity entity)
        {
            if (!Enabled || !Entitys.Contains(entity)) return;
#if UNITY_EDITOR
            _sw.Restart();
#endif
            m_Run(Entitys.IndexOf(entity));
#if UNITY_EDITOR
            _sw.Stop();
            SetTimeConsuming(_sw.Elapsed.TotalMilliseconds);
#endif
        }

        internal abstract void SetComponent(YuoEntity entity, Type type, YuoComponent component2);

        internal abstract bool AddComponent(YuoEntity entity);

        internal abstract void RemoveComponent(YuoEntity entity);

        public List<Type> systemTags = new List<Type>();
    }
}
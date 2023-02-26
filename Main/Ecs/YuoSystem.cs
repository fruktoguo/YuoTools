using System;
using System.Collections.Generic;
using YuoTools.ECS;

namespace YuoTools.Main.Ecs
{
    public class EntitySystem : YuoSystem<EntityComponent>, IAwake
    {
        protected override void Run(EntityComponent component)
        {
        }
    }

    [Serializable]
    public abstract class YuoSystem<T1> : SystemBase where T1 : YuoComponent
    {
        private List<T1> _components1 = new();

        private Type _type1;

        public override void Init(World world)
        {
            _type1 = typeof(T1);
            world.RegisterSystem(this, typeof(T1));
        }

        public override Type[] InfluenceTypes() => new[] { _type1 };

        internal override bool AddComponent(YuoEntity entity)
        {
            var t1 = entity.GetComponent(_type1);
            if (t1 == null) return false;
            Entitys.Add(entity);
            _components1.Add(t1 as T1);
            return true;
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entitys.IndexOf(entity);
            if (index == -1) return;
            _components1[index] = component2 as T1;
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            int index = Entitys.IndexOf(entity);
            if (index == -1) return;
            Entitys.RemoveAt(index);
            _components1.RemoveAt(index);
        }

        internal override void m_Run(int entityIndex)
        {
            try
            {
                Run(_components1[entityIndex]);
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        internal override void Clear()
        {
            _components1.Clear();
            base.Clear();
        }

        protected abstract void Run(T1 component);
    }

    public abstract class YuoSystem<T1, T2> : SystemBase where T1 : YuoComponent where T2 : YuoComponent
    {
        private List<T1> _components1 = new();
        private List<T2> _components2 = new();

        private Type _type1;
        private Type _type2;

        public override void Init(World world)
        {
            _type1 = typeof(T1);
            _type2 = typeof(T2);
            world.RegisterSystem(this, _type1);
            world.RegisterSystem(this, _type2);
        }

        public override Type[] InfluenceTypes() => new[] { _type1, _type2 };

        internal override bool AddComponent(YuoEntity entity)
        {
            var t1 = entity.GetComponent(_type1);
            if (t1 == null) return false;
            var t2 = entity.GetComponent(_type2);
            if (t2 == null) return false;
            Entitys.Add(entity);
            _components1.Add(t1 as T1);
            _components2.Add(t2 as T2);
            return true;
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            int index = Entitys.IndexOf(entity);
            if (index == -1) return;
            Entitys.RemoveAt(index);
            _components1.RemoveAt(index);
            _components2.RemoveAt(index);
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entitys.IndexOf(entity);
            if (index == -1) return;
            if (_type1 == type)
            {
                _components1[index] = (T1)component2;
            }
            else if (_type2 == type)
            {
                _components2[index] = (T2)component2;
            }
        }

        internal override void m_Run(int entityIndex)
        {
            try
            {
                Run(_components1[entityIndex], _components2[entityIndex]);
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        internal override void Clear()
        {
            _components1.Clear();
            _components2.Clear();
            base.Clear();
        }

        protected abstract void Run(T1 component1, T2 component2);
    }

    public abstract class YuoSystem<T1, T2, T3> : SystemBase where T1 : YuoComponent
        where T2 : YuoComponent
        where T3 : YuoComponent
    {
        private readonly List<T1> _components1 = new();
        private readonly List<T2> _components2 = new();
        private readonly List<T3> _components3 = new();

        private Type _type1;
        private Type _type2;
        private Type _type3;

        public override void Init(World world)
        {
            _type1 = typeof(T1);
            _type2 = typeof(T2);
            _type3 = typeof(T3);
            world.RegisterSystem(this, _type1);
            world.RegisterSystem(this, _type2);
            world.RegisterSystem(this, _type3);
        }

        public override Type[] InfluenceTypes() => new[] { _type1, _type2, _type3 };

        internal override bool AddComponent(YuoEntity entity)
        {
            var t1 = entity.GetComponent(_type1);
            if (t1 == null) return false;
            var t2 = entity.GetComponent(_type2);
            if (t2 == null) return false;
            var t3 = entity.GetComponent(_type3);
            if (t3 == null) return false;
            Entitys.Add(entity);
            _components1.Add(t1 as T1);
            _components2.Add(t2 as T2);
            _components3.Add(t3 as T3);
            return true;
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entitys.IndexOf(entity);
            if (index == -1) return;
            if (_type1 == type)
            {
                _components1[index] = (T1)component2;
            }
            else if (_type2 == type)
            {
                _components2[index] = (T2)component2;
            }
            else if (_type3 == type)
            {
                _components3[index] = (T3)component2;
            }
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            int index = Entitys.IndexOf(entity);
            if (index == -1) return;
            Entitys.RemoveAt(index);
            _components1.RemoveAt(index);
            _components2.RemoveAt(index);
            _components3.RemoveAt(index);
        }

        internal override void m_Run(int entityIndex)
        {
            try
            {
                Run(_components1[entityIndex], _components2[entityIndex], _components3[entityIndex]);
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        internal override void Clear()
        {
            _components1.Clear();
            _components2.Clear();
            _components3.Clear();
            base.Clear();
        }

        protected abstract void Run(T1 component1, T2 component2, T3 component3);
    }

    public abstract class YuoSystem<T1, T2, T3, T4> : SystemBase where T1 : YuoComponent
        where T2 : YuoComponent
        where T3 : YuoComponent
        where T4 : YuoComponent
    {
        private readonly List<T1> _components1 = new();
        private readonly List<T2> _components2 = new();
        private readonly List<T3> _components3 = new();
        private readonly List<T4> _components4 = new();
        private Type _type1;
        private Type _type2;
        private Type _type3;
        private Type _type4;

        public override void Init(World world)
        {
            _type1 = typeof(T1);
            _type2 = typeof(T2);
            _type3 = typeof(T3);
            _type4 = typeof(T4);
            world.RegisterSystem(this, _type1);
            world.RegisterSystem(this, _type2);
            world.RegisterSystem(this, _type3);
            world.RegisterSystem(this, _type4);
        }

        public override Type[] InfluenceTypes() => new[] { _type1, _type2, _type3, _type4 };

        internal override bool AddComponent(YuoEntity entity)
        {
            var t1 = entity.GetComponent(_type1);
            if (t1 == null) return false;
            var t2 = entity.GetComponent(_type2);
            if (t2 == null) return false;
            var t3 = entity.GetComponent(_type3);
            if (t3 == null) return false;
            var t4 = entity.GetComponent(_type4);
            if (t4 == null) return false;
            Entitys.Add(entity);
            _components1.Add(t1 as T1);
            _components2.Add(t2 as T2);
            _components3.Add(t3 as T3);
            _components4.Add(t4 as T4);
            return true;
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entitys.IndexOf(entity);
            if (index == -1) return;
            if (_type1 == type)
            {
                _components1[index] = (T1)component2;
            }
            else if (_type2 == type)
            {
                _components2[index] = (T2)component2;
            }
            else if (_type3 == type)
            {
                _components3[index] = (T3)component2;
            }
            else if (_type4 == type)
            {
                _components4[index] = (T4)component2;
            }
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            var index = Entitys.IndexOf(entity);
            if (index == -1) return;
            Entitys.RemoveAt(index);
            _components1.RemoveAt(index);
            _components2.RemoveAt(index);
            _components3.RemoveAt(index);
            _components4.RemoveAt(index);
        }

        internal override void m_Run(int entityIndex)
        {
            try
            {
                Run(_components1[entityIndex], _components2[entityIndex], _components3[entityIndex],
                    _components4[entityIndex]);
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        internal override void Clear()
        {
            _components1.Clear();
            _components2.Clear();
            _components3.Clear();
            _components4.Clear();
            base.Clear();
        }

        protected abstract void Run(T1 component1, T2 component2, T3 component3, T4 component4);
    }

    public abstract class YuoSystem<T1, T2, T3, T4, T5> : SystemBase where T1 : YuoComponent
        where T2 : YuoComponent
        where T3 : YuoComponent
        where T4 : YuoComponent
        where T5 : YuoComponent
    {
        private readonly List<T1> _components1 = new();

        private readonly List<T2> _components2 = new();

        private readonly List<T3> _components3 = new();

        private readonly List<T4> _components4 = new();

        private readonly List<T5> _components5 = new();

        private Type _type1;
        private Type _type2;
        private Type _type3;
        private Type _type4;
        private Type _type5;

        public override void Init(World world)
        {
            _type1 = typeof(T1);
            _type2 = typeof(T2);
            _type3 = typeof(T3);
            _type4 = typeof(T4);
            _type5 = typeof(T5);
            world.RegisterSystem(this, _type1);
            world.RegisterSystem(this, _type2);
            world.RegisterSystem(this, _type3);
            world.RegisterSystem(this, _type4);
            world.RegisterSystem(this, _type5);
        }

        public override Type[] InfluenceTypes() => new[] { _type1, _type2, _type3, _type4, _type5 };

        internal override bool AddComponent(YuoEntity entity)
        {
            var t1 = entity.GetComponent(_type1);
            if (t1 == null) return false;
            var t2 = entity.GetComponent(_type2);
            if (t2 == null) return false;
            var t3 = entity.GetComponent(_type3);
            if (t3 == null) return false;
            var t4 = entity.GetComponent(_type4);
            if (t4 == null) return false;
            var t5 = entity.GetComponent(_type5);
            if (t5 == null) return false;
            Entitys.Add(entity);
            _components1.Add(t1 as T1);
            _components2.Add(t2 as T2);
            _components3.Add(t3 as T3);
            _components4.Add(t4 as T4);
            _components5.Add(t5 as T5);
            return true;
        }

        internal override void SetComponent(YuoEntity entity, Type type, YuoComponent component2)
        {
            int index = Entitys.IndexOf(entity);
            if (index == -1) return;
            if (_type1 == type)
            {
                _components1[index] = (T1)component2;
            }
            else if (_type2 == type)
            {
                _components2[index] = (T2)component2;
            }
            else if (_type3 == type)
            {
                _components3[index] = (T3)component2;
            }
            else if (_type4 == type)
            {
                _components4[index] = (T4)component2;
            }
            else if (_type5 == type)
            {
                _components5[index] = (T5)component2;
            }
        }

        internal override void RemoveComponent(YuoEntity entity)
        {
            var index = Entitys.IndexOf(entity);
            if (index == -1) return;
            Entitys.RemoveAt(index);
            _components1.RemoveAt(index);
            _components2.RemoveAt(index);
            _components3.RemoveAt(index);
            _components4.RemoveAt(index);
            _components5.RemoveAt(index);
        }

        internal override void m_Run(int entityIndex)
        {
            try
            {
                Run(_components1[entityIndex], _components2[entityIndex], _components3[entityIndex],
                    _components4[entityIndex], _components5[entityIndex]);
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        internal override void Clear()
        {
            _components1.Clear();
            _components2.Clear();
            _components3.Clear();
            _components4.Clear();
            _components5.Clear();
            Entitys.Clear();
        }

        protected abstract void Run(T1 component1, T2 component2, T3 component3, T4 component4, T5 component5);
    }
}
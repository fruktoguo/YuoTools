using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace YuoTools.Main.Ecs
{
    public abstract class BuffComponent : YuoComponent
    {
        public int BuffCount;

        /// <summary>
        /// buff的最大层数,默认为1
        /// </summary>
        public virtual int BuffMaxCount { get; set; } = 1;

        [ShowInInspector] public virtual string BuffName { get; set; } = "一个测试用的未命名Buff";
        [ShowInInspector] public virtual string BuffDescription { get; set; } = "一个测试用的Buff描述";

        public float StartTime;

        /// <summary>
        /// buff的持续时间,默认为1秒
        /// </summary>
        public virtual float Duration { get; set; } = 1;

        public float EndTime;
    }

    [AutoAddToMain]
    public class BuffManagerComponent : YuoComponentGet<BuffManagerComponent>
    {
        /// <summary>
        /// 所有buff的索引,用于统一处理
        /// </summary>
        private List<BuffComponent> buffComponents = new();

        public void AddBuff<T>(YuoEntity entity) where T : BuffComponent, new()
        {
            var buffComponent = entity.GetComponent<T>();
            if (buffComponent == null)
            {
                buffComponent = entity.AddComponent<T>();
                World.RunSystem<IBuffCreateBefore>(buffComponent);
                World.RunSystem<IBuffCreate>(buffComponent);
                buffComponents.Add(buffComponent);
            }

            buffComponent.StartTime = Time.time;
            buffComponent.EndTime = buffComponent.StartTime + buffComponent.Duration;

            if (buffComponent.BuffCount < buffComponent.BuffMaxCount)
            {
                buffComponent.BuffCount++;
                World.RunSystem<IBuffAdd>(buffComponent);
                World.RunSystem<IBuffChange>(buffComponent);
            }
        }

        public void RemoveBuff<T>(YuoEntity entity) where T : BuffComponent, new()
        {
            var buffComponent = entity.GetComponent<T>();
            if (buffComponent != null)
            {
                buffComponent.BuffCount--;
                World.RunSystem<IBuffRemove>(buffComponent);
                World.RunSystem<IBuffChange>(buffComponent);
                if (buffComponent.BuffCount <= 0)
                {
                    entity.RemoveComponent<T>();
                    World.RunSystem<IBuffDelete>(buffComponent);
                }
            }
        }

        /// <summary>
        /// 删除buff
        /// </summary>
        public void DeleteBuff(BuffComponent buff)
        {
            if (!_tempDelete.Contains(buff))
            {
                _tempDelete.Add(buff);
            }
        }

        List<BuffComponent> _tempDelete = new();

        public void Update()
        {
            long nowTime = (long)(Time.time * 1000);
            foreach (var buff in buffComponents)
            {
                //检查是否过期
                if (buff.EndTime > nowTime)
                {
                    World.RunSystem<IBuffRemove>(buff);
                }
            }

            foreach (var buff in _tempDelete)
            {
                buffComponents.Remove(buff);
                World.RunSystem<IBuffDelete>(buff);
            }

            _tempDelete.Clear();
        }
    }


    public class BuffManagerSystem : YuoSystem<BuffManagerComponent>, IUpdate
    {
        protected override void Run(BuffManagerComponent component)
        {
            component.Update();
        }
    }

    /// <summary>
    /// buff增加时
    /// </summary>
    public interface IBuffAdd : ISystemTag
    {
    }

    /// <summary>
    /// buff减少时
    /// </summary>
    public interface IBuffRemove : ISystemTag
    {
    }

    /// <summary>
    ///  当Buff层数产生任意改变时
    /// </summary>
    public interface IBuffChange : ISystemTag
    {
    }

    /// <summary>
    /// buff添加时
    /// </summary>
    public interface IBuffCreate : ISystemTag
    {
    }

    /// <summary>
    /// buff添加前
    /// </summary>
    public interface IBuffCreateBefore : ISystemTag
    {
    }

    /// <summary>
    /// buff添加失败
    /// </summary>
    public interface IBuffCreateError : ISystemTag
    {
    }

    /// <summary>
    /// buff删除时
    /// </summary>
    public interface IBuffDelete : ISystemTag
    {
    }
}
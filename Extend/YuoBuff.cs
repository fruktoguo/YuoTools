using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.ECS
{
    public abstract class BuffComponent : YuoComponent
    {
        public int BuffCount;
        public int BuffMaxCount;

        //剩余时间
        public long StartTime;
        public long EndTime;
        public YuoOption<bool> BooleanOption = new();
    }

    public class BuffManagerComponent : YuoComponent
    {
        public List<BuffComponent> Buffs = new List<BuffComponent>();

        public void AddBuff<T>(Main.Ecs.YuoEntity entity) where T : BuffComponent, new()
        {
            var component = entity.GetComponent<T>();
            if (component == null)
            {
                component = entity.AddComponent<T>();
                World.Instance.RunSystemOfTag<IBuffCreateBefore>(component);
                if (component.BooleanOption.Get("CanAdd"))
                {
                    World.Instance.RunSystemOfTag<IBuffCreate>(component);
                }
                else
                {
                    World.Instance.RunSystemOfTag<IBuffCreateError>(component);
                    entity.RemoveComponent(component);
                }
            }
            else
            {
                if (component.BuffCount < component.BuffMaxCount)
                {
                    component.BuffCount++;
                    World.Instance.RunSystemOfTag<IBuffAdd>(component);
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

        public void AfterUpdate()
        {
            foreach (var buff in _tempDelete)
            {
                Buffs.Remove(buff);
                World.Instance.RunSystemOfTag<IBuffDelete>(buff);
            }

            _tempDelete.Clear();
        }
    }


    public class BuffManagerSystem : YuoSystem<BuffManagerComponent>, IUpdate
    {
        protected override void Run(BuffManagerComponent component)
        {
            long nowTime = (long) (Time.time * 1000);
            foreach (var buff in component.Buffs)
            {
                //检查是否过期
                if (buff.EndTime > nowTime)
                {
                    World.Instance.RunSystemOfTag<IBuffRemove>(buff);
                }
            }

            component.AfterUpdate();
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
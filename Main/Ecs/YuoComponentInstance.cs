using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuoTools.ECS;

namespace YuoTools.Main.Ecs
{
    /// <summary>
    /// 不会存,只是简化访问
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class YuoComponentGet<T> : YuoComponent where T : YuoComponent
    {
        protected static YuoEntity InstanceEntity = World.Main;
        public static T Instance => InstanceEntity.GetComponent<T>();
    }

    /// <summary>
    /// 会存的单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class YuoComponentInstance<T> : YuoComponent where T : YuoComponent
    {
        protected static YuoEntity InstanceEntity = World.Main;

        private static T _instance;

        public static T Instance => _instance ??= InstanceEntity.GetComponent<T>();
    }
}
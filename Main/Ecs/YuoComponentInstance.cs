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
    public class YuoComponentGet<T> : YuoComponent where T : YuoComponent, new()
    {
        private static YuoEntity _instanceEntity = World.Main;

        public static T Get
        {
            get
            {
                if (_instanceEntity == null || _instanceEntity.IsDisposed)
                {
                    _instanceEntity = World.Main;
                }

                return _instanceEntity.GetComponent<T>();
            }
        }

        public static void SetInstance(YuoEntity entity)
        {
            _instanceEntity = entity;
            entity.AddComponent<T>();
        }
    }

    /// <summary>
    /// 会存的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class YuoComponentInstance<T> : YuoComponent where T : YuoComponent, new()
    {
        private static YuoEntity _instanceEntity = World.Main;

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instanceEntity == null || _instanceEntity.IsDisposed)
                {
                    _instanceEntity = World.Main;
                }

                if (_instance == null || _instance.IsDisposed)
                {
                    var component = _instanceEntity.GetComponent<T>();
                    _instance = component ?? _instanceEntity.AddComponent<T>();
                }

                return _instance;
            }
        }

        public static void SetInstance(YuoEntity entity)
        {
            _instanceEntity = entity;
            entity.AddComponent<T>();
        }
    }
}
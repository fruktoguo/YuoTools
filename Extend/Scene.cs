using System;
using UnityEngine;
using YuoTools.Main.Ecs;
using YuoTools.UI;

namespace YuoTools.ECS
{
    [SystemOrder(short.MinValue)]
    public class SceneInitSystem : YuoSystem<SceneComponent>, IAwake
    {
        public class UnityEngineLog : YuoLog.LogComponent
        {
            public override T Log<T>(T msg)
            {
                Debug.Log(msg);
                return msg;
            }

            public override T Error<T>(T msg)
            {
                Debug.LogError(msg);
                return msg;
            }
        }
        protected override void Run(SceneComponent component)
        {
            YuoLog.Open(new UnityEngineLog());
            "Init".Log();
            //UI管理组件
            World.Main.AddComponent<UIManagerComponent>();
            //触摸控制器
            World.Main.AddComponent<YuoInputPointerComponent>();
            //控制器
            World.Main.AddComponent<YuoInputComponent>();
            
            World.Main.AddComponent<YuoInputCheckComponent>().Connect<InputCheckBaseComponent>();
            
            
            //存档管理器
            World.Main.AddComponent<SaveManagerComponent>();
            //游戏随机管理器
            World.Scene.AddComponent<YuoRandom>();
        }
    }
}
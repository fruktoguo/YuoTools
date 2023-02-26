using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YuoTools.ECS;
using YuoTools.Extend.Helper;
using YuoTools.Extend.UI;
using YuoTools.Main.Ecs;
using YuoTools.UI;

namespace YuoTools.Extend
{
    [SystemOrder(short.MinValue)]
    public class SceneInitSystem : YuoSystem<SceneComponent>, IAwake
    {
        public class UnityEngineLog : ECS.YuoLog.LogComponent
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
            ECS.YuoLog.Open(new UnityEngineLog());
            ECS.YuoLog.Log($"Init In {Time.frameCount} Frame");

            component.Entity.EntityName = $"Scene {World.Instance.AllScenes.Count - 1}";
            var componentManager = World.Main.GetOrAddComponent<ComponentManager>();
            var autoAddTo = World.Instance.GetAutoAddTo();

            foreach (var item in autoAddTo)
            {
                //auto to main entity
                var autoAddToMain = item.Value.GetCustomAttribute<AutoAddToMainAttribute>();
                var autoAddToScene = item.Value.GetCustomAttribute<AutoAddToSceneAttribute>();

                if (autoAddToMain != null || autoAddToScene != null)
                {
                    $"自动挂载组件 [ {item.Key} ] \n".MergeLog();

                    if (componentManager.Asset.AutoAdd.ContainsKey(item.Key))
                    {
                        if (!componentManager.Asset.AutoAdd[item.Key])
                        {
                            continue;
                        }
                    }
                    else
                    {
                        componentManager.Asset.AutoAdd.Add(item.Key,
                            autoAddToMain?.AutoAdd ?? autoAddToScene?.AutoAdd ?? false);
                    }
                }

                if (autoAddToMain is { AutoAdd: true })
                {
                    World.Main.AddComponent(item.Value);
                }

                //auto to scene entity
                if (autoAddToScene is { AutoAdd: true })
                {
                    World.Scene.AddComponent(item.Value);
                }
            }

            YuoLog.MergeLogOutput();
        }
    }

    public class SceneDestroySystem : YuoSystem<SceneComponent>, IDestroy
    {
        protected override void Run(SceneComponent component)
        {
            World.Instance.AllScenes.Remove(component.Entity);
        }
    }

    public class SceneComponentHelper
    {
        public static void LoadScene(string sceneName)
        {
            DestroyAndCreateScene();
            SceneManager.LoadScene(sceneName);
        }

        public static void DestroyAndCreateScene()
        {
            World.Scene.Destroy();
            var scene = new YuoEntity();
            World.Instance.AllScenes[0] = scene;
            scene.AddComponent<SceneComponent>();
        }

        public static void ReloadScene()
        {
            DestroyAndCreateScene();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// 场景退出时调用
    /// </summary>
    public interface ISceneExit : ISystemTag
    {
    }
}
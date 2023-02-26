using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend
{
    public class WorldInit
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void WorldInitBeforeSceneLoad()
        {
            new GameObject("World").AddComponent<World>();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend.Helper
{
    public static class TransformFindHelper
    {
        public static Transform Find(this YuoComponent component, string name)
        {
            var find = component.GetComponent<UIFindComponent>();
            find ??= component.AddComponent<UIFindComponent>();
            return find.Find(name);
        }

        public class UIFindComponent : YuoComponent
        {
            public Transform root;
            
            Dictionary<string, Transform> cache = new Dictionary<string, Transform>();

            public Transform Find(string name)
            {
                if (cache.TryGetValue(name, out var value))
                    return value;
                var find = root.Find(name);
                cache.Add(name, find);
                return find;
            }
        }
    }
}
using ET;
using UnityEngine;

namespace YuoTools.UI
{
    public class UILoadComponentResources : UILoadComponent
    {
        public override ETTask<GameObject> Load(string path, Transform parent)
        {
            var tcs = ETTask<GameObject>.Create(true);
            tcs.SetResult(Object.Instantiate(Resources.Load<GameObject>(path), parent));
            return tcs;
        }
    }
}
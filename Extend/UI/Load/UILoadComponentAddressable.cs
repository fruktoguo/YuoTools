using ET;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace YuoTools.UI
{
    public class UILoadComponentAddressable : UILoadComponent
    {
        public override ETTask<GameObject> Load(string path, Transform parent)
        {
            ETTask<GameObject> tcs = ETTask<GameObject>.Create();
            Addressables.InstantiateAsync(path, parent).Completed += go =>
            {
                go.Result.gameObject.Hide();
                tcs.SetResult(go.Result);
            };
            return tcs;
        }
    }
}
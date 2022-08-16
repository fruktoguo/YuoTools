using System.Collections;
using System.Collections.Generic;
using ET;
using UnityEngine;
using YuoTools;
using YuoTools.ECS;

namespace YuoTools
{
    public class YuoAwait_Mono : SingletonSerializedMono<YuoAwait_Mono>
    {
        [SerializeField] private AwaitPools awaitPools = new AwaitPools();

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            gameObject.name = "YuoAwait";
        }

        private void Update()
        {
            CheckTimerOut();
        }

        private List<AwaitItem> removeList = new List<AwaitItem>();

        private void CheckTimerOut()
        {
            if (awaitPools.ActiveCount > 0)
            {
                for (int i = 0; i < awaitPools.Actives.Count; i++)
                {
                    var item = awaitPools.Actives[i];
                    if (!item.unscaledTime)
                    {
                        if (Time.time < item.TargetTime) continue;
                    }
                    else if (Time.unscaledTime < item.TargetTime) continue;

                    item.tcs.SetResult();
                    removeList.Add(item);
                }

                if (removeList.Count > 0)
                {
                    foreach (var item in removeList)
                    {
                        awaitPools.Remove(item);
                    }

                    removeList.Clear();
                }
            }
        }

        public async ETTask WaitUnscaledTimeAsync(float waitTime)
        {
            AwaitItem item = awaitPools.GetItem();
            item.CreatTime = Time.unscaledTime;
            item.TargetTime = Time.unscaledTime + waitTime;
            await item.tcs;
        }

        public async ETTask WaitTimeAsync(float waitTime)
        {
            AwaitItem item = awaitPools.GetItem();
            item.CreatTime = Time.time;
            item.TargetTime = Time.time + waitTime;
            await item.tcs;
        }

        public ETTask<T> ResourcesLoadAsync<T>(string path)
            where T : Object
        {
            ETTask<T> tcs = ETTask<T>.Create(true);
            Instance.StartCoroutine(LoadAsset(path, tcs));
            return tcs;
        }

        IEnumerator LoadAsset<T>(string path, ETTask<T> tcs)
            where T : Object
        {
            ResourceRequest asset = Resources.LoadAsync<T>(path);
            yield return asset;
            var go = asset.asset as T;
            tcs.SetResult(go);
        }

        private class AwaitItem
        {
            public ETTask tcs;

            /// <summary>
            /// 添加时间
            /// </summary>
            public float CreatTime = 0;

            /// <summary>
            /// 目标时间
            /// </summary>
            public float TargetTime = 0;

            public bool unscaledTime = false;

            public AwaitItem()
            {
                tcs = ETTask.Create(true);
            }
        }

        private class AwaitPools : PoolsBase<AwaitItem>
        {
            public override AwaitItem CreatItem()
            {
                return new AwaitItem();
            }

            public override void OnDestroyItem(AwaitItem item)
            {
            }

            public override void OnResetItem(AwaitItem item)
            {
                item.unscaledTime = false;
                item.tcs = ETTask.Create(true);
                //item.taskSource = new TaskCompletionSource<bool>();
                //item.taskSource.Recycle();
            }
        }
    }
}
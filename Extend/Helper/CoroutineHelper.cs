using System;
using System.Collections;
using ET;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace YuoTools.Extend.Helper
{
    public static class CoroutineHelper
    {
        public static async ETTask GetAwaiter(this AsyncOperation asyncOperation)
        {
            var task = ETTask.Create(true);
            asyncOperation.completed += _ => { task.SetResult(); };
            await task;
        }

        public static async ETTask<string> HttpGet(string link)
        {
            try
            {
                var req = UnityWebRequest.Get(link);
                await req.SendWebRequest();
                return req.downloadHandler.text;
            }
            catch (Exception e)
            {
                throw new Exception($"http request fail: {link.Substring(0, link.IndexOf('?'))}\n{e}");
            }
        }

        public static ETTask<T> GetAwaiter<T>(this ResourceRequest request) where T : Object
        {
            var task = ETTask<T>.Create(true);
            YuoAwait_Mono.Instance.StartCoroutine(LoadAsset(request, task));
            return task;
        }

        private static IEnumerator LoadAsset<T>(ResourceRequest request, ETTask<T> tcs) where T : Object
        {
            yield return request;
            var go = request.asset as T;
            tcs.SetResult(go);
        }

        private static ETTask<T> ToAwaiter<T>(T enumerator) where T : IEnumerator
        {
            var task = ETTask<T>.Create();
            YuoAwait_Mono.Instance.StartCoroutine(ToAwaiter(enumerator, task));
            return task;
        }

        private static IEnumerator ToAwaiter<T>(T enumerator, ETTask<T> task) where T : IEnumerator
        {
            while (enumerator.MoveNext()) yield return enumerator.Current;

            task.SetResult(enumerator);
        }
    }
}
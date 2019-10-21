using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace YuoTools
{
    public class YuoDelayCon : SingletonMono<YuoDelayCon>
    {
        public List<YuoDealyMod> Invokes = new List<YuoDealyMod>();
        public List<YuoDealyMod> InvokesRealtime = new List<YuoDealyMod>();
        IEnumerator IYuoInvoke(YuoDealyMod yuoDelayMod)
        {
            yield return null;
            yield return YuoWait.GetWait((int)(yuoDelayMod.DelayTime * 1000));
            if (yuoDelayMod.End)
            {
                Invokes.Remove(yuoDelayMod);
                yield break;
            }
            yuoDelayMod.action?.Invoke();
            Invokes.Remove(yuoDelayMod);
            yield break;
        }
        IEnumerator IYuoInvokeRealtime(YuoDealyMod yuoDelayMod)
        {
            yield return null;
            yield return YuoWait.GetWaitRealtime((int)(yuoDelayMod.DelayTime * 1000));
            if (yuoDelayMod.End)
            {
                InvokesRealtime.Remove(yuoDelayMod);
                yield break;
            }
            yuoDelayMod.action?.Invoke();
            InvokesRealtime.Remove(yuoDelayMod);
            yield break;
        }

        public YuoDealyMod Invoke(UnityAction unityAction, float delay)
        {
            YuoDealyMod mod = new YuoDealyMod() { StartTime = DateTime.Now.ToString(), DelayTime = delay, action = unityAction };
            StartCoroutine(IYuoInvoke(mod));
            Invokes.Add(mod);
            return mod;
        }
        public YuoDealyMod InvokeRealtime(UnityAction unityAction, float delay)
        {
            YuoDealyMod mod = new YuoDealyMod() { StartTime = DateTime.Now.ToString(), DelayTime = delay, action = unityAction };
            StartCoroutine(IYuoInvokeRealtime(mod));
            Invokes.Add(mod);
            return mod;
        }

        public void StopCor(YuoDealyMod yuoInvokeMod)
        {
            yuoInvokeMod.End = true;
        }
    }
    [Serializable]
    public class YuoDealyMod
    {
        [Header("启动时间")]
        public string StartTime;
        [Header("延迟时间")]
        public float DelayTime;
        [Header("是否提前终止")]
        public bool End = false;
        public UnityAction action;
    }
    public static class YuoDelay
    {
        public static YuoDealyMod Delay(UnityAction action, float delay)
        {
            return YuoDelayCon.Instance.Invoke(action, delay);
        }
        public static YuoDealyMod DelayRealtime(UnityAction action, float delay)
        {
            return YuoDelayCon.Instance.InvokeRealtime(action, delay);
        }
        public static void Stop(YuoDealyMod yuoInvokeMod)
        {
            YuoDelayCon.Instance.StopCor(yuoInvokeMod);
        }
    }
}
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

        public List<YuoDealyMod> Pools = new List<YuoDealyMod>();
        IEnumerator IYuoDelay(YuoDealyMod yuoDelayMod)
        {
            yield return null;
            yield return YuoWait.GetWait((int)yuoDelayMod.DelayTime * 1000);
            for (int i = 0; i < yuoDelayMod.AddDelayTime.Count; i++)
            {
                yield return YuoWait.GetWait((int)yuoDelayMod.AddDelayTime[i] * 1000);
            }
            if (yuoDelayMod.End)
            {
                Invokes.Remove(yuoDelayMod);
                yield break;
            }
            yuoDelayMod.action?.Invoke();
            Invokes.Remove(yuoDelayMod);
            Pools.Add(yuoDelayMod);
            yield break;
        }
        IEnumerator IYuoDelayRealtime(YuoDealyMod yuoDelayMod)
        {
            yield return null;
            yield return YuoWait.GetWaitRealtime((int)yuoDelayMod.DelayTime * 1000);
            for (int i = 0; i < yuoDelayMod.AddDelayTime.Count; i++)
            {
                yield return YuoWait.GetWaitRealtime((int)yuoDelayMod.AddDelayTime[i] * 1000);
            }
            if (yuoDelayMod.End)
            {
                InvokesRealtime.Remove(yuoDelayMod);
                yield break;
            }
            yuoDelayMod.action?.Invoke();
            InvokesRealtime.Remove(yuoDelayMod);
            Pools.Add(yuoDelayMod);
            yield break;
        }

        public YuoDealyMod Invoke(UnityAction unityAction, float delay)
        {
            tempMod = GetMod(unityAction,delay);
            StartCoroutine(IYuoDelay(tempMod));
            Invokes.Add(tempMod);
            return tempMod;
        }
        YuoDealyMod tempMod;
        public YuoDealyMod InvokeRealtime(UnityAction unityAction, float delay)
        {
            tempMod = GetMod(unityAction,delay);
            StartCoroutine(IYuoDelayRealtime(tempMod));
            InvokesRealtime.Add(tempMod);
            return tempMod;
        }
        YuoDealyMod GetMod(UnityAction unityAction, float delay)
        {
            if (Pools.Count>0)
            {
                tempMod = Pools[0];
                tempMod.ReSet();
                tempMod.action = unityAction;
                tempMod.DelayTime = delay;
                tempMod.StartTime = DateTime.Now.ToString();
                Pools.Remove(tempMod);
            }
            else
            {
                tempMod = new YuoDealyMod() { StartTime = DateTime.Now.ToString(), DelayTime = delay, action = unityAction };
            }
            return tempMod;
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
        [Header("额外的延迟时间")]
        public List<float> AddDelayTime = new List<float>();
        [Header("是否提前终止")]
        public bool End = false;
        public UnityAction action;
        public void AddDelay(float time)
        {
            AddDelayTime.Add(time);
        }
        public void ReSet()
        {
            StartTime = "";
            DelayTime = 0;
            AddDelayTime.Clear();
            End = false;
            action = null;
        }
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
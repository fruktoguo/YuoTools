using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace YuoTools
{
    public class YuoDelayCon : SingletonMono<YuoDelayCon>
    {
        public List<YuoDelayMod> Invokes = new List<YuoDelayMod>();
        public List<YuoDelayMod> InvokesRealtime = new List<YuoDelayMod>();

        public List<YuoDelayMod> Pools = new List<YuoDelayMod>();
        IEnumerator IYuoDelay(YuoDelayMod yuoDelayMod)
        {
            yield return null;
            yield return YuoWait.GetWait((int)(yuoDelayMod.DelayTime * 1000));

            while (yuoDelayMod.ExtraDelayTime.Count > 0)
            {
                YuoTempVar.intTemp = (int)(yuoDelayMod.ExtraDelayTime[0] * 1000);
                yuoDelayMod.ExtraDelayTime.RemoveAt(0);
                yield return YuoWait.GetWait(YuoTempVar.intTemp);
            }
            if (!yuoDelayMod.End)
            {
                yuoDelayMod.action?.Invoke();
            }
            Invokes.Remove(yuoDelayMod);
            Pools.Add(yuoDelayMod);
            yield break;
        }
        IEnumerator IYuoDelayRealtime(YuoDelayMod yuoDelayMod)
        {
            yield return null;
            yield return YuoWait.GetWaitRealtime((int)(yuoDelayMod.DelayTime * 1000));

            while (yuoDelayMod.ExtraDelayTime.Count > 0)
            {
                YuoTempVar.intTemp = (int)(yuoDelayMod.ExtraDelayTime[0] * 1000);
                yuoDelayMod.ExtraDelayTime.RemoveAt(0);
                yield return YuoWait.GetWaitRealtime(YuoTempVar.intTemp);
            }
            if (!yuoDelayMod.End)
            {
                yuoDelayMod.action?.Invoke();
            }
            InvokesRealtime.Remove(yuoDelayMod);
            Pools.Add(yuoDelayMod);
            yield break;
        }

        public YuoDelayMod Invoke(UnityAction unityAction, float delay)
        {
            tempMod = GetMod(unityAction, delay);
            tempMod.coroutine = StartCoroutine(IYuoDelay(tempMod));
            Invokes.Add(tempMod);
            return tempMod;
        }
        YuoDelayMod tempMod;
        public YuoDelayMod InvokeRealtime(UnityAction unityAction, float delay)
        {
            tempMod = GetMod(unityAction, delay);
            tempMod.coroutine = StartCoroutine(IYuoDelayRealtime(tempMod));
            InvokesRealtime.Add(tempMod);
            return tempMod;
        }
        YuoDelayMod GetMod(UnityAction unityAction, float delay)
        {
            if (Pools.Count > 0)
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
                tempMod = new YuoDelayMod() { StartTime = DateTime.Now.ToString(), DelayTime = delay, action = unityAction };
            }
            return tempMod;
        }
        public void StopCor(YuoDelayMod yuoInvokeMod)
        {
            yuoInvokeMod.End = true;
        }
        public void StopForce(YuoDelayMod yuoDelayMod)
        {
            StopCoroutine(yuoDelayMod.coroutine);
            Invokes.Remove(yuoDelayMod);
            Pools.Add(yuoDelayMod);
        }
    }

    [Serializable]
    public class YuoDelayMod
    {
        [Header("启动时间")]
        public string StartTime;
        [Header("延迟时间")]
        public float DelayTime;
        [Header("额外的延迟时间")]
        public List<float> ExtraDelayTime = new List<float>();
        [Header("是否提前终止")]
        public bool End = false;
        public UnityAction action;
        public Coroutine coroutine;
        public void AddDelay(float time)
        {
            ExtraDelayTime.Add(time);
        }
        public void SetDelay(float time)
        {
            YuoTempVar.floatTemp = DelayTime;
            foreach (var item in ExtraDelayTime)
            {
                YuoTempVar.floatTemp += item;
            }
            if (time > YuoTempVar.floatTemp)
            {
                AddDelay(time - YuoTempVar.floatTemp);
            }
            else
            {
                YuoDelayCon.Instance.StopForce(this);
                YuoDelay.SwitchMod(this, YuoDelay.Delay(action, time));
            }
        }
        public void ReSet()
        {
            StartTime = "";
            DelayTime = 0;
            ExtraDelayTime.Clear();
            End = false;
            action = null;
        }
    }
    public static class YuoDelay
    {
        public static void SwitchMod(YuoDelayMod mod1, YuoDelayMod mod2)
        {
            mod1 = mod2;
        }
        public static YuoDelayMod Delay(UnityAction action, float delay)
        {
            return YuoDelayCon.Instance.Invoke(action, delay);
        }
        public static YuoDelayMod DelayRealtime(UnityAction action, float delay)
        {
            return YuoDelayCon.Instance.InvokeRealtime(action, delay);
        }
        public static void Stop(YuoDelayMod yuoInvokeMod)
        {
            YuoDelayCon.Instance.StopCor(yuoInvokeMod);
        }
    }
}
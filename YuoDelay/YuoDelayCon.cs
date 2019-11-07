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
            yield return YuoWait.GetWait(yuoDelayMod.DelayTime);

            while (yuoDelayMod.ExtraDelayTime.Count > 0)
            {
                Temp.Float = yuoDelayMod.ExtraDelayTime[0];
                yuoDelayMod.ExtraDelayTime.RemoveAt(0);
                yield return YuoWait.GetWait(Temp.Float);
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
                Temp.Int = (int)(yuoDelayMod.ExtraDelayTime[0] * 1000);
                yuoDelayMod.ExtraDelayTime.RemoveAt(0);
                yield return YuoWait.GetWaitRealtime(Temp.Int);
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
            tempMod = GetMod(unityAction, delay, true);
            tempMod.coroutine = StartCoroutine(IYuoDelayRealtime(tempMod));
            InvokesRealtime.Add(tempMod);
            return tempMod;
        }
        YuoDelayMod GetMod(UnityAction unityAction, float delay, bool realtime = false)
        {
            if (Pools.Count > 0)
            {
                Temp.Int = 0;
                tempMod = Pools[0];
                tempMod.ReSet();
                tempMod.action = unityAction;
                tempMod.DelayTime = delay;
                tempMod.StartTime = DateTime.Now.ToString();
                tempMod.IsRealtime = realtime;
                Pools.Remove(tempMod);
            }
            else
            {
                tempMod = new YuoDelayMod() { StartTime = DateTime.Now.ToString(), DelayTime = delay, action = unityAction, IsRealtime = realtime };
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
            if (yuoDelayMod.IsRealtime)
                InvokesRealtime.Remove(yuoDelayMod);
            else
                Invokes.Remove(yuoDelayMod);
            Pools.Add(yuoDelayMod);
        }
    }

    [Serializable]
    public class YuoDelayMod
    {
        [Header("方法名字")]
        public string Name;
        [Header("启动时间")]
        public string StartTime;
        [Header("延迟时间")]
        public float DelayTime;
        [Header("额外的延迟时间")]
        public List<float> ExtraDelayTime = new List<float>();
        [Header("是否提前终止")]
        public bool End = false;
        /// <summary>
        /// 是否受TimeScale影响
        /// </summary>
        public bool IsRealtime;
        public UnityAction action;
        public Coroutine coroutine;
        public void AddDelay(float time)
        {
            if (time <= 0) return;
            ExtraDelayTime.Add(time);
        }
        public void SetDelay(float time)
        {
            if (time < 0) return;
            Temp.Float = DelayTime;
            foreach (var item in ExtraDelayTime)
            {
                Temp.Float += item;
            }
            if (time > Temp.Float)
            {
                AddDelay(time - Temp.Float);
            }
            else
            {
                YuoDelayCon.Instance.StopForce(this);
                if (IsRealtime)
                    YuoDelay.SwitchMod(this, this.YuoDelayRealtime(action, time));
                else
                    YuoDelay.SwitchMod(this, this.YuoDelay(action, time));
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
        public void SetName(string name)
        {
            Name = name;
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
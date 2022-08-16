using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace YuoTools.Deprecated
{
    public class YuoDelayManagerTest : SingletonMono<YuoDelayManagerTest>
    {
        [SerializeField]
        private List<YuoDelayMod> Invokes = new List<YuoDelayMod>();

        [SerializeField]
        private List<YuoDelayMod> InvokesRealtime = new List<YuoDelayMod>();

        [SerializeField]
        private List<YuoDelayMod> Pools = new List<YuoDelayMod>();

        private void Update()
        {
            foreach (var item in Invokes)
            {
                YuoDelayUpdate(item);
            }
        }

        public class TimeData
        {
            public float NowTime;
        }

        private void YuoDelayUpdate(YuoDelayMod mod)
        {
        }

        private IEnumerator IYuoDelay<T>(T obj, YuoDelayMod mod)
        {
            yield return null;
            if (mod.IsUpdate)
            {
                while (Time.time - mod.StartTime < mod.DelayTime)
                {
                    yield return null;
                    mod.UpdateAction?.Invoke();
                }
            }
            else
            {
                yield return YuoWait.WaitTime(mod.DelayTime - Time.deltaTime);
                while (mod.ExtraDelayTime.Count > 0)
                {
                    Temp.Float = mod.ExtraDelayTime[0];
                    mod.ExtraDelayTime.RemoveAt(0);
                    yield return YuoWait.WaitTime(Temp.Float);
                }
            }
            if (!mod.End)
            {
                if (!mod.SafeGameObject || mod.SafeGameObject.activeSelf)
                {
                    if (!mod.UseSafe || obj.ToString() != "null")
                    {
                        mod.action?.Invoke();
                        mod.ModAction?.Invoke(mod);
                    }
                }
            }
            Invokes.Remove(mod);
            Pools.Add(mod);
            mod = null;
            yield break;
        }

        private IEnumerator IYuoDelayRealtime<T>(T obj, YuoDelayMod mod)
        {
            yield return null;
            if (mod.IsUpdate)
            {
                while (Time.unscaledTime - mod.UnStartTime < mod.DelayTime)
                {
                    yield return null;
                    mod.UpdateAction?.Invoke();
                }
            }
            else
            {
                yield return YuoWait.WaitUnscaledTime(mod.DelayTime);
                while (mod.ExtraDelayTime.Count > 0)
                {
                    Temp.Float = mod.ExtraDelayTime[0];
                    mod.ExtraDelayTime.RemoveAt(0);
                    yield return YuoWait.WaitUnscaledTime(Temp.Float);
                }
            }
            if (!mod.End)
            {
                if (!mod.UseSafe || obj.ToString() != "null")
                {
                    mod.action?.Invoke();
                    mod.ModAction?.Invoke(mod);
                }
            }
            InvokesRealtime.Remove(mod);
            Pools.Add(mod);
            yield break;
        }

        private IEnumerator IYuoDelayLoad<T>(UnityAction action, T obj)
        {
            yield return obj;
            action?.Invoke();
            yield break;
        }

        public YuoDelayMod Invoke<T>(T obj, UnityAction unityAction, float delay)
        {
            tempMod = GetMod(unityAction, delay);
            tempMod.coroutine = StartCoroutine(IYuoDelay(obj, tempMod));
            Invokes.Add(tempMod);
            return tempMod;
        }

        public YuoDelayMod Invoke<T>(T obj, UnityAction<YuoDelayMod> unityAction, float delay)
        {
            tempMod = GetMod(null, delay);
            tempMod.ModAction = unityAction;
            tempMod.coroutine = StartCoroutine(IYuoDelay(obj, tempMod));
            Invokes.Add(tempMod);
            return tempMod;
        }

        public void Invoke<T>(UnityAction unityAction, T delay)
        {
            StartCoroutine(IYuoDelayLoad(unityAction, delay));
        }

        private YuoDelayMod tempMod;

        public YuoDelayMod InvokeRealtime<T>(T obj, UnityAction unityAction, float delay)
        {
            tempMod = GetMod(unityAction, delay, true);
            tempMod.coroutine = StartCoroutine(IYuoDelayRealtime(obj, tempMod));
            InvokesRealtime.Add(tempMod);
            return tempMod;
        }

        public YuoDelayMod InvokeRealtime<T>(T obj, UnityAction<YuoDelayMod> unityAction, float delay)
        {
            tempMod = GetMod(null, delay);
            tempMod.ModAction = unityAction;
            tempMod.coroutine = StartCoroutine(IYuoDelayRealtime(obj, tempMod));
            InvokesRealtime.Add(tempMod);
            return tempMod;
        }

        private YuoDelayMod GetMod(UnityAction unityAction, float delay, bool realtime = false)
        {
            if (Pools.Count > 0)
            {
                Temp.Int = 0;
                tempMod = Pools[Pools.Count - 1];
                tempMod.ReSet();
                tempMod.action = unityAction;
                tempMod.DelayTime = delay;
                tempMod.StartTime = Time.time;
                tempMod.UnStartTime = Time.unscaledTime;
                tempMod.IsRealtime = realtime;
                Pools.Remove(tempMod);
            }
            else
            {
                tempMod = new YuoDelayMod() { StartTime = Time.time, UnStartTime = Time.unscaledTime, DelayTime = delay, action = unityAction, IsRealtime = realtime };
            }
            return tempMod;
        }

        /// <summary>
        /// 重要提示:请在确定该延迟指令还未结束再使用Stop,否则其他正在使用这个mod的延迟指令将被意外暂停
        /// </summary>
        /// <param name="mod"></param>
        public void StopCor(YuoDelayMod mod)
        {
            mod.End = true;
        }

        /// <summary>
        /// 重要提示:请在确定该延迟指令还未结束再使用Stop,否则其他正在使用这个mod的延迟指令将被意外暂停
        /// </summary>
        /// <param name="mod"></param>
        public void StopForce(YuoDelayMod mod)
        {
            StopCoroutine(mod.coroutine);
            if (Pools.Contains(mod)) return;
            if (mod.IsRealtime)
                InvokesRealtime.Remove(mod);
            else
                Invokes.Remove(mod);
            Pools.Add(mod);
        }
    }
}
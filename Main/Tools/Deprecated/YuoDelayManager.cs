using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace YuoTools.Deprecated
{
    public class YuoDelayManager : SingletonMono<YuoDelayManager>
    {
        [SerializeField]
        private List<YuoDelayMod> Invokes = new List<YuoDelayMod>();

        [SerializeField]
        private List<YuoDelayMod> InvokesRealtime = new List<YuoDelayMod>();

        [SerializeField]
        private List<YuoDelayMod> Pools = new List<YuoDelayMod>();

        private IEnumerator IYuoDelay<T>(T obj, YuoDelayMod mod)
        {
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
                yield return YuoWait.WaitTime(mod.DelayTime);
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

    [Serializable]
    public class YuoDelayMod
    {
        [Header("方法名字")]
        public string Name;

        [Header("启动时间")]
        public float StartTime;

        [HideInInspector]
        public float UnStartTime;

        [Header("延迟时间")]
        public float DelayTime;

        [Header("额外的延迟时间")]
        public List<float> ExtraDelayTime = new List<float>();

        [Header("是否提前终止")]
        public bool End = false;

        /// <summary>
        /// 更加安全的模式,会传入调用者(必须继承mono),如果调用者被销毁了,这个方法不会执行
        /// </summary>
        public bool UseSafe = true;

        /// <summary>
        /// 设置是否开启安全模式
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public YuoDelayMod SetSafe(bool b)
        {
            UseSafe = b;
            return this;
        }

        /// <summary>
        /// 设置是否关联物体(关联物体隐藏,则事件不会执行)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public YuoDelayMod SafeWithGO(GameObject go)
        {
            SafeGameObject = go;
            return this;
        }

        public GameObject SafeGameObject;
        private bool _isUpdate;

        /// <summary>
        /// 是否受TimeScale影响
        /// </summary>
        public bool IsRealtime;

        /// <summary>
        /// 具体的事件
        /// </summary>
        public UnityAction action;

        /// <summary>
        /// 传入自己
        /// </summary>
        public UnityAction<YuoDelayMod> ModAction;

        /// <summary>
        /// 每帧执行的事件
        /// </summary>
        private UnityAction _updateAction;

        /// <summary>
        /// 协程的返回值
        /// </summary>
        public Coroutine coroutine;

        /// <summary>
        /// 是否使用Update的模式
        /// </summary>
        public bool IsUpdate { get => _isUpdate; }

        /// <summary>
        /// 只能通过setUpdate的方式来设定
        /// </summary>
        public UnityAction UpdateAction { get => _updateAction; }

        /// <summary>
        /// 额外添加一个延迟
        /// </summary>
        /// <param name="time"></param>
        public void AddDelay(float time)
        {
            if (time <= 0) return;
            if (IsUpdate) DelayTime += time;
            else ExtraDelayTime.Add(time);
        }

        /// <summary>
        /// 重新设置延迟时间
        /// </summary>
        /// <param name="time"></param>
        public void SetDelay(float time)
        {
            if (time < 0) return;
            if (IsUpdate)
            {
                DelayTime = time;
                return;
            }
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
                YuoDelayManager.Instance.StopForce(this);
                if (IsRealtime)
                    Delay.SwitchMod(this, this.YuoDelayRealtime(action, time));
                else
                    Delay.SwitchMod(this, this.YuoDelay(action, time));
            }
        }

        /// <summary>
        /// 将计时模式转为update模式,并可每帧执行update事件
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public YuoDelayMod SetUpdate(UnityAction action)
        {
            _updateAction = action;
            _isUpdate = true;
            return this;
        }

        /// <summary>
        /// 重设这个mod
        /// </summary>
        public void ReSet()
        {
            StartTime = 0;
            UnStartTime = 0;
            DelayTime = 0;
            ExtraDelayTime.Clear();
            SafeGameObject = null;
            End = false;
            action = null;
            ModAction = null;
        }

        /// <summary>
        /// 可以设置名字,并在关闭的时候启用安全关闭模式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public YuoDelayMod SetName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        /// 安全关闭模式,需要设定名字之后再使用,如果名字被改变了则不会停止
        /// </summary>
        /// <param name="mod"></param>
        public void Stop(string name)
        {
            if (name.Equals(Name))
                YuoDelayManager.Instance.StopCor(this);
        }

        /// <summary>
        /// 通过获取mod来停止延迟,不一定安全
        /// </summary>
        public void Stop()
        {
            YuoDelayManager.Instance.StopCor(this);
        }

        /// <summary>
        /// 直接停止相关协程来停止延迟,不一定安全
        /// </summary>
        /// <param name="mod"></param>
        public void StopForce()
        {
            YuoDelayManager.Instance.StopForce(this);
        }
    }

    public static class Delay
    {
        public static void SwitchMod(YuoDelayMod mod1, YuoDelayMod mod2)
        {
            mod1 = mod2;
        }

        public static void Stop(YuoDelayMod yuoInvokeMod)
        {
            YuoDelayManager.Instance.StopCor(yuoInvokeMod);
        }

        public static YuoDelayMod YuoDelay<T>(this T obj, UnityAction action, float delay) where T : class
        {
            return YuoDelayManager.Instance.Invoke(obj, action, delay);
        }

        public static YuoDelayMod YuoDelay<T>(this T obj, UnityAction action) where T : class
        {
            return YuoDelayManager.Instance.Invoke(obj, action, 0);
        }

        public static YuoDelayMod YuoDelayRealtime<T>(this T obj, UnityAction action, float delay) where T : class
        {
            return YuoDelayManager.Instance.InvokeRealtime(obj, action, delay);
        }

        public static YuoDelayMod YuoDelayRealtime<T>(this T obj, UnityAction<YuoDelayMod> action, float delay) where T : class
        {
            return YuoDelayManager.Instance.InvokeRealtime(obj, action, delay);
        }

        public static YuoDelayMod YuoDelay<T>(this T obj, UnityAction<YuoDelayMod> action, float delay) where T : class
        {
            return YuoDelayManager.Instance.Invoke(obj, action, delay);
        }

        /// <summary>
        /// 重要提示:请在确定该延迟指令还未结束再使用Stop,否则其他正在使用这个mod的延迟指令将被意外暂停
        /// </summary>
        public static void YuoStop<T>(this YuoDelayMod yuoInvokeMod)
        {
            YuoDelayManager.Instance.StopCor(yuoInvokeMod);
        }

        public static void YuoStopForce<T>(this YuoDelayMod mod)
        {
            YuoDelayManager.Instance.StopForce(mod);
        }

        /// <summary>
        /// 重要提示:请在确定该延迟指令还未结束再使用Stop,否则其他正在使用这个mod的延迟指令将被意外暂停
        /// </summary>
        public static void YuoDelay<T>(this T obj, T load, UnityAction action) where T : class
        {
            YuoDelayManager.Instance.Invoke(action, load);
        }

        public static void YuoDelay<T>(UnityAction action, T load)
        {
            YuoDelayManager.Instance.Invoke(action, load);
        }
    }
}
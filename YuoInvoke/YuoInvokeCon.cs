using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace YuoTools
{
    public class YuoInvokeCon : SingletonMono<YuoInvokeCon>
    {
        public List<YuoInvokeMod> Invokes = new List<YuoInvokeMod>();
        IEnumerator IYuoInvoke(YuoInvokeMod yuoInvokeMod)
        {
            yield return null;
            yield return YuoWaitWaitForSec.GetWaitForSeconds((int)(yuoInvokeMod.DelayTime * 1000));
            if (yuoInvokeMod.End)
            {
                yield break;
            }
            yuoInvokeMod.action?.Invoke();
            Invokes.Remove(yuoInvokeMod);
            yield break;
        }

        public YuoInvokeMod Invoke(UnityAction unityAction, float delay)
        {
            YuoInvokeMod Yuoimod = new YuoInvokeMod() { StartTime = DateTime.Now.ToString(), DelayTime = delay, action = unityAction };
            StartCoroutine(IYuoInvoke(Yuoimod));
            Invokes.Add(Yuoimod);
            return Yuoimod;
        }
        public void StopCor(YuoInvokeMod yuoInvokeMod)
        {
            yuoInvokeMod.End = true;
        }
    }
    [Serializable]
    public class YuoInvokeMod
    {
        [Header("启动时间")]
        public string StartTime;
        [Header("延迟时间")]
        public float DelayTime;
        [Header("是否提前终止")]
        public bool End = false;
        public UnityAction action;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public static class YuoInvokeClass
{
    public static YuoInvokeMod Start(UnityAction action,float delay)
    {
        return YuoInvokeCon.Instance.Invoke(action, delay);
    }
    public static YuoInvokeMod YuoInvoke(this MonoBehaviour mono, UnityAction action, float delay)
    {
        return YuoInvokeCon.Instance.Invoke(action, delay);
    }
    public static void StopInvoke(this MonoBehaviour mono, YuoInvokeMod yuoInvokeMod)
    {
        YuoInvokeCon.Instance.StopCor(yuoInvokeMod);
    }
    public static void StopInvoke(YuoInvokeMod yuoInvokeMod)
    {
        YuoInvokeCon.Instance.StopCor(yuoInvokeMod);
    }
}

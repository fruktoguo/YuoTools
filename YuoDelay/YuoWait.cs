﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YuoTools
{
    public class YuoWait
    {

        static Dictionary<int, WaitForSeconds> waits = new Dictionary<int, WaitForSeconds>();
        /// <summary>
        /// 返回一个WaitForSeconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        //public static WaitForSeconds GetWait(int time = 1000)
        //{
        //    if (!waits.ContainsKey(time))
        //    {
        //        waits.Add(time, new WaitForSeconds((float)time / 1000));
        //    }
        //    return waits[time];
        //}
        public static WaitForSeconds GetWait(float time = 1)
        {
            YuoTempVar.intTemp = (int)(time * 1000);
            if (!waits.ContainsKey(YuoTempVar.intTemp))
            {
                waits.Add(YuoTempVar.intTemp, new WaitForSeconds(time));
            }
            return waits[YuoTempVar.intTemp];
        }
        static Dictionary<int, WaitForSecondsRealtime> waitsRealtime = new Dictionary<int, WaitForSecondsRealtime>();
        /// <summary>
        /// 返回一个不受TimeScale影响的WaitForSeconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        //public static WaitForSecondsRealtime GetWaitRealtime(int time = 1000)
        //{
        //    if (!waitsRealtime.ContainsKey(time))
        //    {
        //        waitsRealtime.Add(time, new WaitForSecondsRealtime((float)time / 1000));
        //    }
        //    return waitsRealtime[time];
        //}
        public static WaitForSecondsRealtime GetWaitRealtime(float time = 1)
        {
            YuoTempVar.intTemp = (int)(time * 1000);
            if (!waitsRealtime.ContainsKey(YuoTempVar.intTemp))
            {
                waitsRealtime.Add(YuoTempVar.intTemp, new WaitForSecondsRealtime(time));
            }
            return waitsRealtime[YuoTempVar.intTemp];
        }
    }
}
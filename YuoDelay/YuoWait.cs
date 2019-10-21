using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YuoTools
{
    public class YuoWait
    {

        static Dictionary<int, WaitForSeconds> waits = new Dictionary<int, WaitForSeconds>();
        /// <summary>
        /// 返回一个WaitForSeconds,单位(毫秒)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static WaitForSeconds GetWait(int time = 1000)
        {
            if (!waits.ContainsKey(time))
            {
                waits.Add(time, new WaitForSeconds((float)time / 1000));
            }
            return waits[time];
        }
        static Dictionary<int, WaitForSecondsRealtime> waitsRealtime = new Dictionary<int, WaitForSecondsRealtime>();
        /// <summary>
        /// 返回一个不受TimeScale影响的WaitForSeconds,单位(毫秒)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static WaitForSecondsRealtime GetWaitRealtime(int time)
        {
            if (!waitsRealtime.ContainsKey(time))
            {
                waitsRealtime.Add(time, new WaitForSecondsRealtime((float)time / 1000));
            }
            return waitsRealtime[time];
        }
    }
}
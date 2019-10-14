using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YuoTools
{
    public static class YuoLog
    {
        public const string ExtensionTagContains = "[yuolog]";
        public const string ExtensionTag = "[YuoLog]";
        static string ColorRGBTo16(this Color color)
        {
            return $"#{color.r.ColorFloat10To16()}{color.g.ColorFloat10To16()}{color.b.ColorFloat10To16()}";
        }
        static string ColorFloat10To16(this float f)
        {
            if (f * 255 <= 16)
                return $"0{System.Convert.ToString(Mathf.Clamp((int)(f * 255), 0, 255), 16)}";
            else
                return System.Convert.ToString(Mathf.Clamp((int)(f * 255), 0, 255), 16);
        }
        static string ColorInt10To16(this int i)
        {
            if (i * 255 <= 16)
                return $"0{System.Convert.ToString(i, 16)}";
            else
                return System.Convert.ToString(i, 16);
        }

        public static T Log<T>(this T obj)
        {
#if UNITY_EDITOR
            Debug.Log($"{ExtensionTag.LogSetColor(YuoColor.矢车菊的蓝色).LogSetBold()}{obj.ToString().LogSetBold().LogSetColor(YuoColor.深紫罗兰的蓝色)}");
#endif
            return obj;
        }
        public static void LogAll<T>(this List<T> objs)
        {
#if UNITY_EDITOR
            $"该 {typeof(T).ToString().LogSetColor(YuoColor.钢蓝) } 集合 的长度为 [{objs.Count.ToString().LogSetColor(YuoColor.深粉色)}]".Log();
            for (int i = 0; i < objs.Count; i++)
            {
                $"第 [{(i + 1).ToString().LogSetColor(YuoColor.深粉色)}] 个元素为 {objs[i].ToString().LogSetColor(YuoColor.耐火砖)}".Log();
            }
#endif
        }
        public static void LogAll<T>(this T[] objs)
        {
#if UNITY_EDITOR
            $"该 {typeof(T).ToString().LogSetColor(YuoColor.钢蓝)} 数组 的长度为 [{objs.Length.ToString().LogSetColor(YuoColor.深粉色)}]".Log();
            for (int i = 0; i < objs.Length; i++)
            {
                $"第 [{(i + 1).ToString().LogSetColor(YuoColor.深粉色)}] 个元素为 {objs[i].ToString().LogSetColor(YuoColor.耐火砖)}".Log();
            }
#endif
        }
        /// <summary>
        /// 更改控制台输出字体的颜色,请使用YuoColor
        /// </summary>
        /// <param name="color">请使用YuoColor</param>
        /// <returns></returns>
        public static string LogSetColor(this string str, string color)
        {
            return $"<color={color}>{str}</color>";
        }
        public static string LogSetColor(this string str, Color color)
        {
            return $"<color={color.ColorRGBTo16()}>{str}</color>";
        }
        public static string LogSetBold(this string str)
        {
            return $"<b>{str}</b>";
        }
        public static string LogSetLtalic(this string str)
        {
            return $"<i>{str}</i>";
        }
        public static string LogSetSize(this string str, int size)
        {
            return $"<size={size}>{str}</size>";
        }
        public static string LogSetAlpha(this string str, int alpha)
        {
            alpha = Mathf.Clamp(alpha, 0, 255);
            return $"{str}{alpha.ColorInt10To16()}";
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YuoTools
{
    public static class YuoTools
    {
        /// <summary>
        /// 根据T值，计算贝塞尔曲线上面相对应的点
        /// </summary>
        /// <param name="t"></param>T值
        /// <param name="p0"></param>起始点
        /// <param name="p1"></param>控制点
        /// <param name="p2"></param>目标点
        /// <returns></returns>根据T值计算出来的贝赛尔曲线点
        private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }
        /// <summary>
        /// 二维贝塞尔
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="controlPoint">控制点</param>
        /// <param name="endPoint">终点</param>
        /// <param name="segments">采样数量</param>
        /// <returns>返回的数组</returns>
        public static Vector2[] Bezier(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint, int segments)
        {
            float d = 1f / segments;
            Vector2[] points = new Vector2[segments - 1];
            for (int i = 0; i < points.Length; i++)
            {
                float t = d * (i + 1);
                points[i] = (1 - t) * (1 - t) * controlPoint + 2 * t * (1 - t) * startPoint + t * t * endPoint;
            }
            List<Vector2> rps = new List<Vector2>();
            rps.Add(controlPoint);
            rps.AddRange(points);
            rps.Add(endPoint);
            return rps.ToArray();
        }
        /// <summary>
        /// 三维塞尔尔
        /// </summary>
        /// <param name="startPoint"></param>起始点
        /// <param name="controlPoint"></param>控制点
        /// <param name="endPoint"></param>目标点
        /// <param name="segment"></param>采样点的数量
        /// <returns></returns>存储贝塞尔曲线点的数组
        public static Vector3[] Bezier(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segment)
        {
            Vector3[] path = new Vector3[segment];
            for (int i = 1; i <= segment; i++)
            {
                float t = i / (float)segment;
                Vector3 pixel = CalculateCubicBezierPoint(t, startPoint,
                    controlPoint, endPoint);
                path[i - 1] = pixel;
                Debug.Log(path[i - 1]);
            }
            return path;

        }
        /// <summary>
        ///复制内容到剪切板
        /// </summary>
        /// <param name="input"></param>
        public static void CopyToClipboard(string input)
        {
#if UNITY_EDITOR
            TextEditor t = new TextEditor();
            t.text = input;
            t.OnFocus();
            t.Copy();
#elif UNITY_IPHONE
        CopyTextToClipboard_iOS(input);  
#elif UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass tool = new AndroidJavaClass("com.my.ugcf.Tool");
        tool.CallStatic("CopyTextToClipboard", currentActivity, input);
#endif
        }
    }
}
using System;
using System.Collections.Generic;

namespace YuoTools.ECS
{
    public static class YuoLog
    {
        /// <summary>
        /// 关闭后所有debug失效
        /// </summary>
        static bool _openDebug = true;

        static bool _showTime = false;
        public static bool IsEditor = true;

        public static void Open(LogComponent logComponent)
        {
            _logComponent = logComponent;
            _openDebug = true;
        }

        public static void Close()
        {
            _openDebug = false;
            _logComponent = null;
        }

        private static string _mergeLog;
        
        public static void MergeLog<T>(T obj)
        {
            if (!_openDebug) return;
            _mergeLog += obj;
        }
        
        public static void MergeLogOutput()
        {
            if (!_openDebug) return;
            _mergeLog.Log();
            _mergeLog = "";
        }
        
        static LogComponent _logComponent;

        public abstract class LogComponent
        {
            public abstract T Log<T>(T msg);
            public abstract T Error<T>(T msg);
        }

        public static T Log<T>(this T obj)
        {
            if (!_openDebug) return obj;
            if (IsEditor)
            {
                UnityEngine.Debug.Log(_showTime
                    ? $"<color=#FFF00FF>[{DateTime.Now:mm:ss:fff}-{UnityEngine.Time.frameCount}]</color>"
                    : "" + obj);
                return obj;
            }

            _logComponent?.Log(obj);
            return obj;
        }

        public static T LogError<T>(this T obj)
        {
            if (!_openDebug) return obj;
            if (IsEditor)
            {
                UnityEngine.Debug.LogError(_showTime
                    ? $"<color=#FFF00FF>[{DateTime.Now:mm:ss:fff}-{UnityEngine.Time.frameCount}]</color>"
                    : "" + obj);
                return obj;
            }

            _logComponent?.Error(obj);
            return obj;
        }
    }
}
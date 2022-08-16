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

        static LogComponent _logComponent;

        public abstract class LogComponent
        {
            public abstract T Log<T>(T msg);
            public abstract T Error<T>(T msg);
        }

        public static T Log<T>(this T obj)
        {
            if (!_openDebug) return obj;
            _logComponent?.Log(obj);
            return obj;
        }

        public static T LogError<T>(this T obj)
        {
            if (!_openDebug) return obj;
            _logComponent?.Error(obj);
            return obj;
        }
    }
}
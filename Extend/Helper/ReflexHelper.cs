using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YuoTools.Extend.Helper
{
    public static class ReflexHelper
    {
        /// <summary>
        /// 调用对象的方法
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回值</returns>
        public static object InvokeMethod<T>(T obj, string methodName, params object[] parameters)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod(methodName);
            if (method != null) return method.Invoke(obj, parameters);
            return null;
        }

        /// <summary>
        /// 判断是否存在某个方法
        ///  </summary>
        public static bool IsExistMethod<T>(T obj, string methodName)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod(methodName);
            if (method != null) return true;
            return false;
        }

        /// <summary>
        /// 获取以某个前缀开头的方法
        ///  </summary>
        public static MethodInfo[] GetMethodByPrefix<T>(T obj, string prefix)
        {
            Type type = obj.GetType();
            MethodInfo[] methods = type.GetMethods();
            List<MethodInfo> list = new List<MethodInfo>();
            foreach (MethodInfo method in methods)
            {
                if (method.Name.StartsWith(prefix))
                {
                    list.Add(method);
                }
            }

            return list.ToArray();
        }

        public static void InvokeMethodByPrefix<T>(T obj, string prefix, params object[] parameters)
        {
            MethodInfo[] methods = GetMethodByPrefix(obj, prefix);
            foreach (MethodInfo method in methods)
            {
                method.Invoke(obj, parameters);
            }
        }

        /// <summary>
        /// 获取私有字段的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetPrivateField<T>( object instance, string fieldName)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            FieldInfo info = type.GetField(fieldName, flags);
            return (T)info.GetValue(instance);
        }
 
        /// <summary>
        /// 设置私有字段的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public static void SetPrivateField( object instance, string fieldName, object value)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            FieldInfo info = type.GetField(fieldName, flags);
            info.SetValue(instance, value);
        }
 
        /// <summary>
        /// 获取私有属性的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetPrivateProperty<T>( object instance, string propertyName)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            PropertyInfo info = type.GetProperty(propertyName, flags);
            return (T)info.GetValue(instance, null);
        }
 
        /// <summary>
        /// 设置私有属性的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetPrivateProperty<T>( object instance, string propertyName, object value)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            PropertyInfo info = type.GetProperty(propertyName, flags);
            info.SetValue(instance, value, null);
        }
 
        /// <summary>
        /// 调用私有方法
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object CallPrivateMethod( object instance, string methodName, params object[] param)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            MethodInfo info = type.GetMethod(methodName, flags);
            return info.Invoke(instance, param);
        }
    }
    public static class ReflexExtension
    {
        public static IEnumerable<T> GetAttributes<T>(
            this ICustomAttributeProvider member,
            bool inherit)
            where T : Attribute
        {
            try
            {
                return member.GetCustomAttributes(typeof(T), inherit).Cast<T>();
            }
            catch
            {
                return (IEnumerable<T>)new T[0];
            }
        }
        public static T GetAttribute<T>(this ICustomAttributeProvider member, bool inherit) where T : Attribute
        {
            T[] array = member.GetAttributes<T>(inherit).ToArray();
            return array.Length != 0 ? array[0] : default;
        }
        public static T GetAttribute<T>(this ICustomAttributeProvider member) where T : Attribute => member.GetAttribute<T>(false);
    }
}
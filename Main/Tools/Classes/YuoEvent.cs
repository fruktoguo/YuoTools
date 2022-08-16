using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace YuoTools
{
    public class YuoEvent<T>
    {
        public T Value { get; private set; }
        private List<UnityAction<T>> actions = new List<UnityAction<T>>();

        /// <summary>
        /// ����¼�
        /// </summary>
        /// <param name="action"></param>
        /// <param name="single">ͬһ���¼��������һ��</param>
        public void AddAction(UnityAction<T> action, bool single = false)
        {
            if (!single || !actions.Contains(action))
            {
                actions.Add(action);
            }
        }

        /// <summary>
        /// �Ƴ��¼�
        /// </summary>
        /// <param name="action"></param>
        public void RemoveAction(UnityAction<T> action)
        {
            if (actions.Contains(action))
            {
                actions.Remove(action);
            }
        }

        public YuoEvent(T v)
        {
            Value = v;
        }

        /// <summary>
        /// ͳһִ���¼�
        /// </summary>
        public void Invoke()
        {
            foreach (var item in actions)
            {
                item?.Invoke(Value);
            }
        }
    }
}
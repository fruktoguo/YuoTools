using System;
using System.Collections.Generic;

namespace YuoTools.ECS
{
    /// <summary>
    /// dic嵌套list
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    public class MultiMap<K, V> : Dictionary<K, List<V>>
    {
        private readonly List<V> _empty = new List<V>();

        public V[] Copy(K t)
        {
            TryGetValue(t, out var list);
            return list == null ? Array.Empty<V>() : list.ToArray();
        }

        public new List<V> this[K t] => TryGetValue(t, out var list) ? list : _empty;

        public void AddItem(K t, V k)
        {
            if (!TryGetValue(t, out var list))
            {
                list = new List<V>();
                Add(t, list);
            }

            list.Add(k);
        }

        public bool RemoveItem(K t, V k)
        {
            if (!TryGetValue(t, out var list))
            {
                Add(t, new List<V>());
                return false;
            }

            if (list.Contains(k))
            {
                list.Remove(k);
                return true;
            }

            return false;
        }

        public List<V> GetAll()
        {
            List<V> values = new();
            foreach (var item in this)
            {
                values.AddRange(item.Value);
            }

            return values;
        }

        public new bool Remove(K t)
        {
            if (!ContainsKey(t))
            {
                return false;
            }

            Remove(t);
            return true;
        }
    }

}
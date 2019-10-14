using System.Collections.Generic;
using UnityEngine;
namespace YuoTools
{
    public class ExtractorManger<T> : Singleton<ExtractorManger<T>>
    {
        Dictionary<string, Extractor<T>> extractors = new Dictionary<string, Extractor<T>>();

        /// <summary>
        /// 获取一个抽取器,如果没有则会创建一个新的
        /// </summary>
        /// <param name="extractorName"></param>
        /// <returns></returns>
        public Extractor<T> GetExtractor(string extractorName)
        {
            if (!extractors.ContainsKey(extractorName))
            {
                Extractor<T> ex = new Extractor<T>();
                extractors.Add(extractorName, ex);
                $"创建抽取器 [{extractorName.LogSetColor(YuoColor.番茄)}] 成功".Log();
            }
            return extractors[extractorName];
        }

        public void Remove(string extractorName)
        {
            if (!extractors.ContainsKey(extractorName))
            {
                Debug.Log($"抽取器 [{extractorName}] 不存在");
            }
            else
            {
                extractors.Remove(extractorName);
                Debug.Log($"移除 抽取器 [{extractorName}] 成功");
            }
        }

        public void RemoveAll()
        {
            extractors.Clear();
        }
    }

    public class RanMod<T>
    {
        public int Prop;
        public int RandomNum;
        public T ReturnObj;
        public RanMod(int prop, T obj)
        {
            Prop = prop;
            ReturnObj = obj;
            RandomNum = 0;
        }
    }

    public class Extractor<T>
    {
        List<RanMod<T>> RM = new List<RanMod<T>>();
        List<RanMod<T>> RMBackup = new List<RanMod<T>>();
        /// <summary> 
        ///进行一次抽取,并返回抽到的物品,如果奖池中没有物品,则会返回null
        /// </summary>
        /// <param name="newProp">如果需要在抽取之后,更改下次抽到该物品的抽到概率,则修改此项为大于-1的值</param>
        /// <returns></returns>
        public RanMod<T> GetItem(int newProp = -1)
        {
            int[] ints = new int[RM.Count];
            for (int i = 0; i < RM.Count; i++)
            {
                if (i != 0) ints[i] = ints[i - 1] + RM[i].Prop;
                else ints[i] = RM[i].Prop;
            }
            int r = UnityEngine.Random.Range(0, ints[ints.Length - 1]);
            for (int i = 0; i < ints.Length; i++)
            {
                if (ints[i] > r)
                {
                    if (newProp > -1)
                    {
                        RM[i].Prop = newProp;
                    }
                    RM[i].RandomNum++;
                    return RM[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 添加一个随机物品,prop为抽到该物品的几率,obj为物品本身
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="obj"></param>
        public void Add(int prop, T obj)
        {
            RM.Add(new RanMod<T>(prop, obj));
            RMBackup.Add(new RanMod<T>(prop, obj));
        }

        /// <summary>
        /// 重置抽取器
        /// </summary>
        public void ReSet()
        {
            for (int i = 0; i < RM.Count; i++)
            {
                RM[i].Prop = RMBackup[i].Prop;
            }
        }
    }
}
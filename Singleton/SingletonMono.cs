using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T: SingletonMono<T>
{
    private static T instance;
	private static object S_objectlock = new object();
	public static T Instance
    {
        get
        {
            if (instance==null)
            {
				object obj;
				//防止在同一时间有2个线程创建实例
				//Monitor能够对值类型进行加锁，实质上是Monitor.Enter(object)对值类型装箱
				Monitor.Enter(obj = S_objectlock);
				try
				{
                    instance = new GameObject(typeof(T).Name).AddComponent<T>();
				}
				finally
				{
					Monitor.Exit(obj);
				}
            }

            return instance;
        }

    }
    public virtual void Awake()
    {
        instance = this as T;
    }
	//仅限当前类和子类中访问
	protected SingletonMono() { }
}

using System;
using System.Collections;
using UnityEngine;

public abstract class AutoSingleton<T> where T : AutoSingleton<T>
{

	private static volatile T instance;//防止指令重排序，
	private static readonly object _lock = new object();

	public static T Instance
	{
		get
		{
            //性能优化避免不必要锁竞争
            if (instance == null)
			{
				lock (_lock)
				{
                    //确保线程安全
                    if (instance == null)
					{
                        instance = Activator.CreateInstance<T>();
                        //instance = new T();//使用new()后，可以使用，但继承的类无法使用private/protected限制构造函数
                    }
                }
			}
			return instance;
		}
	}
}
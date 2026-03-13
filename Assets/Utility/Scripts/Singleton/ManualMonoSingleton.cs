using System;
using UnityEngine;

public class ManualMonoSingleton<T> : MonoBehaviour where T : ManualMonoSingleton<T>
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null) throw new NullReferenceException($"单例类{typeof(T)}尚未初始化，请先调用SetInstance方法设置实例。");
            return instance;
		}
	}

	public static void SetInstance(T _instance, bool dontDestroyOnLoad = false)
	{
		if (_instance == null) throw new Exception($"设置实例不能为空。");
        if (instance != null) throw new Exception($"单例类{typeof(T)}已初始化，不能重复设置实例。");
        instance = _instance;
        if (dontDestroyOnLoad) DontDestroyOnLoad(instance.gameObject);
    }
}
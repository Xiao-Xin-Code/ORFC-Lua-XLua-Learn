using UnityEngine;

public class AutoMonoSingleton<T> : MonoBehaviour where T : AutoMonoSingleton<T>
{

	private static T instance;
	private static readonly object lockObj = new object();

	public static T Instance
	{
		get
		{
			if (instance == null) 
			{
				lock (lockObj)
				{
                    // 1. 首先尝试查找所有实例（包括隐藏的）
                    instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }
			} 
			return instance;
		}
	}

}
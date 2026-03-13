using System.Collections.Generic;
using UnityEngine;

public class MonoPool<T> : IPool<T> where T : MonoBehaviour
{
    private Queue<T> _pool = new Queue<T>();
    private HashSet<T> _activeItems = new HashSet<T>();
    private T _prefab;
    private Transform _parent;
    private int _expansionFrequency = 1;

    public MonoPool(T prefab, Transform parent = null, int initialsize = 5, int expansionFrequency = 5)
    {
        _prefab = prefab;
        _parent = parent;
        _expansionFrequency = expansionFrequency <= 1 ? 1 : expansionFrequency;
		ExpandPool(initialsize);
	}

    private void ExpandPool(int count)
    {
		for (int i = 0; i < count; i++)
		{
			T obj = GameObject.Instantiate(_prefab, _parent);
			obj.gameObject.SetActive(false);
			_pool.Enqueue(obj);
		}
	}

	public T Get()
    {
        if(_pool.Count == 0)
        {
            ExpandPool(_expansionFrequency);
		}
        T obj = _pool.Dequeue();
        obj.gameObject.SetActive(true);
        _activeItems.Add(obj);
        return obj;
    }

    public void Recycle(T recycleT)
    {
        if (recycleT == null) return;
        if (_activeItems.Contains(recycleT))
        {
            recycleT.gameObject.SetActive(false);
            recycleT.transform.SetParent(_parent);
            _activeItems.Remove(recycleT);
            _pool.Enqueue(recycleT);
        }
    }

    public void RecycleAll()
    {
        var itemsToRecycle = new HashSet<T>(_activeItems);
        _activeItems.Clear();
        foreach (var item in itemsToRecycle) 
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(_parent);
            _pool.Enqueue(item);
        }
    }
}
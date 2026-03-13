using System.Collections.Generic;

public class ObjectPool<T> : IPool<T> where T : class, new()
{
    private Queue<T> _pool = new Queue<T>();
    private HashSet<T> _activeItems = new HashSet<T>();
    private int _expansionFrequency = 1;

    public ObjectPool(int initialSize = 5,int expansionFrequency = 5)
    {
        _expansionFrequency = expansionFrequency <= 1 ? 1 : expansionFrequency;
        ExpandPool(initialSize);
    }

    private void ExpandPool(int count)
    {
        for(int i = 0; i < count; i++)
        {
            T obj = new T();
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
        _activeItems.Add(obj);
        return obj;
    }

    public void Recycle(T recycleT)
    {
        if (recycleT == null) return;
        if (_activeItems.Contains(recycleT))
        {
            _activeItems.Remove(recycleT);
            _pool.Enqueue(recycleT);
        }
    }

    public void RecycleAll()
    {
        var itemsToRecycle = _activeItems;
		_activeItems.Clear();
		foreach (var item in _activeItems)
        {
            _pool.Enqueue(item);
        }
        
    }
}
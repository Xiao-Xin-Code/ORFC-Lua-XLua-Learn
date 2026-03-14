using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TagClassification : MonoBehaviour
{
    [SerializeField]
    TMP_Text name_Text;
    [SerializeField]
    InfiniteScrollView tag_scroll;

    public InfiniteScrollView Scroll { get => tag_scroll; }

    MonoPool<TagUnit> pool;

    public void SetName(string name)
    {
        name_Text.text = name;
    }


    public void Init()
    {
        if (pool == null)
        {
            pool = new MonoPool<TagUnit>(LuaManager.Instance.TagPrefab);
        }
        pool.RecycleAll();

        Scroll.RegisterGet(() => pool.Get());
        Scroll.RegisterBeforeUpdate(() => pool.RecycleAll());
        //Scroll.RegisterEndUpdate();
    }

    public void OnDeInit()
    {
        if (pool != null) pool.RecycleAll();
    }
}

using UnityEngine;

public class RoleClassification : MonoBehaviour
{
	[InspectorName("TagInfo")][SerializeField] InfiniteScrollView tag_scroll;
	[InspectorName("RoleInfo")][SerializeField] InfiniteScrollView role_scroll;


	public InfiniteScrollView Tag_Scroll => tag_scroll;
	public InfiniteScrollView Role_Scroll => role_scroll;



	MonoPool<TagElement> tag_pool;
	MonoPool<RoleUnit> role_pool;




	public void Init()
	{
		if(tag_pool == null)
		{
			tag_pool = new MonoPool<TagElement>(LuaManager.Instance.TagElementPrefab);
		}
		tag_pool.RecycleAll();

		if(role_pool == null)
		{
			role_pool = new MonoPool<RoleUnit>(LuaManager.Instance.RolePrefab);
		}
		role_pool.RecycleAll();


		tag_scroll.RegisterGet(() => tag_pool.Get());
		tag_scroll.RegisterBeforeUpdate(() => tag_pool.RecycleAll());

		role_scroll.RegisterGet(() => role_pool.Get());
		role_scroll.RegisterBeforeUpdate(() => role_pool.RecycleAll());
	}

	public void OnDeInit()
	{
		if (tag_pool != null) tag_pool.RecycleAll();
		if (role_pool != null) role_pool.RecycleAll();
	}


}

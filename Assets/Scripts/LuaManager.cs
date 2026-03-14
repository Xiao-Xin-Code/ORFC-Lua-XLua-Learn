using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class LuaManager : AutoMonoSingleton<LuaManager>
{
	public Transform tagGroupParent;
	public Transform roleGroupParent;

	#region 预制件

	TagUnit tagprefab;
	RoleUnit rolePrefab;
	TagElement tagElementPrefab;
	TagClassification tagGroupPrefab;
	RoleClassification roleGroupPrefab;


	public TagUnit TagPrefab => tagprefab;
	public RoleUnit RolePrefab => rolePrefab;
	public TagElement TagElementPrefab => tagElementPrefab;
	public TagClassification TagGroupPrefab => tagGroupPrefab;
	public RoleClassification RoleGroupPrefab => roleGroupPrefab;


	#endregion

	#region Pool

	MonoPool<TagClassification> tag_group_pool;
	MonoPool<RoleClassification> role_group_pool;

	#endregion


	Transform pool_root;
	




	private LuaEnv luaEnv;
    public List<int> tagCombines = new List<int>();

    void Start()
    {
		pool_root = new GameObject("Pools").transform;
		Transform pool_tag_group = new GameObject("TagGroup").transform;
		Transform pool_role_group = new GameObject("RoleGroup").transform;

		pool_tag_group.SetParent(pool_root);
		pool_role_group.SetParent(pool_root);

		tagprefab = Resources.Load<TagUnit>("tag_unit");
		rolePrefab = Resources.Load<RoleUnit>("role_unit");
		tagElementPrefab = Resources.Load<TagElement>("tag_element");
		tagGroupPrefab = Resources.Load<TagClassification>("tag_group");
		roleGroupPrefab = Resources.Load<RoleClassification>("role_group");

		tag_group_pool = new MonoPool<TagClassification>(tagGroupPrefab, pool_tag_group);
		role_group_pool = new MonoPool<RoleClassification>(roleGroupPrefab, pool_role_group);
		

		luaEnv = new LuaEnv();

        luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Data/tagsdata.lua"}')");
        luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Data/rolesdata.lua"}')");
        luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Config/tagsgroupconfig.lua"}')");
        luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Config/matchtagrolesconfig.lua"}')");
        luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Config/colorconfig.lua"}')");
		luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Logic/orfclogic.lua"}')");

		SetTagGroup();
	}



	public void ReLoadLua()
	{
		if(luaEnv != null)
		{
			luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Data/tagsdata.lua"}')");
			luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Data/rolesdata.lua"}')");
			luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Config/tagsgroupconfig.lua"}')");
			luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Config/matchtagrolesconfig.lua"}')");
			luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Config/colorconfig.lua"}')");
			luaEnv.DoString($"dofile('{Application.streamingAssetsPath + "/Lua/Logic/orfclogic.lua"}')");
		}
	}


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			ReLoadLua();
		}
	}




	void OnDestroy()
    {
        if (luaEnv != null)
        {
            luaEnv.Dispose(); // 释放环境
            luaEnv = null;
        }
    }



	public bool IsSelect(int tag_id)
	{
		return tagCombines.Contains(tag_id);
	}


	private void SetTagGroup()
	{
		tag_group_pool.RecycleAll();

		LuaTable tags = luaEnv.Global.Get<LuaTable>("tags");
		LuaTable tagsgroup = luaEnv.Global.Get<LuaTable>("tagsgroup");

		tagsgroup.ForEach<int, LuaTable>((index, taggroup) =>
		{
			string grouname = taggroup.Get<string>("name");
			LuaTable tagids = taggroup.Get<LuaTable>("tags");

			//创建TagGroup
			TagClassification tagClassification = tag_group_pool.Get();
			tagClassification.SetName(grouname);
			tagClassification.transform.SetParent(tagGroupParent);

			tagClassification.Init();
			tagClassification.Scroll.SetDatas(tagids.Cast<List<int>>());
		});

		LayoutRebuilder.ForceRebuildLayoutImmediate(tagGroupParent.GetComponent<RectTransform>());
	}

    public void Search_Roles()
    {
		role_group_pool.RecycleAll();

		LuaTable tags = luaEnv.Global.Get<LuaTable>("tags");
		LuaTable roles = luaEnv.Global.Get<LuaTable>("roles");
		object[] result = ExecuteLuaFunction("search_role_with_tag", ConverToTable(tagCombines));
		LuaTable table = result[0] as LuaTable;

		//遍历所有返回数据{ { { a},{ b} } }->{ { a},{ b} }
		table.ForEach<object, LuaTable>((k, v) =>
		{
			RoleClassification roleClassification = role_group_pool.Get();
			roleClassification.transform.SetParent(roleGroupParent);
			roleClassification.Init();

			int index = 1;

			//遍历每组数据中的tag信息与role信息 {a},{b}
			v.ForEach<object, LuaTable>((m, n) =>
			{
				if(index == 1)
				{
					roleClassification.Tag_Scroll.SetDatas(n.Cast<List<int>>());
				}
				else
				{
					roleClassification.Role_Scroll.SetDatas(n.Cast<List<int>>());
				}
				index++;
			});
		});

		LayoutRebuilder.ForceRebuildLayoutImmediate(roleGroupParent.GetComponent<RectTransform>());
	}

	public Color GetTagColor(int tagid, string color)
	{
		LuaTable tags = luaEnv.Global.Get<LuaTable>("tags");
		LuaTable tagcolors = luaEnv.Global.Get<LuaTable>("tagcolors");
		int weight = tags.Get<int, LuaTable>(tagid).Get<int>("weight");
        LuaTable colors = tagcolors.Get<int, LuaTable>(weight);
		LuaTable colorTable = colors.Get<LuaTable>(color);
		return new Color(colorTable.Get<float>("r"), colorTable.Get<float>("g"), colorTable.Get<float>("b"), colorTable.Get<float>("a"));
	}


	public LuaTable GetTagInfo(int tagid)
	{
		LuaTable tags = luaEnv.Global.Get<LuaTable>("tags");
		LuaTable taginfo = tags.Get<int, LuaTable>(tagid);
		return taginfo;
	}

	public LuaTable GetRoleInfo(int roleid)
	{
		LuaTable roles = luaEnv.Global.Get<LuaTable>("roles");
		LuaTable roleinfo = roles.Get<int, LuaTable>(roleid);
		return roleinfo;
	}


	private LuaTable ConverToTable<T>(IEnumerable<T> values)
    {
        if (luaEnv != null) 
        {
			LuaTable table = luaEnv.NewTable();
			int index = 1;
			foreach (var item in values)
			{
				table.Set(index, item);
				index++;
			}
			return table;
		}
        return null;
    }

    private object[] ExecuteLuaFunction(string function, params object[] a)
    {
        if (luaEnv != null)
        {
			LuaFunction lauFunction = luaEnv.Global.Get<LuaFunction>(function);
			object[] functionResult = lauFunction.Call(a);
			return functionResult;
		}
        return null;
	}



	private void UpdateTagParentSize()
	{

		 

	}

	private void UpdateRoleParnetSize()
	{

	}



}

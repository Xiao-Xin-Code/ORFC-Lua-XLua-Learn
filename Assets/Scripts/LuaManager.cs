using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class LuaManager : AutoMonoSingleton<LuaManager>
{
	public Transform tagGroupParent;
	public Transform roleGroupParent;

	TagUnit tagprefab;
	RoleUnit rolePrefab;
	TagClassification tagGroupPrefab;
	RoleClassification roleGroupPrefab;

    private MonoPool<TagUnit> tag_pool;
    private MonoPool<RoleUnit> role_pool;
	private MonoPool<TagClassification> tag_group_pool;
    private MonoPool<RoleClassification> role_group_pool;

	private LuaEnv luaEnv;
    public List<int> tagCombines = new List<int>();

    void Start()
    {
		Transform pool_root = new GameObject("Pools").transform;
		Transform pool_tag_unit = new GameObject("Tags").transform;
		Transform pool_role_unit = new GameObject("Roles").transform;
		Transform pool_tag_group = new GameObject("TagGroup").transform;
		Transform pool_role_group = new GameObject("RoleGroup").transform;
		pool_tag_unit.SetParent(pool_root);
		pool_role_unit.SetParent(pool_root);
		pool_tag_group.SetParent(pool_root);
		pool_role_group.SetParent(pool_root);

		tagprefab = Resources.Load<TagUnit>("TagUnit");
		rolePrefab = Resources.Load<RoleUnit>("RoleUnit");
		tagGroupPrefab = Resources.Load<TagClassification>("Tag_Group");
		roleGroupPrefab = Resources.Load<RoleClassification>("Role_Group");

		tag_pool = new MonoPool<TagUnit>(tagprefab, pool_tag_unit);
		role_pool = new MonoPool<RoleUnit>(rolePrefab, pool_role_unit);
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


	private void SetTagGroup()
	{
		tag_pool.RecycleAll();
		tag_group_pool.RecycleAll();

		LuaTable tags = luaEnv.Global.Get<LuaTable>("tags");
		LuaTable tagsgroup = luaEnv.Global.Get<LuaTable>("tagsgroup");
		//Debug.Log(tagsgroup == null);
		tagsgroup.ForEach<int, LuaTable>((index, taggroup) =>
		{
			string grouname = taggroup.Get<string>("name");
			LuaTable tagids = taggroup.Get<LuaTable>("tags");
			//创建TagGroup
			TagClassification tagClassification = tag_group_pool.Get();
			tagClassification.SetName(grouname);
			tagClassification.transform.SetParent(tagGroupParent);
			tagids.ForEach<int, int>((i, id) =>
			{
				LuaTable tagInfo = tags.Get<int, LuaTable>(id);
				TagUnit unit = tag_pool.Get();
				unit.SetName(tagInfo.Get<string>("name"));
				unit.SetID(id);
				unit.SetIsOn(false);
				tagClassification.SetTagLocation(unit.transform);
			});
		});

		LayoutRebuilder.ForceRebuildLayoutImmediate(tagGroupParent.GetComponent<RectTransform>());
	}

    public void Search_Roles()
    {
		role_pool.RecycleAll();
		role_group_pool.RecycleAll();

		LuaTable roles = luaEnv.Global.Get<LuaTable>("roles");
		object[] result = ExecuteLuaFunction("search_role_with_tag", ConverToTable(tagCombines));
		LuaTable table = result[0] as LuaTable;
		//遍历所有返回数据{ { { a},{ b} } }->{ { a},{ b} }
		table.ForEach<object, object>((k, v) =>
		{
			RoleClassification roleClassification = role_group_pool.Get();
			roleClassification.transform.SetParent(roleGroupParent);
			LuaTable table = v as LuaTable;
			int index = 1;
			//遍历每组数据中的tag信息与role信息 {a},{b}
			table.ForEach<object, object>((m, n) =>
			{
				string result = "";
				LuaTable table = n as LuaTable;
				table.ForEach<int, object>((i, j) =>
				{
					if (index == 1)
					{
						result += j;
					}
					else
					{
						int id = (int)(Int64)j;
						result += j;
						LuaTable roleInfo = roles.Get<int, LuaTable>(id);
						RoleUnit role = role_pool.Get();
						role.SetID(id);
						role.SetName(roleInfo.Get<string>("name"));
						roleClassification.SetRoleLocation(role.transform);
					}
				});

				if (index == 1) roleClassification.SetName(result);

				//Debug.Log(result);
				index++;
			});
		});

		LayoutRebuilder.ForceRebuildLayoutImmediate(roleGroupParent.GetComponent<RectTransform>());
	}

	public Color GetTagColor(int tagid, string color)
	{
		LuaTable roles = luaEnv.Global.Get<LuaTable>("tagcolors");
		LuaTable colors = roles.Get<int, LuaTable>(tagid);
		LuaTable colorTable = colors.Get<LuaTable>(color);
		return new Color(colorTable.Get<float>("r"), colorTable.Get<float>("g"), colorTable.Get<float>("b"), colorTable.Get<float>("a"));
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
}

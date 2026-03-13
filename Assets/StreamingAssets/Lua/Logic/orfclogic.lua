--获取到所有tag组合与其对应的所有roles
function search_role_with_tag(tagids)
	--tag 的所有组合方式
	local tagcombines = gettagscombines(tagids)
	local resultroles = {}
	for x,y in ipairs(tagcombines) do
		table.sort(y,comparewith_tags)
		local target = {}
		one = true
		--tagids
		for i,j in pairs(y) do
			if one then
				if tagtoroles[j] ~= nil then
					target = tagtoroles[j]
				else
					return nil
				end
				one = false
			else
				--判断指定tag是否存在对应的role table
				if tagtoroles[j] ~= nil then
					--存在，需要与原本的target取重
					local temp = {}
					for m,n in pairs(tagtoroles[j]) do
						if target[m] ~= nil then
							--存在相同key
							temp[m] = n
						end
					end
					target = temp
				else 
					--不存在，直接nil
					return nil
				end
			end
		end
		--添加每个tag组合与符合这个组合的roles
		if target ~= nil and next(target) ~= nil then
			local target_ids = {}
			for k,v in pairs(target) do
				table.insert(target_ids,k)
			end
			table.sort(target_ids,comparewith_roleweight)
			table.insert(resultroles,{y,target_ids})
		end
	end
	return resultroles
end

--获取tags组合
function gettagscombines(tagids)
	local tagcombines = {}
	for i = 1,#tagids do
		local result = {tagids[i]}
		table.insert(tagcombines,result)
		tagsiterate(i + 1,tagids,tagcombines,result)
	end
	table.sort(tagcombines,comparewith_tagcombines)
	return tagcombines		
end

--tags 迭代
function tagsiterate(index,tagids,tagcombines,result)	
	for i = index,#tagids do
		local temp = copytable(result)
		table.insert(temp,tagids[i])
		table.insert(tagcombines,temp)
		tagsiterate(i + 1,tagids,tagcombines,temp)
	end
end

function copytable(resource)
	local clone_table = {}
	for i,v in ipairs(resource) do
		table.insert(clone_table,v)
	end
	return clone_table
end



--以Tag的权重比较
function comparewith_tagweight(a,b)
	return tags[a].weight > tags[b].weight
end

--以角色权重比较
function comparewith_roleweight(a,b)
	return roles[a].weight > roles[b].weight
end

--tagcombines 比较器
function comparewith_tagcombines(a,b)
	local a_weight = 0
	for i,v in pairs(a) do
		a_weight = a_weight + tags[v].weight
	end
	local b_weight = 0
	for i,v in pairs(b) do
		b_weight = b_weight + tags[v].weight
	end
	if a_weight ~= b_weight then
		return a_weight > b_weight
	else
		return #a > #b
	end	
end



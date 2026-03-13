function convert(s,numrows)
	local dir = 1
	local curindex = 1
	local strs
	for i = 1,#s do
		strs[curindex] = (strs[curindex] or "") .. s:sub(i,i)
		if curindex == numrows then
			dir = -1
		elseif curindex == 1 then
			dir = 1
		end
		curindex = curindex + dir
	end
	return table.concat(strs)
end
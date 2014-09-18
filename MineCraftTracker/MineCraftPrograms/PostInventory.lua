function PrintTable(table)
	for key, value in pairs(table) do 
		print(key .. " = " .. tostring(value)) 
	end
end

function SerializeInventory(side)
	local chest = peripheral.wrap(side)
	local stacks = chest.getAllStacks()
	local totals = SumInventory(stacks)
	PrintTable(totals)
	local result = ''
	for key, value in pairs(totals) do
		if string.len(result) > 0 then result = result .. '&' end
		result = result .. key .. '=' .. value 
	end
	return result
end

function SumInventory(stacks)
	local totals = {}
	for key, value in pairs(stacks) do
                if totals[value.name] == nil then
                        totals[value.name] = value.qty
		else
			totals[value.name] = totals[value.name] + value.qty
                end
	end
	return totals
end

function PostChestContents(value)
	local content = SerializeInventory(value)
	print(content)
	local headers = { ["content-type"] = "application/x-www-form-urlencoded" }
	local response = http.request ('http://yavorg-droid.azure-mobile.net/api/updateinventory', content, headers)
	print(response)
end

function FindAdjacentTrappedChest()
	local peripheralNames = peripheral.getNames()
	for key, value in pairs(peripheralNames) do
		local type = peripheral.getType(value)
		local match = string.match(type, "container_chest")
		if match then
			return value
		end
	end
end

chestDirection = FindAdjacentTrappedChest()

print("Starting to monitor chest...")
while true do
	local event = os.pullEvent("redstone")
	if (redstone.getInput(chestDirection)) then
		--Got a signal from the chest
		print("Chest opened at " .. os.time())
		chestOpen = true
		while chestOpen do
			local event = os.pullEvent("redstone")
			if (not redstone.getInput(chestDirection)) then
				print("Chest closed at " .. os.time())
				chestOpen = false
			end
		end

		-- Send the final chest contents to the Mobile Service
		PostChestContents(chestDirection)
	end
end
-- Lua 核心组件声明， 通常不需要改动

UAudioSource = UnityEngine.AudioSource
Time = UnityEngine.Time
UGameObject = UnityEngine.GameObject
UObject = UnityEngine.Object

class = require("core/middleclass")
lua_object = require("core/lua_object")
require("core/functions")
require("core/stack")
JSON = require("core/json")
bigint = require("core/bigint")

-------------------------- 扩展全局方法

-- （解决回调参数个数不一致的问题）
function callback_bridge(obj, listenFunc)
    return function (...) 
                return listenFunc(obj, ...) 
            end
end

-- 复制Lua Table
function clone(object)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local new_table = {}
        lookup_table[object] = new_table
        for key, value in pairs(object) do
            new_table[_copy(key)] = _copy(value)
        end
        return setmetatable(new_table, getmetatable(object))
    end
    return _copy(object)
end

-- 移除Table中某个元素, 仅支持有序table
function table_remove(myTable,item)
	
	-- 判空
    if myTable == nil then
        return false
    end

    local len = #myTable
    if len > 0 then
        for i = 1, len do
            if myTable[i] == item then
                table.remove(myTable, i)
                return true
            end
        end
    end
    return false
end

-- 取出Table中第一个元素, 仅支持有序table
function table_shift(myTable)
	
	-- 判空
    if myTable == nil then
        return nil
    end

    local len = #myTable
    if len > 0 then
        local item = myTable[1]
        table.remove(myTable, 1)
        return item
    end
    return nil
end

-- 强转数字类型
function convert_to_number(userdata)
    return tonumber(tostring(userdata))
end

---数字转化为中文大写
function number_to_ChineseStr(num)
    local zhChar = {"壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖"}
    local places = {"", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟"}
    if nil == tonumber(num) then
        return ''
    end
    if tonumber(num)<0 then
        return ''
    end
    local numStr = tostring(num)
    local len = string.len(numStr)
    local str = ''
    local has0 = false
    local count=0
    for i = 1, len do
        local n = tonumber(string.sub(numStr,i,i))
        local p = len - i + 1
        if p<=8 and n==0 and p>=5 then
            count=count+1
        end
        if n > 0 and has0 == true then --连续多个零只显示一个
            str = str .. '零'
            has0 = false
        end
        if p % 4 == 2 and n == 1 then --十位数如果是首位则不显示一十这样的
            if len > p then
                str = str .. zhChar[n]
            end
            str = str .. places[p]
        elseif n > 0 then
            str = str .. zhChar[n]
            str = str .. places[p]
        elseif n == 0 then
            if p % 4 == 1 then --各位是零则补单位
                if count==4 then
                    str = str .. places[1]
                else
                    str = str .. places[p]
                end
            else
                has0 = true
            end
        end
    end
    return str
end
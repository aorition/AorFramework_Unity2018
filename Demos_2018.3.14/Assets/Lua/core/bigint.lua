-- 大数运算 目前只实现的加减乘运算

local bigint = class("bigint")
local mod = 10000

function bigint.show(a)
    print(bigint.get(a))
end

function bigint.get(a)
    s = {a[#a]}
    for i=#a-1, 1, -1 do
        table.insert(s, string.format("%04d", a[i]))
    end
    return table.concat(s, "")
end

function bigint.create(s)
    if s["xyBitInt"] == true then return s end
    n, t, a = math.floor(#s/4), 1, {}
    a["xyBitInt"] = true
    if #s%4 ~= 0 then a[n + 1], t = tonumber(string.sub(s, 1, #s%4), 10), #s%4 + 1 end
    for i = n, 1, -1 do a[i], t= tonumber(string.sub(s, t, t + 3), 10), t + 4 end
    return a
end

function bigint.add(a, b)
    a, b, c, t = bigint.create(a), bigint.create(b), bigint.create("0"), 0
    for i = 1, math.max(#a,#b) do
        t = t + (a[i] or 0) + (b[i] or 0)
        c[i], t = t%mod, math.floor(t/mod)
    end
    while t ~= 0 do c[#c + 1], t = t%mod, math.floor(t/mod) end
    return c
end

function bigint.sub(a, b)
    a, b, c, t = bigint.create(a), bigint.create(b), bigint.create("0"), 0
    for i = 1, #a do
        c[i] = a[i] - t - (b[i] or 0)
        if c[i] < 0 then t, c[i] = 1, c[i] + mod  else t = 0 end
    end
    return c
end
function bigint.by(a, b)
    a, b, c, t = bigint.create(a), bigint.create(b), bigint.create("0"), 0
    for i = 1, #a do
        for j = 1, #b do
            t = t + (c[i + j - 1] or 0) + a[i] * b[j]
            c[i + j - 1], t = t%mod, math.floor(t / mod)
        end
        if t ~= 0 then c[i + #b], t = t + (c[i + #b] or 0), 0 end
    end
    return c
end

return bigint
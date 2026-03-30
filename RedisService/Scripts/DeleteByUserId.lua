local userKey = @key
local tokens = redis.call("ZRANGE", userKey, 0, -1)

if #tokens == 0 then
    redis.call("DEL", userKey)
    return 0
end

local keys = {}
for i, token in ipairs(tokens) do
    keys[i] = "refresh:" .. token
end

keys[#keys + 1] = userKey
return redis.call("DEL", unpack(keys))

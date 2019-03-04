local monitor = peripheral.wrap("top")
local width, height = monitor.getSize()

local url = "http://internal.romanport.com:43301/?width="..width.."&height="..height
local reply = http.get(url)
local data = reply.readAll()
reply.close()

local monitor = peripheral.wrap("top")
monitor.clear()
monitor.setCursorPos(1,1)

local y = 0
while y < height do
local d = string.sub(data, (y*width)+1)
monitor.setCursorPos(1, y + 1)
monitor.blit(d, d, d)
y = y + 1
end
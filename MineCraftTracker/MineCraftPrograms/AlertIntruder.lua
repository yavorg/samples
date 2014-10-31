function CallMobileServices(m)
        http.post("http://yavorg-droid.azure-mobile.net/api/notifyallusers", "message="..m)
        print("http request sent with message: "..m)
end
 
active = false
 
while true do
        local event = os.pullEvent("redstone")
        active = not active
        if active then
                CallMobileServices("INTRUDER ALERT!")
        end
end
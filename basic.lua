function OnP(user, chan, msg)

if msg == "@sayhi" then
SendMsg(chan, "Hello!!")
end

end

RegisterHook("OnPublicMessage", "OnP")
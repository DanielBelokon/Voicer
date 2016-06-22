
namespace Voicer.Common.Net
{
    public enum Messages : short
    {
        CONNECT = 1,
        KEEPALIVE,
        DISCONNECT,
        RECIEVED,
        CONNECTED,
        GETUSERS,
        SHUTDOWN,
        SWAPCHANNEL,    // Server to notify other users of user swapping channel
        CHAT,           //Send/Recieve chat message
        VOICE,          //Send/Recieve voice packet
        JOINCHANNEL,    // User requests to join channel, server to notify user swapped channel
        CONNECTCHANNEL,
        MOVE,           //Move a user from channel to channel (forced by a second user)
        BAN,            //Ban a user from the server
        KICK,           // kick user from the server
        SETKEY,
        GETKEY,
        SERVERMESSAGE,
        SETADMIN,
    };
}

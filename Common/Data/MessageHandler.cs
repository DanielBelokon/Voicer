using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voicer.Common.Data
{
    public class MessageHandler
    {
        private Dictionary<short, Delegate> messageHandler = new Dictionary<short, Delegate>();

        public enum Messages : short
        {
            // Client
            CONNECT = 1,
            KEEPALIVE = 2,
            DISCONNECT = 3,
            RECIEVED = 4,

            // Server
            CONNECTED = 5,
            GETUSERS = 6,
            SHUTDOWN = 8,

            // Shared
            CHAT = 9, //Send/Recieve chat message
            VOICE = 10, //Send/Recieve voice packet
            JOINCHANNEL, // Switch to a channel
            MOVE, //Move a user from channel to channel (forced by a second user)
            BAN, //Ban a user from the server
            KICK, // kick user from the server
            CHANNEL,
            SETKEY,
            NEWKEY,
            GETKEY,
            SERVERMESSAGE,
            SETADMIN,
        };

        public enum ChannelCommands : short
        {
            DELETE,
            RENAME,
            EDIT,
        };

        public void AddMessageHandler(Messages messageEnum, Delegate function)
        {
            short messageKey = (short)messageEnum;
            AddHandler(messageKey, function);
        }

        public void AddHandler(short messageKey, Delegate function)
        {
            if (messageHandler.ContainsKey(messageKey))
                messageHandler.Remove(messageKey);

            messageHandler.Add(messageKey, function);
        }

        public void RemoveHandler(Messages messageEnum)
        {
            short messageKey = (short)messageEnum;

            if (messageHandler.ContainsKey(messageKey))
                messageHandler.Remove(messageKey);
        }

        public void HandleMessage(Messages messageEnum, object[] param)
        {
            short messagekey = (short)messageEnum;
            if (messageHandler.ContainsKey(messagekey))
            {
                Delegate function;
                messageHandler.TryGetValue(messagekey, out function);

                function.DynamicInvoke(param);
            }
            else throw new KeyNotFoundException("The specified message did not have an affiliated handler function.");
        }

    }
}

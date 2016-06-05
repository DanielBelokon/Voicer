using System;
using System.Collections.Generic;

namespace Voicer.Common.Data
{
    public class MessageHandler
    {
        private Dictionary<short, Delegate> messageHandler = new Dictionary<short, Delegate>();

        public enum ChannelCommands : short
        {
            DELETE,
            RENAME,
            EDIT,
        };

        public void AddHandler(short key, Delegate function)
        {
            if (messageHandler.ContainsKey(key))
                messageHandler.Remove(key);

            messageHandler.Add(key, function);
        }

        public void RemoveHandler(short key)
        {
            if (messageHandler.ContainsKey(key))
                messageHandler.Remove(key);
        }

        public void Handle(short key, params object[] parametars)
        {
            if (messageHandler.ContainsKey(key))
            {
                Delegate function;
                messageHandler.TryGetValue(key, out function);

                function.DynamicInvoke(parametars);
            }
            else throw new KeyNotFoundException("The specified message did not have an affiliated handler function.");
        }
    }
}

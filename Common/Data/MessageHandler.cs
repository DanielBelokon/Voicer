using System;
using System.Collections.Generic;

namespace Voicer.Common.Data
{
    public class MessageHandler
    {
        private Dictionary<short, Delegate> _messageHandlers = new Dictionary<short, Delegate>();

        public enum ChannelCommands : short
        {
            DELETE,
            RENAME,
            EDIT,
        };

        public void AddHandler(short key, Delegate function)
        {
            if (_messageHandlers.ContainsKey(key))
                _messageHandlers.Remove(key);

            _messageHandlers.Add(key, function);
        }

        public void RemoveHandler(short key)
        {
            if (_messageHandlers.ContainsKey(key))
                _messageHandlers.Remove(key);
        }

        public void Handle(short key, params object[] parametars)
        {
            if (_messageHandlers.ContainsKey(key))
            {
                Delegate function;
                _messageHandlers.TryGetValue(key, out function);

                function.DynamicInvoke(parametars);
            }
            else Console.WriteLine(" UNKNOWN PACKET RECIEVED...");
                
            //throw new KeyNotFoundException("The specified message did not have an affiliated handler function.");
        }
    }
}

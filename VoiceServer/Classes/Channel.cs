using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voicer.Common.Data;

namespace VoiceServer
{
    public class Channel
    {

        private static short CHANNEL_NEXTID = 1;

        public short channelId;
        public string channelName;
        public List<ServerClient> clients;

        private MessageHandler commandHandler;

        public delegate void ClientJoinedChannelDelegate(Channel channel, ServerClient client);
        public ClientJoinedChannelDelegate ClientJoinedChannel;

        private int joinPower;
        private int talkPower;

        private int channelLimit;

        public Channel(string name = null, int limit = 0, int joinPower = 0, int talkPower = 0)
        {
            commandHandler = new MessageHandler();
            commandHandler.AddHandler((short)MessageHandler.ChannelCommands.DELETE, new Action<byte[]>(Command_Delete));
            commandHandler.AddHandler((short)MessageHandler.ChannelCommands.RENAME, new Action<byte[]>(Command_Rename));
            commandHandler.AddHandler((short)MessageHandler.ChannelCommands.EDIT, new Action<byte[]>(Command_Edit));

            clients = new List<ServerClient>();
            this.channelId = CHANNEL_NEXTID;
            CHANNEL_NEXTID++;

            this.joinPower = joinPower;
            this.talkPower = talkPower;

            if (name != null)
                this.channelName = Data.MakeSafe(name, "Channel " + this.channelId);
            else
                this.channelName = "Channel " + this.channelId;

            this.channelLimit = limit;
        }

        public void Command_Delete(byte[] data)
        {

        }

        public void Command_Rename(byte[] data)
        {

        }

        public void Command_Edit(byte[] data)
        {

        }

        public void Join(ServerClient client)
        {
            if ((clients.Count >= channelLimit || channelLimit == 0) && client.joinPower > this.joinPower)
            {
                clients.Add(client);
                client.Send(MessageHandler.Messages.JOINCHANNEL, BitConverter.GetBytes(this.channelId));
                if (ClientJoinedChannel != null)
                    ClientJoinedChannel(this, client);
            }
        }

        public void Send(MessageHandler.Messages message, byte[] data, short filterId)
        {
            foreach (ServerClient client in clients)
            {
                if (client.ID != filterId)
                    client.Send(message, data);
            }
        }

        public void Kick(short clientId)
        {

        }

        public void Join(short clientId)
        {

        }

        public void FindClient()
        {

        }

        public override string ToString()
        {
            return (this.channelName + "," + this.channelId);
        }

        internal void Leave(ServerClient serverClient)
        {
            clients.Remove(serverClient);
        }
    }
}

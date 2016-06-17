using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoicerClient.ServerObjects
{
    public class Server : IDisposable
    {
        static short LAST_ID = 0;

        private List<Channel> channels;
        private string name;
        private short id;

        public EventHandler UserlistUpdate;

        public List<Channel> Channels
        {
            get
            {
                return channels;
            }

            set
            {
                channels = value;
            }
        }

        public Server(List<Channel> channels = null, string name = null)
        {
            id = ++LAST_ID;
            if (channels == null)
                channels = new List<Channel>();
            this.channels = channels;
            if (name == null)
                name = "Server " + id;

            this.name = name;
        }

        public void SetChannels(List<Channel> channels)
        {
            this.channels = channels;
        }

        public void UserSwitchChannel(User user, Channel newChannel)
        {
            if (newChannel != null && user != null && channels.Contains(newChannel))
            {
                UserLeaveChannel(user);
                UserAdd(user, newChannel);
                Console.WriteLine("User added to channel : " + newChannel.ID);
                OnUserlistUpdate();
            }
            else
            {
                Console.WriteLine("Channel or user not found");
            }
        }

        public void UserLeaveChannel(User user)
        {
            Channel channel = FindUserChannel(user);
            if (channel != null)
            {
                channel.RemoveUser(user);
                OnUserlistUpdate();
            }
        }

        public void UserAdd(User user, Channel channel)
        {
            if (channels.Contains(channel))
            {
                channel.AddUser(user);
            }
        }

        public void UserAdd(User user, short channelID)
        {
            Channel channel = GetChannel(channelID);
            if (channel != null)
            {
                UserAdd(user, channel);
                OnUserlistUpdate();
            }
        }

        public void ServerAddChannel(Channel newChannel)
        {
            channels.Add(newChannel);
        }

        public void ServerRemoveChannel(Channel channel)
        {
            channels.Remove(channel);
        }

        public void ServerRemoveChannel(short channelId)
        {
            ServerRemoveChannel(GetChannel(channelId));
        }

        public User GetUser(short userID, short channelId = 0)
        {
            if (channelId > 0)
            {
                return GetChannel(channelId).GetUser(userID);
            }
            else
            {
                foreach (Channel curChannel in channels)
                {
                    User user = curChannel.GetUser(userID);
                    if (user != null)
                        return user;
                }
            }

            return null;
        }

        public Channel FindUserChannel(User user)
        {
            foreach (Channel curChannel in channels)
            {
                if (curChannel.FindUser(user))
                {
                    return curChannel;
                }
            }
            return null;
        }

        public Channel GetChannel(short channelID)
        {
            Console.WriteLine("FINDING CHANNEL: " + channelID);
            foreach (Channel curChannel in channels)
            {
                if (curChannel.ID == channelID)
                    return curChannel;
            }
            return null;
        }

        public void Dispose()
        {
            foreach (Channel curChannel in channels)
            {
                curChannel.Dispose();
            }

            channels = null;
            id = 0;
            name = null;
            
        }

        public void OnUserlistUpdate()
        {
            if (UserlistUpdate != null)
                UserlistUpdate(this, EventArgs.Empty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voicer.ServerObjects
{
    public class Channel
    {
        private List<User> users;
        private string name;
        private string description;

        private short id;

        public short ID
        {
            get
            {
                return id;
            }

            private set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            private set
            {
                name = value;
            }
        }

        public List<User> Users
        {
            get
            {
                return users;
            }

            private set
            {
                users = value;
            }
        }

        public Channel(string name, short ID, string description = "")
        {
            users = new List<User>();
            id = ID;
            this.name = name;
            this.description = description;
        }

        public void SetUsers(List<User> userList)
        {
            users = userList;
        }

        public void RemoveUser(User user)
        {
            users.Remove(user);
            users.Sort();
        }

        public void AddUser(User user)
        {
            users.Add(user);
        }

        internal void Dispose()
        {
            foreach(User curUser in users)
            {
                curUser.Dispose();
            }
            users = null;
            id = 0;
            name = null;
            description = null;
        }

        internal bool FindUser(User user)
        {
            return (users.Contains(user));
        }

        internal User GetUser(short id)
        {
            foreach(User curUser in users)
            {
                if (curUser.ID == id)
                    return curUser;
            }

            return null;
        }
    }
}

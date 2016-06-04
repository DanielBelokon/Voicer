using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voicer
{
    public class Channel
    {
        public List<User> users;
        public string name;
        public string description;

        public short ID;

        public Channel(string name, short ID, string description = "")
        {
            users = new List<User>();
            this.ID = ID;
            this.name = name;
            this.description = description;
        }



    }
}

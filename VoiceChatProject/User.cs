using System;
using Voicer.Sound;

// Represents the users present on the server (Remote clients)
namespace Voicer
{
    public class User : IDisposable
    {
        public string nickname;
        public short ID;
        public Audio clientAudio;

        public static User Empty = new User("", -1);

        public User(string nick, short ID)
        {
            nickname = nick;
            this.ID = ID;
            if (ID > 0)
            {
                clientAudio = new Audio();
                clientAudio.StartSound();
            }
        }

        public void Dispose()
        {
            if (clientAudio != null)
                clientAudio.StopSound();
        }
    }
}
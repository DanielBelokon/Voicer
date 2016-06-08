using System;
using VoicerClient.Sound;

// Represents the users present on the server (Remote clients)
namespace VoicerClient.ServerObjects
{
    public class User : IDisposable, IComparable
    {

        private short id;
        public short ID
        {
            get
            {
                return id;
            }
            internal set
            {
                id = value;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (clientAudio != null)
                    return clientAudio.IsPlaying;
                else return false;
            }
        }

        private Audio clientAudio;

        public User(string nick, short ID)
        {
            name = nick;
            id = ID;
            if (ID > 0)
            {
                clientAudio = new Audio();
            }
        }

        public void StartSound()
        {
            clientAudio.StartSound();
        }

        public void StopSound()
        {
            clientAudio.StopSound();
        }

        public void AddSound(byte[] data)
        {
            if (clientAudio.IsPlaying)
            {
                clientAudio.AddSound(data);
            }
        }

        public void Dispose()
        {
            if (clientAudio != null)
            {
                clientAudio.Dispose();
            }
            id = 0;
            name = null;
        }

        public int CompareTo(object obj)
        {
            try
            {
                User user = (User)obj;
                return name.CompareTo(user.name);
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException("Cannot compare object of type " + obj.GetType().ToString() + " to User.");
            }
        }
    }
}
using Voicer.Sound;

// Represents the users present on the server (Remote clients)
namespace Voicer
{
    public struct User
    {
        public string nickname;
        public short ID;
        public Audio clientAudio;

        public static User Empty = new User("", -1);

        public User(string nick, short ID)
        {
            nickname = nick;
            this.ID = ID;

            clientAudio = new Audio();
            clientAudio.StartSound();
        }
    }
}
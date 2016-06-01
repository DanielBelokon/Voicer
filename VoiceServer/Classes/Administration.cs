using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VoiceServer
{
    public static class Administration
    {
        const string AvailableKeyChars = "=-+abcdefghijklmnopqrstuvwsyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const string userKeyFileName = "userkeys";
        const string adminKeyFileName = "adminKeys";

        private static string serverKey;
        public static string ServerKey
        {
            get
            {
                return serverKey;
            }

            private set
            {

            }
        }

        private static  List<string> userKeys;

        public static List<string> UserKeys
        {
            get { return userKeys; }
            private set { userKeys = value; }
        }

        private static List<string> adminKeys;

        public static List<string> AdminKeys
        {
            get { return adminKeys; }
            private set { adminKeys = value; }
        }
        

        public static void LoadServerKeys()
        {
            // Load server key
            try
            {
                string[] lines = File.ReadAllLines(Environment.CurrentDirectory + "/server.txt");
                serverKey = lines.First();

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Server key not found -- generating new one");
                serverKey = GenerateKey(16);
                File.WriteAllText(Environment.CurrentDirectory + "/server.txt", serverKey);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Server key not found -- generating new one");
                serverKey = GenerateKey(16);
                File.WriteAllText(Environment.CurrentDirectory + "/server.txt", serverKey);
            }

            // Load admin list
            try
            {
                adminKeys = File.ReadAllLines(Environment.CurrentDirectory + "/" + adminKeyFileName + ".txt").ToList();

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(adminKeyFileName + " not found");
                adminKeys = new List<string>();
            }

            // Load user list
            try
            {
                userKeys = File.ReadAllLines(Environment.CurrentDirectory + "/" + userKeyFileName + ".txt").ToList();

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(userKeyFileName + " not found");
                userKeys = new List<string>();
            }
        }

        public static void SaveServerKeys()
        {
            File.WriteAllText(Environment.CurrentDirectory + "/server.txt", serverKey);
            File.WriteAllLines(Environment.CurrentDirectory + "/" + adminKeyFileName + ".txt", adminKeys);
            File.WriteAllLines(Environment.CurrentDirectory + "/" + userKeyFileName + ".txt", userKeys);
        }

        static public string AddUserKey()
        {
            string newKey = "";
            do
            {
                newKey = GenerateKey(32);

            } while (userKeys.Contains(newKey));

            userKeys.Add(newKey);     
            return newKey;
        }

        static public string GenerateKey(short length)
        {
            char[] stringChars = new char[length];
            Random random = new Random();


            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = AvailableKeyChars[random.Next(AvailableKeyChars.Length)];
            }

            return new String(stringChars);
        }

        static public void SetAdmin(string key)
        {
            if (!adminKeys.Contains(key))
                adminKeys.Add(key);
        }

        static public void RemoveAdmin(string key)
        {
            adminKeys.Remove(key);
        }

        static public bool IsAdmin(string key)
        {
            return adminKeys.Contains(key);
        }

        static public bool KeyExists(string key)
        {
            return userKeys.Contains(key);
        }
    }
}

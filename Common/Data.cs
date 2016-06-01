using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voicer.Common
{
    public static class Data
    {
        private static Dictionary<string, string> conversion = new Dictionary<string, string>()
            {
                {"|", "%c"},
                {",", "%s"},
            };

        public static string Serialize(string input)
        {
            StringBuilder output = new StringBuilder(input);

            foreach (KeyValuePair<string, string> pair in conversion)
            {
                output = output.Replace(pair.Key, pair.Value);
            }

            return output.ToString();
        }

        public static string DeSerialize(string input)
        {
            StringBuilder output = new StringBuilder(input);

            foreach (KeyValuePair<string, string> pair in conversion)
            {
                output = output.Replace(pair.Value, pair.Key);
            }

            return output.ToString();
        }

        public static string MakeSafe(string input, string onEmpty = " ")
        {
            StringBuilder output = new StringBuilder(input);

            output = output.Replace("%", "");
            

            if (output.ToString() != "")
                return output.ToString();
            else return onEmpty;
        }
    }
}

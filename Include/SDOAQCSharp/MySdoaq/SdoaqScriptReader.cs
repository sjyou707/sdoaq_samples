using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDOAQCSharp
{
    public static class SdoaqScriptReader
    {
        private delegate bool FuncTryParse<T>(string input, out T result);

        public static bool GetIntFromLineScript(string path, string token, int defaultVal, out int val)
        {
            return TryGetValueFromScript(path, token, defaultVal, out val, int.TryParse);
        }

        public static bool GetDblFromLineScript(string path, string token, double defaultVal, out double val)
        {
            return TryGetValueFromScript(path, token, defaultVal, out val, double.TryParse);
        }

        public static bool GetStringFromLineScript(string path, string token, string defaultVal, out string val)
        {
            return TryGetValueFromScript(path, token, defaultVal, out val, null);
        }

        private static bool TryGetValueFromScript<T>(string path, string token, T defaultVal, out T val, FuncTryParse<T> tryParseFunc)
        {
            val = defaultVal;

            if (string.IsNullOrEmpty(token) || System.IO.File.Exists(path) == false)
            {
                return false;
            }

            token = token.Replace(" ", "").ToUpper();
            
            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string lineData = line.Replace(" ", "").ToUpper();

                if (lineData.StartsWith("#"))
                {
                    continue;
                }
                    
                var parts = lineData.Split('=');

                if (parts.Length < 2 || parts[0] != token)
                {
                    continue;
                }
                
                var value = parts[1];

                if (tryParseFunc == null)
                {
                    val = (T)Convert.ChangeType(value, typeof(T));
                    return true;
                }
                else if (tryParseFunc(value, out val))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
            return false;
        }
    }
}

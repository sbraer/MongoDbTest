using System;
using System.Collections.Generic;
using System.IO;

namespace MongoDbTest.Services
{
    public class HelperService
    {
        private readonly Dictionary<string, string> _cacheDictionary;

        public HelperService()
        {
            _cacheDictionary = new Dictionary<string, string>();
        }

        public string GetEnvFileValue(string key, string defaultValue)
        {
            if (_cacheDictionary.ContainsKey(key))
            {
                return _cacheDictionary[key];
            }

            string returnValue = defaultValue;

            if (Environment.GetEnvironmentVariable(key+"_FILE") != null)
            {
                string filePath = Environment.GetEnvironmentVariable(key+"_FILE");
                if (File.Exists(filePath))
                {
                    returnValue = File.ReadAllText(filePath).Trim();
                }
            }
            else if (Environment.GetEnvironmentVariable(key) != null)
            {
                returnValue = Environment.GetEnvironmentVariable(key);
            }

            _cacheDictionary[key] = returnValue;            
            return returnValue;
        }
    }
}

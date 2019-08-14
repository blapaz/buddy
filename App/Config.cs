using System;
using System.IO;

namespace App
{
    public class Config
    {
        public string ExecutablePath { get; private set; }
        public string ConfigFile { get; private set; }
        public bool ShouldCompileOnly { get; private set; }
        public bool ShouldOutputCompiled { get; private set; }
        public bool ShouldSilentStart { get; private set; }

        public Config(string configFile)
        {
            ExecutablePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Default values set here, overridable via config
            ConfigFile = configFile;
            ShouldCompileOnly = false;
            ShouldOutputCompiled = false;
            ShouldSilentStart = false;

            Read();
        }

        private void Read()
        {
            if (File.Exists(ConfigFile))
            {
                string[] lines = File.ReadAllLines(ConfigFile);

                foreach (string line in lines)
                {
                    // Split by first occurance of seperator
                    string[] lineSplit = line.Split(new[] { ':' }, 2);
                    Assign(lineSplit[0], lineSplit[1]);
                }
            }
        }

        private void Assign(string name, string value)
        {
            name = name.ToLower().Trim();
            value = value.Trim();

            switch (name)
            {
                case "compile-only":
                    if (IsValidBoolean(value))
                        ShouldCompileOnly = Convert.ToBoolean(value);
                    break;
                case "output-compiled":
                    if (IsValidBoolean(value))
                        ShouldOutputCompiled = Convert.ToBoolean(value);
                    break;
                case "silent-start":
                    if (IsValidBoolean(value))
                        ShouldSilentStart = Convert.ToBoolean(value);
                    break;
                default:
                    break;
            }
        }

        public bool IsValidDirectory(string value)
        {
            if (Directory.Exists(value))
                return true;

            return false;
        }

        public bool IsValidBoolean(string value)
        {
            string val = value.ToLower();

            if (val.Equals("true") || val.Equals("false"))
                return true;

            return false;
        }
    }
}

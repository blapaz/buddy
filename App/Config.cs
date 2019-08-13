using System;
using System.IO;

namespace App
{
    class Config
    {
        public static string ExecutablePath { get; private set; }
        public static string ConfigFilePath { get; private set; }
        public static string ScriptsDirectory { get; private set; }
        public static bool ShouldCompileOnly { get; private set; }
        public static bool ShouldOutputCompiled { get; private set; }

        static Config()
        {
            ExecutablePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Default values set here, overridable via config
            ScriptsDirectory = ExecutablePath;
            ShouldCompileOnly = false;
            ShouldOutputCompiled = true;

            Read();
        }

        private static void Read()
        {
            if (File.Exists(ConfigFilePath))
            {
                string[] lines = File.ReadAllLines(ConfigFilePath);

                foreach (string line in lines)
                {
                    // Split by first occurance of seperator
                    string[] lineSplit = line.Split(new[] { ':' }, 2);
                    Assign(lineSplit[0], lineSplit[1]);
                }
            }
        }

        private static void Assign(string name, string value)
        {
            name = name.ToLower().Trim();
            value = value.Trim();

            switch (name)
            {
                case "scripts-dir":
                    if (IsValidDirectory(value))
                        ScriptsDirectory = value;
                    break;
                case "compile-only":
                    if (IsValidBoolean(value))
                        ShouldCompileOnly = Convert.ToBoolean(value);
                    break;
                case "output-compiled":
                    if (IsValidBoolean(value))
                        ShouldOutputCompiled = Convert.ToBoolean(value);
                    break;
                default:
                    break;
            }
        }

        public static bool IsValidDirectory(string value)
        {
            if (Directory.Exists(value))
                return true;

            return false;
        }

        public static bool IsValidBoolean(string value)
        {
            string val = value.ToLower();

            if (val.Equals("true") || val.Equals("false"))
                return true;

            return false;
        }
    }
}

namespace Facepunch.Utility
{
    using System;
    using System.Collections.Generic;

    public static class CommandLine
    {
        private static string commandline = string.Empty;
        private static bool initialized = false;
        private static Dictionary<string, string> switches = new Dictionary<string, string>();

        public static void Force(string val)
        {
            commandline = val;
            initialized = false;
        }

        public static string GetSwitch(string strName, string strDefault)
        {
            Initalize();
            string str = string.Empty;
            if (!switches.TryGetValue(strName, out str))
            {
                return strDefault;
            }
            return str;
        }

        public static int GetSwitchInt(string strName, int iDefault)
        {
            Initalize();
            string str = string.Empty;
            if (!switches.TryGetValue(strName, out str))
            {
                return iDefault;
            }
            int result = iDefault;
            if (!int.TryParse(str, out result))
            {
                return iDefault;
            }
            return result;
        }

        public static bool HasSwitch(string strName)
        {
            return switches.ContainsKey(strName);
        }

        private static void Initalize()
        {
            if (!initialized)
            {
                initialized = true;
                if (commandline == string.Empty)
                {
                    foreach (string str in Environment.GetCommandLineArgs())
                    {
                        commandline = commandline + "\"" + str + "\" ";
                    }
                }
                if (commandline != string.Empty)
                {
                    string key = string.Empty;
                    foreach (string str3 in String.SplitQuotesStrings(commandline))
                    {
                        if (str3.Length != 0)
                        {
                            if ((str3[0] == '-') || (str3[0] == '+'))
                            {
                                if ((key != string.Empty) && !switches.ContainsKey(key))
                                {
                                    switches.Add(key, string.Empty);
                                }
                                key = str3;
                            }
                            else if (key != string.Empty)
                            {
                                if (!switches.ContainsKey(key))
                                {
                                    switches.Add(key, str3);
                                }
                                key = string.Empty;
                            }
                        }
                    }
                    if ((key != string.Empty) && !switches.ContainsKey(key))
                    {
                        switches.Add(key, string.Empty);
                    }
                }
            }
        }
    }
}


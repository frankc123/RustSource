namespace Facepunch.Utility
{
    using System;
    using System.Text.RegularExpressions;

    public static class String
    {
        public static string QuoteSafe(string str)
        {
            str = str.Replace("\"", "\\\"");
            char[] trimChars = new char[] { '\\' };
            str = str.TrimEnd(trimChars);
            return ("\"" + str + "\"");
        }

        public static string[] SplitQuotesStrings(string input)
        {
            input = input.Replace("\\\"", "&qute;");
            MatchCollection matchs = new Regex("\"([^\"]+)\"|'([^']+)'|\\S+", RegexOptions.Compiled).Matches(input);
            string[] strArray = new string[matchs.Count];
            for (int i = 0; i < matchs.Count; i++)
            {
                char[] trimChars = new char[] { ' ', '"' };
                strArray[i] = matchs[i].Groups[0].Value.Trim(trimChars);
                strArray[i] = strArray[i].Replace("&qute;", "\"");
            }
            return strArray;
        }
    }
}


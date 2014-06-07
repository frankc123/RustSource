namespace Rust
{
    using Facepunch.Utility;
    using System;
    using System.Runtime.InteropServices;

    public static class Notice
    {
        public static void Inventory(string strIcon, string strText)
        {
            strText = String.QuoteSafe(strText);
            ConsoleSystem.Run("notice.inventory " + strText, false);
        }

        public static void Popup(string strIcon, string strText, float fDuration = 4f)
        {
            strIcon = String.QuoteSafe(strIcon);
            strText = String.QuoteSafe(strText);
            ConsoleSystem.Run("notice.popup " + fDuration.ToString() + " " + strIcon + " " + strText, false);
        }
    }
}


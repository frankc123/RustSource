namespace Rust.Steam
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class SteamGroups
    {
        public static List<Group> groupList = new List<Group>();

        public static void Init()
        {
            groupList.Clear();
            int num = SteamGroup_GetCount();
            for (int i = 0; i < num; i++)
            {
                Group item = new Group {
                    steamid = SteamGroup_GetSteamID(i)
                };
                groupList.Add(item);
            }
        }

        public static bool MemberOf(ulong iGroupID)
        {
            <MemberOf>c__AnonStorey50 storey = new <MemberOf>c__AnonStorey50 {
                iGroupID = iGroupID
            };
            return Enumerable.Any<Group>(groupList, new Func<Group, bool>(storey.<>m__F));
        }

        [DllImport("librust")]
        private static extern int SteamGroup_GetCount();
        [DllImport("librust")]
        private static extern ulong SteamGroup_GetSteamID(int iCount);

        [CompilerGenerated]
        private sealed class <MemberOf>c__AnonStorey50
        {
            internal ulong iGroupID;

            internal bool <>m__F(SteamGroups.Group item)
            {
                return (item.steamid == this.iGroupID);
            }
        }

        public class Group
        {
            public ulong steamid;
        }
    }
}


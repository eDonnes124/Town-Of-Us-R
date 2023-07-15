using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using AmongUs.Data.Player;
using Assets.InnerNet;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUs.Patches.ModNews;

// ##https://github.com/Yumenopai/TownOfHost_Y
public class ModNews
{
    public int Number;
    public int BeforeNumber;
    public string Title;
    public string SubTitle;
    public string ShortTitle;
    public string Text;
    public string Date;

    public Announcement ToAnnouncement()
    {
        var result = new Announcement
        {
            Number = Number,
            Title = Title,
            SubTitle = SubTitle,
            ShortTitle = ShortTitle,
            Text = Text,
            Language = (uint)DataManager.Settings.Language.CurrentLanguage,
            Date = Date,
            Id = "ModNews"
        };

        return result;
    }
}
[HarmonyPatch]
public class ModNewsHistory
{
    public static List<ModNews> AllModNews = new();

    // 
    public static void Init()
    {
        {
            // TOU 5.0.1
            var news = new ModNews
            {
                Number = 100001, // 100001
                Title = "TownOfUs v5.0.1",
                SubTitle = "★★★★Another Fix update?★★★★",
                ShortTitle = "★TOU v5.0.1",
                Text = "<size=150%>Welcome to TOU v5.0.1.</size>\n\n<size=125%>Support for Among Us v2023.6.13/2023.6.27.</size>\n"
                    + "\n【Fixes】\n - Bug Fix: Airship Ladders work again\n\r",
                Date = "2023-7-9T00:00:00Z"

            };
            AllModNews.Add(news);
        }   

        {
            // When creating new news, you can not delete old news
            // TOU v5.0.0
            var news = new ModNews
            {
                Number = 100000,
                Title = "TownOfUs v5.0.0",
                SubTitle = "★★★★Ooooh bigger update With Many Roles!!★★★★",
                ShortTitle = "★TOU v5.0.0",
                Text = "Added Support AU 2023.6.13\n" 
                    + "\n【Fixes】\n - Plaguebearer no longer turns into Pestilence early\n\r"
                    + "\n 【New Roles】\n - Doomsayer, Vampire, Vampire Haunter, Prosecutor, Oracle, Venerer\n\r"
                    + "\n 【Reworked】\n - Detective, Mayor\r\n,"
                    + "\n 【New Modifiers】\n - AfterMath, Frosty\r\n,"
                    + "\n 【Remove】\n - Blind, And Settings for disabling name plates and level numbers\r\n,"
                    + "\n 【Changes】\n - New Setting: First round shield for first death in prior game\r\n,"
                    + "\n 【Changes】\n - New Setting: Guardian Angel target evil percentage\r\n,"
                    + "\n 【Changes】\n - Guardian Angel targets can now be a Neutral Killer\r\n,"
                    + "\n 【Changes】\n - Added a new version of Snitch to Cultist\r\n",                  
                Date = "2023-7-5T00:00:00Z"

            };
            AllModNews.Add(news);
        }
    }

    [HarmonyPatch(typeof(PlayerAnnouncementData), nameof(PlayerAnnouncementData.SetAnnouncements)), HarmonyPrefix]
    public static bool SetModAnnouncements(PlayerAnnouncementData __instance, [HarmonyArgument(0)] ref Il2CppReferenceArray<Announcement> aRange)
    {
        if (AllModNews.Count < 1)
        {
            Init();
            AllModNews.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });
        }

        List<Announcement> FinalAllNews = new();
        AllModNews.Do(n => FinalAllNews.Add(n.ToAnnouncement()));
        foreach (var news in aRange)
        {
            if (!AllModNews.Any(x => x.Number == news.Number))
                FinalAllNews.Add(news);
        }
        FinalAllNews.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });

        aRange = new(FinalAllNews.Count);
        for (int i = 0; i < FinalAllNews.Count; i++)
            aRange[i] = FinalAllNews[i];

        return true;
    }
}

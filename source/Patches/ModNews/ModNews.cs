using AmongUs.Data;
using AmongUs.Data.Player;
using Assets.InnerNet;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Reactor.Utilities;

namespace TownOfUs.Patches.ModNews
{
    public class ModNews
    {
        public int Number;
    public uint Lang;
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
            Language = Lang,
            Title = Title,
            SubTitle = SubTitle,
            ShortTitle = ShortTitle,
            Text = Text,
            Date = Date,
            Id = "TOU-ModNews"
        };

        return result;
    }
}

[HarmonyPatch]
public class ModNewsHistory
{
    public static List<ModNews> AllModNews = new();

    [HarmonyPatch(typeof(AnnouncementPopUp), nameof(AnnouncementPopUp.Init)), HarmonyPostfix]
    public static void Initialize(ref Il2CppSystem.Collections.IEnumerator __result)
    {
        static IEnumerator GetEnumerator()
        {
            while (AnnouncementPopUp.UpdateState == AnnouncementPopUp.AnnounceState.Fetching) yield return null;
            if (AnnouncementPopUp.UpdateState > AnnouncementPopUp.AnnounceState.Fetching && DataManager.Player.Announcements.AllAnnouncements.Count > 0) yield break;

            AnnouncementPopUp.UpdateState = AnnouncementPopUp.AnnounceState.Fetching;
            AllModNews.Clear();

            var lang = DataManager.Settings.Language.CurrentLanguage.ToString();
            if (!Assembly.GetExecutingAssembly().GetManifestResourceNames().Any(x => x.StartsWith($"TownOfUs.Resources.ModNews.{lang}.")))
                lang = SupportedLangs.English.ToString();

            var fileNames = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.StartsWith($"TownOfUs.Resources.ModNews.{lang}."));
            foreach (var file in fileNames)
                AllModNews.Add(GetContentFromRes(file));

            AnnouncementPopUp.UpdateState = AnnouncementPopUp.AnnounceState.NotStarted;
        }

        __result = Effects.Sequence(GetEnumerator().WrapToIl2Cpp(), __result);
    }

    public static ModNews GetContentFromRes(string path)
    {
        ModNews mn = new();
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
        stream.Position = 0;
        using StreamReader reader = new(stream, Encoding.UTF8);
        string text = "";
        uint langId = (uint)DataManager.Settings.Language.CurrentLanguage;
        //uint langId = (uint)SupportedLangs.SChinese;
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            if (line.StartsWith("#Number:")) mn.Number = int.Parse(line.Replace("#Number:", string.Empty));
            else if (line.StartsWith("#LangId:")) langId = uint.Parse(line.Replace("#LangId:", string.Empty));
            else if (line.StartsWith("#Title:")) mn.Title = line.Replace("#Title:", string.Empty);
            else if (line.StartsWith("#SubTitle:")) mn.SubTitle = line.Replace("#SubTitle:", string.Empty);
            else if (line.StartsWith("#ShortTitle:")) mn.ShortTitle = line.Replace("#ShortTitle:", string.Empty);
            else if (line.StartsWith("#Date:")) mn.Date = line.Replace("#Date:", string.Empty);
            else if (line.StartsWith("#---")) continue;
            else
            {
                if (line.StartsWith("## ")) line = line.Replace("## ", "<b>") + "</b>";
                else if (line.StartsWith("- ")) line = line.Replace("- ", "・");
                text += $"\n{line}";
            }
        }
        mn.Lang = langId;
        mn.Text = text;
        PluginSingleton<TownOfUs>.Instance.Log.LogInfo($"Number:{mn.Number}");
        PluginSingleton<TownOfUs>.Instance.Log.LogInfo($"Title:{mn.Title}");
        PluginSingleton<TownOfUs>.Instance.Log.LogInfo($"SubTitle:{mn.SubTitle}");
        PluginSingleton<TownOfUs>.Instance.Log.LogInfo($"ShortTitle:{mn.ShortTitle}");
        PluginSingleton<TownOfUs>.Instance.Log.LogInfo($"Date:{mn.Date}");
        return mn;
    }

    [HarmonyPatch(typeof(PlayerAnnouncementData), nameof(PlayerAnnouncementData.SetAnnouncements)), HarmonyPrefix]
    public static bool SetModAnnouncements(PlayerAnnouncementData __instance, [HarmonyArgument(0)] Il2CppReferenceArray<Announcement> aRange)
    {
        List<Announcement> list = new();
        foreach (var a in aRange) list.Add(a);
        foreach (var m in AllModNews) list.Add(m.ToAnnouncement());
        list.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });

        __instance.allAnnouncements = new Il2CppSystem.Collections.Generic.List<Announcement>();
        foreach (var a in list) __instance.allAnnouncements.Add(a);


        __instance.HandleChange();
        __instance.OnAddAnnouncement?.Invoke();

        return false;
    }
}

}
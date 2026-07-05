using HarmonyLib;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Twitch;
using Reactor.Utilities;

namespace TownOfUs
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public class ModUpdaterButton
    {
        private static Sprite TOUUpdateSprite => TownOfUs.UpdateTOUButton;
        private static Sprite SubmergedUpdateSprite => TownOfUs.UpdateSubmergedButton;
        private static bool CheckedBadVersion;

        private static void Prefix(MainMenuManager __instance)
        {
            //Check if there's a ToU update
            ModUpdater.LaunchUpdater();
            if (!CheckedBadVersion)
            {
                CheckedBadVersion = true;
                var auVersion = Version.Parse(Application.version);

                var data = GetVersioning().FirstOrDefault(x => x.ModVersion.Equals(TownOfUs.VersionString));
                if (data != null)
                {
                    var RequiredVersions = new Dictionary<Version, AuSupport>();
                    foreach (var pair in data.InternalVersions)
                    {
                        RequiredVersions.Add(Version.Parse(pair.Key), (AuSupport)pair.Value);
                    }

                    if (!RequiredVersions.TryGetValue(auVersion, out var support) || support is AuSupport.Broken)
                    {
                        string action = auVersion > RequiredVersions.Keys.Max() ? "downgrade" : "update";
                        var neededVersions = RequiredVersions.Where(x => x.Value is AuSupport.Preferred)
                            .Select(y => y.Key.ToString()).ToArray();
                        var mainInfo = neededVersions.Count() > 1
                            ? $"Town of Us {TownOfUs.VersionString} requires one of the following Among Us versions: {string.Join(",", neededVersions)}!"
                            : $"Town of Us {TownOfUs.VersionString} requires Among Us v{neededVersions.First()}!";
                        string info =
                            $"ALERT\n{mainInfo}\nyou have {Application.version}\nPlease {action} Among Us!"
                            + "\nvisit Github or Discord for any help";
                        TwitchManager man = TwitchManager.Instance;
                        ModUpdater.InfoPopup = UnityEngine.Object.Instantiate(man.TwitchPopup);
                        ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.68f;
                        ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = true;
                        ModUpdater.InfoPopup.Show(info);
                        ModUpdater.InfoPopup.StartCoroutine(Effects.Lerp(0.01f,
                            new Action<float>((p) => { ModUpdater.setPopupText(info); })));
                        ModUpdater.AuVersionSupported = AuSupport.Broken;
                        PluginSingleton<TownOfUs>.Instance.Log.LogMessage(
                            $"Current AU Version is {Application.version} | Tou Support: {AuSupport.Broken}");

                        return;
                    }

                    if (support is AuSupport.Usable)
                    {
                        var neededVersions = RequiredVersions.Where(x => x.Value is AuSupport.Preferred)
                            .Select(y => y.Key.ToString()).ToArray();
                        var mainInfo = neededVersions.Count() > 1
                            ? $"Town of Us {TownOfUs.VersionString} works best with the following Among Us versions: {string.Join(",", neededVersions)}."
                            : $"Town of Us {TownOfUs.VersionString} works best with Among Us v{neededVersions.First()}.";
                        string info =
                            $"NOTICE\n{mainInfo}\nAmong us v{Application.version} should still work, but may have issues."
                            + "\nVisit Github or Discord for additional information";
                        TwitchManager man = TwitchManager.Instance;
                        ModUpdater.InfoPopup = UnityEngine.Object.Instantiate(man.TwitchPopup);
                        ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.68f;
                        ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = true;
                        ModUpdater.InfoPopup.Show(info);
                        ModUpdater.InfoPopup.StartCoroutine(Effects.Lerp(0.01f,
                            new Action<float>((p) => { ModUpdater.setPopupText(info); })));
                        ModUpdater.AuVersionSupported = AuSupport.Usable;
                    }

                    PluginSingleton<TownOfUs>.Instance.Log.LogMessage(
                        $"Current AU Version is {Application.version} | Tou Support: {support}");
                }
                else
                {
                    PluginSingleton<TownOfUs>.Instance.Log.LogMessage(
                        $"Current AU Version is {Application.version} | Tou Support: ???");
                }
            }

            if (ModUpdater.HasTOUUpdate)
            {
                //If there's an update, create and show the update button
                UpdateButton(__instance, () => ModUpdater.ExecuteUpdate("TOU"));
            }

            if (ModUpdater.HasSubmergedUpdate)
            {
                //If there's an update, create and show the update button
                UpdateButton(__instance, () => ModUpdater.ExecuteUpdate("Submerged"), 1);
            }
        }

        private static void UpdateButton(MainMenuManager __instance, Action Onclick, int _pos = 0)
        {
            var template = GameObject.Find("ExitGameButton");
            if (template != null)
            {

                var Button = UnityEngine.Object.Instantiate(template, null);
                Button.transform.localPosition = new Vector3(Button.transform.localPosition.x,
                    Button.transform.localPosition.y + 0.6f, Button.transform.localPosition.z);

                Button.transform.localScale = new Vector3(0.44f, 0.84f, 1f);

                PassiveButton passiveButton = Button.GetComponent<PassiveButton>();
                SpriteRenderer ButtonSprite = Button.transform.GetChild(1).GetComponent<SpriteRenderer>();
                passiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();

                ButtonSprite.sprite = _pos == 0 ? TOUUpdateSprite : SubmergedUpdateSprite;
                Button.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite =
                    _pos == 0 ? TOUUpdateSprite : SubmergedUpdateSprite;

                Button.transform.SetParent(GameObject.Find("RightPanel").transform);
                var pos = Button.GetComponent<AspectPosition>();
                pos.Alignment = AspectPosition.EdgeAlignments.LeftBottom;
                pos.DistanceFromEdge = new Vector3(1.5f, 1f + 0.5f * _pos, 0f);

                //Add onClick event to run the update on button click
                passiveButton.OnClick.AddListener((Action)(() =>
                {
                    Onclick();
                    Button.SetActive(false);
                }));

                //Set button text
                var text = Button.transform.GetChild(2).GetChild(0).GetComponent<TMPro.TMP_Text>();
                __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
                {
                    text.SetText("");
                    pos.AdjustPosition();
                })));

                //Set popup stuff
                TwitchManager man = TwitchManager.Instance;
                ModUpdater.InfoPopup = UnityEngine.Object.Instantiate(man.TwitchPopup);
                ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.7f;
                ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = false;
            }
        }

        private static List<ModUpdater.UpdateData> GetVersioning()
        {
            var text = ModUpdater.Httpclient
                .GetAsync("https://github.com/eDonnes124/Town-Of-Us-R/raw/master/source/VersioningV2.json")
                .GetAwaiter().GetResult().Content.ReadAsStringAsync().Result;
            var data = JsonSerializer.Deserialize<List<ModUpdater.UpdateData>>(text,
                options: new() { ReadCommentHandling = JsonCommentHandling.Skip });
            return data;
        }
    }

    public class ModUpdater
    {
        public static bool Running = false;
        public static bool HasTOUUpdate = false;
        public static bool HasSubmergedUpdate = false;
        public static AuSupport AuVersionSupported = AuSupport.Preferred;
        public static string UpdateTOUURI = null;
        public static string UpdateSubmergedURI = null;
        private static Task UpdateTOUTask = null;
        private static Task UpdateSubmergedTask = null;
        public static GenericPopup InfoPopup;
        public static HttpClient Httpclient = new() 
        {
            DefaultRequestHeaders = 
            {
                {"User-Agent", "TownOfUs Updater"}
            } 
        };

        public static void LaunchUpdater()
        {
            if (Running) return;
            Running = true;

            checkForUpdate("TOU").GetAwaiter().GetResult();

            //Only check of Submerged update if Submerged is already installed
            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uri = new(codeBase);
            string submergedPath = Uri.UnescapeDataString(uri.Path.Replace("TownOfUs", "Submerged"));
            if (File.Exists(submergedPath))
            {
                checkForUpdate("Submerged").GetAwaiter().GetResult();
            }

            clearOldVersions();
        }

        public static void ExecuteUpdate(string updateType = "TOU")
        {
            string info = "";
            if (updateType == "TOU")
            {
                info = "Updating Town Of Us\nPlease wait...";
                InfoPopup.Show(info);
                if (UpdateTOUTask == null)
                {
                    if (UpdateTOUURI != null)
                    {
                        UpdateTOUTask = downloadUpdate("TOU");
                    }
                    else
                    {
                        info = "Unable to auto-update\nPlease update manually";
                    }
                }
                else
                {
                    info = "Update might already\nbe in progress";
                }
            }
            else if (updateType == "Submerged")
            {
                info = "Updating Submerged\nPlease wait...";
                InfoPopup.Show(info);
                if (UpdateSubmergedTask == null)
                {
                    if (UpdateSubmergedURI != null)
                    {
                        UpdateSubmergedTask = downloadUpdate("Submerged");
                    }
                    else
                    {
                        info = "Unable to auto-update\nPlease update manually";
                    }
                }
                else
                {
                    info = "Update might already\nbe in progress";
                }
            }
            InfoPopup.StartCoroutine(Effects.Lerp(0.01f, new System.Action<float>((p) => { ModUpdater.setPopupText(info); })));
        }

        public static void clearOldVersions()
        {
            //Removes any old versions (Denoted by the suffix `.old`)
            try
            {
                DirectoryInfo d = new DirectoryInfo(Path.GetDirectoryName(Application.dataPath) + @"\BepInEx\plugins");
                string[] files = d.GetFiles("*.old").Select(x => x.FullName).ToArray();
                foreach (string f in files) File.Delete(f);
            }
            catch (Exception e)
            {
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Exception occured when clearing old versions:\n" + e);
            }
        }
        public static async Task<bool> checkForUpdate(string updateType = "TOU")
        {
            //Checks the github api for Town Of Us tags. Compares current version (from VersionString in TownOfUs.cs) to the latest tag version(on GitHub)
            try
            {
                string githubURI = "";
                if (updateType == "TOU")
                {
                    githubURI = "https://api.github.com/repos/eDonnes124/Town-Of-Us-R/releases/latest";
                }
                else if (updateType == "Submerged")
                {
                    githubURI = "https://api.github.com/repos/SubmergedAmongUs/Submerged/releases/latest";
                }
                var response = await  Httpclient.GetAsync(new Uri(githubURI), HttpCompletionOption.ResponseContentRead);

                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }
                string json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<GitHubApiObject>(json);

                string tagname = data.tag_name;
                if (tagname == null)
                {
                    return false; // Something went wrong
                }

                int diff = 0;
                Version ver = Version.Parse(tagname.Replace("v", ""));
                if (updateType == "TOU")
                { //Check TOU version
                    diff = TownOfUs.Version.CompareTo(ver);
                    if (diff < 0)
                    { // TOU update required
                        HasTOUUpdate = true;
                    }
                }
                else if (updateType == "Submerged")
                {
                    //account for broken version
                    if (Patches.SubmergedCompatibility.Version == null) HasSubmergedUpdate = true;
                    else
                    {
                        diff = Patches.SubmergedCompatibility.Version.CompareTo(SemanticVersioning.Version.Parse(tagname.Replace("v", ""))); ;
                        if (diff < 0)
                        { // Submerged update required
                            HasSubmergedUpdate = true;
                        }
                    }
                }
                var assets = data.assets;
                if (assets == null) return false;

                foreach (var asset in assets)
                {
                    if (asset.browser_download_url == null) continue;
                    if (asset.browser_download_url.EndsWith(".dll"))
                    {
                        if (updateType == "TOU")
                        {
                            UpdateTOUURI = asset.browser_download_url;
                        }
                        else if (updateType == "Submerged")
                        {
                            UpdateSubmergedURI = asset.browser_download_url;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage(ex);
            }
            return false;
        }

        public static async Task<bool> downloadUpdate(string updateType = "TOU")
        {
            //Downloads the new TownOfUs/Submerged dll from GitHub into the plugins folder
            string downloadDLL = "";
            string info = "";
            if (updateType == "TOU")
            {
                downloadDLL = UpdateTOUURI;
                info = "Town Of Us\nupdated successfully.\nPlease RESTART the game.";
            }
            else if (updateType == "Submerged")
            {
                downloadDLL = UpdateSubmergedURI;
                info = "Submerged\nupdated successfully.\nPlease RESTART the game.";
            }
            try
            {
                var response = await  Httpclient.GetAsync(new System.Uri(downloadDLL), HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }
                string codeBase = Assembly.GetExecutingAssembly().Location;
                System.UriBuilder uri = new System.UriBuilder(codeBase);
                string fullname = System.Uri.UnescapeDataString(uri.Path);
                if (updateType == "Submerged")
                {
                    fullname = fullname.Replace("TownOfUs", "Submerged"); //TODO A better solution than this to correctly name the dll files
                }
                if (File.Exists(fullname + ".old")) // Clear old file in case it wasnt;
                    File.Delete(fullname + ".old");

                File.Move(fullname, fullname + ".old"); // rename current executable to old

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    using var fileStream = File.Create(fullname);
                    responseStream.CopyTo(fileStream);
                }
                showPopup(info);
                return true;
            }
            catch (Exception ex)
            {
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage(ex);
            }
            showPopup("Update wasn't successful\nTry again later,\nor update manually.");
            return false;
        }
        private static void showPopup(string message)
        {
            setPopupText(message);
            InfoPopup.gameObject.SetActive(true);
        }

        public static void setPopupText(string message)
        {
            if (InfoPopup == null) return;
            
            if (InfoPopup.TextAreaTMP != null)
            {
                InfoPopup.TextAreaTMP.text = message;
            }
        }


        class GitHubApiObject
        {
            public string tag_name { get; set; }
            public GitHubApiAsset[] assets { get; set; }
        }

        class GitHubApiAsset
        {
            public string browser_download_url { get; set; }
        }

        public class UpdateData
        {
            public Dictionary<string, int> InternalVersions { get; set; }

            public string ModVersion { get; set; }
        }
    }

    [HarmonyPatch(typeof(GenericPopup), nameof(GenericPopup.Close))]
    public class TextBoxPatch
    {
        [HarmonyPostfix]
        public static void Postfix(GenericPopup __instance)
        {
            if (__instance != ModUpdater.InfoPopup) return;

            if (ModUpdater.AuVersionSupported is AuSupport.Broken)
            {
                Application.Quit();
            }
        }
    }

    public enum AuSupport
    {
        Preferred,
        Usable,
        Broken
    }
}
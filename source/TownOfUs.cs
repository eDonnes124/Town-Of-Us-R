using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Utilities.Extensions;
using Reactor.Networking.Attributes;
using TownOfUs.CustomOption;
using TownOfUs.Patches;
using TownOfUs.RainbowMod;
using TownOfUs.Extensions;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.SceneManagement;
using TownOfUs.Patches.ScreenEffects;
using System.IO;

namespace TownOfUs
{
    [BepInPlugin(Id, "Town Of Us", VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
    public class TownOfUs : BasePlugin
    {
        public const string Id = "com.slushiegoose.townofus";
        public const string VersionString = "5.0.4";
        public static System.Version Version = System.Version.Parse(VersionString);
        public const string VersionTag = "<color=#ff33fc></color>";

        public static AssetLoader bundledAssets;

        public static Sprite JanitorClean;
        public static Sprite EngineerFix;
        public static Sprite SwapperSwitch;
        public static Sprite SwapperSwitchDisabled;
        public static Sprite Footprint;
        public static Sprite NormalKill;
        public static Sprite MedicSprite;
        public static Sprite SeerSprite;
        public static Sprite SampleSprite;
        public static Sprite MorphSprite;
        public static Sprite Arrow;
        public static Sprite MineSprite;
        public static Sprite SwoopSprite;
        public static Sprite DouseSprite;
        public static Sprite IgniteSprite;
        public static Sprite ReviveSprite;
        public static Sprite ButtonSprite;
        public static Sprite DisperseSprite;
        public static Sprite CycleBackSprite;
        public static Sprite CycleForwardSprite;
        public static Sprite GuessSprite;
        public static Sprite DragSprite;
        public static Sprite DropSprite;
        public static Sprite FlashSprite;
        public static Sprite AlertSprite;
        public static Sprite RememberSprite;
        public static Sprite TrackSprite;
        public static Sprite PlantSprite;
        public static Sprite DetonateSprite;
        public static Sprite TransportSprite;
        public static Sprite MediateSprite;
        public static Sprite VestSprite;
        public static Sprite ProtectSprite;
        public static Sprite BlackmailSprite;
        public static Sprite BlackmailLetterSprite;
        public static Sprite BlackmailOverlaySprite;
        public static Sprite LighterSprite;
        public static Sprite DarkerSprite;
        public static Sprite InfectSprite;
        public static Sprite RampageSprite;
        public static Sprite TrapSprite;
        public static Sprite InspectSprite;
        public static Sprite ExamineSprite;
        public static Sprite EscapeSprite;
        public static Sprite MarkSprite;
        public static Sprite Revive2Sprite;
        public static Sprite WhisperSprite;
        public static Sprite ImitateSelectSprite;
        public static Sprite ImitateDeselectSprite;
        public static Sprite ObserveSprite;
        public static Sprite BiteSprite;
        public static Sprite StakeSprite;
        public static Sprite RevealSprite;
        public static Sprite ConfessSprite;
        public static Sprite NoAbilitySprite;
        public static Sprite CamouflageSprite;
        public static Sprite CamoSprintSprite;
        public static Sprite CamoSprintFreezeSprite;
        public static Sprite RadiateSprite;
        public static Sprite HackSprite;
        public static Sprite MimicSprite;
        public static Sprite LockSprite;
        public static Sprite StalkSprite;

        public static Sprite SettingsButtonSprite;
        public static Sprite CrewSettingsButtonSprite;
        public static Sprite NeutralSettingsButtonSprite;
        public static Sprite ImposterSettingsButtonSprite;
        public static Sprite ModifierSettingsButtonSprite;
        public static Sprite ToUBanner;
        public static Sprite UpdateTOUButton;
        public static Sprite UpdateSubmergedButton;

        public static Sprite ZoomPlusButton;
        public static Sprite ZoomMinusButton;

        public static Vector3 ButtonPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);

        private static DLoadImage _iCallLoadImage;


        private Harmony _harmony;

        public ConfigEntry<string> Ip { get; set; }

        public ConfigEntry<ushort> Port { get; set; }
        public static string RuntimeLocation;
        public override void Load()
        {
            RuntimeLocation = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TownOfUs)).Location);

            System.Console.WriteLine("000.000.000.000/000000000000000000");

            _harmony = new Harmony("com.slushiegoose.townofus");

            Generate.GenerateAll();

            bundledAssets = new();

            JanitorClean = CreateSprite("TownOfUs.Resources.Janitor.png");
            EngineerFix = CreateSprite("TownOfUs.Resources.Engineer.png");
            SwapperSwitch = CreateSprite("TownOfUs.Resources.SwapperSwitch.png");
            SwapperSwitchDisabled = CreateSprite("TownOfUs.Resources.SwapperSwitchDisabled.png");
            Footprint = CreateSprite("TownOfUs.Resources.Footprint.png");
            NormalKill = CreateSprite("TownOfUs.Resources.NormalKill.png");
            MedicSprite = CreateSprite("TownOfUs.Resources.Medic.png");
            SeerSprite = CreateSprite("TownOfUs.Resources.Seer.png");
            SampleSprite = CreateSprite("TownOfUs.Resources.Sample.png");
            MorphSprite = CreateSprite("TownOfUs.Resources.Morph.png");
            Arrow = CreateSprite("TownOfUs.Resources.Arrow.png");
            MineSprite = CreateSprite("TownOfUs.Resources.Mine.png");
            SwoopSprite = CreateSprite("TownOfUs.Resources.Swoop.png");
            DouseSprite = CreateSprite("TownOfUs.Resources.Douse.png");
            IgniteSprite = CreateSprite("TownOfUs.Resources.Ignite.png");
            ReviveSprite = CreateSprite("TownOfUs.Resources.Revive.png");
            ButtonSprite = CreateSprite("TownOfUs.Resources.Button.png");
            DisperseSprite = CreateSprite("TownOfUs.Resources.Disperse.png");
            DragSprite = CreateSprite("TownOfUs.Resources.Drag.png");
            DropSprite = CreateSprite("TownOfUs.Resources.Drop.png");
            CycleBackSprite = CreateSprite("TownOfUs.Resources.CycleBack.png");
            CycleForwardSprite = CreateSprite("TownOfUs.Resources.CycleForward.png");
            GuessSprite = CreateSprite("TownOfUs.Resources.Guess.png");
            FlashSprite = CreateSprite("TownOfUs.Resources.Flash.png");
            AlertSprite = CreateSprite("TownOfUs.Resources.Alert.png");
            RememberSprite = CreateSprite("TownOfUs.Resources.Remember.png");
            TrackSprite = CreateSprite("TownOfUs.Resources.Track.png");
            PlantSprite = CreateSprite("TownOfUs.Resources.Plant.png");
            DetonateSprite = CreateSprite("TownOfUs.Resources.Detonate.png");
            TransportSprite = CreateSprite("TownOfUs.Resources.Transport.png");
            MediateSprite = CreateSprite("TownOfUs.Resources.Mediate.png");
            VestSprite = CreateSprite("TownOfUs.Resources.Vest.png");
            ProtectSprite = CreateSprite("TownOfUs.Resources.Protect.png");
            BlackmailSprite = CreateSprite("TownOfUs.Resources.Blackmail.png");
            BlackmailLetterSprite = CreateSprite("TownOfUs.Resources.BlackmailLetter.png");
            BlackmailOverlaySprite = CreateSprite("TownOfUs.Resources.BlackmailOverlay.png");
            LighterSprite = CreateSprite("TownOfUs.Resources.Lighter.png");
            DarkerSprite = CreateSprite("TownOfUs.Resources.Darker.png");
            InfectSprite = CreateSprite("TownOfUs.Resources.Infect.png");
            RampageSprite = CreateSprite("TownOfUs.Resources.Rampage.png");
            TrapSprite = CreateSprite("TownOfUs.Resources.Trap.png");
            InspectSprite = CreateSprite("TownOfUs.Resources.Inspect.png");
            ExamineSprite = CreateSprite("TownOfUs.Resources.Examine.png");
            EscapeSprite = CreateSprite("TownOfUs.Resources.Recall.png");
            MarkSprite = CreateSprite("TownOfUs.Resources.Mark.png");
            Revive2Sprite = CreateSprite("TownOfUs.Resources.Revive2.png");
            WhisperSprite = CreateSprite("TownOfUs.Resources.Whisper.png");
            ImitateSelectSprite = CreateSprite("TownOfUs.Resources.ImitateSelect.png");
            ImitateDeselectSprite = CreateSprite("TownOfUs.Resources.ImitateDeselect.png");
            ObserveSprite = CreateSprite("TownOfUs.Resources.Observe.png");
            BiteSprite = CreateSprite("TownOfUs.Resources.Bite.png");
            StakeSprite = CreateSprite("TownOfUs.Resources.Stake.png");
            RevealSprite = CreateSprite("TownOfUs.Resources.Reveal.png");
            ConfessSprite = CreateSprite("TownOfUs.Resources.Confess.png");
            NoAbilitySprite = CreateSprite("TownOfUs.Resources.NoAbility.png");
            CamouflageSprite = CreateSprite("TownOfUs.Resources.Camouflage.png");
            CamoSprintSprite = CreateSprite("TownOfUs.Resources.CamoSprint.png");
            CamoSprintFreezeSprite = CreateSprite("TownOfUs.Resources.CamoSprintFreeze.png");
            RadiateSprite = CreateSprite("TownOfUs.Resources.Radiate.png");
            HackSprite = CreateSprite("TownOfUs.Resources.Hack.png");
            MimicSprite = CreateSprite("TownOfUs.Resources.Mimic.png");
            LockSprite = CreateSprite("TownOfUs.Resources.Lock.png");
            StalkSprite = CreateSprite("TownOfUs.Resources.Stalk.png");

            SettingsButtonSprite = CreateSprite("TownOfUs.Resources.SettingsButton.png");
            CrewSettingsButtonSprite = CreateSprite("TownOfUs.Resources.Crewmate.png");
            NeutralSettingsButtonSprite = CreateSprite("TownOfUs.Resources.Neutral.png");
            ImposterSettingsButtonSprite = CreateSprite("TownOfUs.Resources.Impostor.png");
            ModifierSettingsButtonSprite = CreateSprite("TownOfUs.Resources.Modifiers.png");
            ToUBanner = CreateSprite("TownOfUs.Resources.TownOfUsBanner.png");
            UpdateTOUButton = CreateSprite("TownOfUs.Resources.UpdateToUButton.png");
            UpdateSubmergedButton = CreateSprite("TownOfUs.Resources.UpdateSubmergedButton.png");

            ZoomPlusButton = CreateSprite("TownOfUs.Resources.Plus.png");
            ZoomMinusButton = CreateSprite("TownOfUs.Resources.Minus.png");

            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();

            // RegisterInIl2CppAttribute.Register();

            Ip = Config.Bind("Custom", "Ipv4 or Hostname", "127.0.0.1");
            Port = Config.Bind("Custom", "Port", (ushort) 22023);
            var defaultRegions = ServerManager.DefaultRegions.ToList();
            var ip = Ip.Value;
            if (Uri.CheckHostName(Ip.Value).ToString() == "Dns")
                foreach (var address in Dns.GetHostAddresses(Ip.Value))
                {
                    if (address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    ip = address.ToString();
                    break;
                }

            ServerManager.DefaultRegions = defaultRegions.ToArray();

            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((scene, loadSceneMode) =>
            {
                try { ModManager.Instance.ShowModStamp(); }
                catch { }
            }));

            _harmony.PatchAll();
            SubmergedCompatibility.Initialize();
        }

        public static Sprite CreateSprite(string name)
        {
            var pixelsPerUnit = 100f;
            var pivot = new Vector2(0.5f, 0.5f);

            var assembly = Assembly.GetExecutingAssembly();
            var tex = AmongUsExtensions.CreateEmptyTexture();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite;
        }

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    }
}

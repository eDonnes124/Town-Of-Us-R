using System;
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
using TownOfUs.CrewmateRoles.DetectiveMod;
using TownOfUs.NeutralRoles.SoulCollectorMod;
using System.IO;
using Reactor.Utilities;

namespace TownOfUs
{
    [BepInPlugin(Id, "Town Of Us: Reactivated", VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
    public class TownOfUs : BasePlugin
    {
        public static string DataPath => Path.GetFullPath("reactivated_presets", Application.persistentDataPath);
        public const string Id = "com.reactivated.townofus";
        public const string VersionString = "6.0.0";
        public static System.Version Version = System.Version.Parse(VersionString);
        public const string VersionTag = "<color=#ff33fc></color>";

        public static AssetLoader bundledAssets;
        public static Sprite NextSprite;
        public static Sprite NextActiveSprite;

        public static Sprite JanitorClean;
        public static Sprite EngineerFix;
        public static Sprite SwapperSwitch;
        public static Sprite SwapperSwitchDisabled;
        public static Sprite Footprint;
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
        public static Sprite ImitateSelectSprite;
        public static Sprite ImitateDeselectSprite;
        public static Sprite ObserveSprite;
        public static Sprite BiteSprite;
        public static Sprite RevealSprite;
        public static Sprite ConfessSprite;
        public static Sprite BlessSprite;
        public static Sprite NoAbilitySprite;
        public static Sprite CamouflageSprite;
        public static Sprite CamoSprintSprite;
        public static Sprite CamoSprintFreezeSprite;
        public static Sprite HackSprite;
        public static Sprite MimicSprite;
        public static Sprite LockSprite;
        public static Sprite StalkSprite;
        public static Sprite CrimeSceneSprite;
        public static Sprite CampaignSprite;
        public static Sprite FortifySprite;
        public static Sprite HypnotiseSprite;
        public static Sprite HysteriaSprite;
        public static Sprite JailSprite;
        public static Sprite InJailSprite;
        public static Sprite JailCellSprite;
        public static Sprite ExecuteSprite;
        public static Sprite ReapSprite;
        public static Sprite SoulSprite;
        public static Sprite WatchSprite;
        public static Sprite CampSprite;
        public static Sprite ShootSprite;
        public static Sprite FlushSprite;
        public static Sprite BlockSprite;
        public static Sprite BarricadeSprite;
        public static Sprite BlindSprite;
        public static Sprite GuardSprite;
        public static Sprite BribeSprite;
        public static Sprite BarrierSprite;
        public static Sprite CleanseSprite;
        public static Sprite DetectSprite;

        public static Sprite ToUBanner;
        public static Sprite UpdateTOUButton;
        public static Sprite UpdateSubmergedButton;

        public static Sprite ZoomPlusButton;
        public static Sprite ZoomMinusButton;
        public static Sprite ZoomPlusActiveButton;
        public static Sprite ZoomMinusActiveButton;

        public static Vector3 ButtonPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);

        private Harmony _harmony;

        public static ConfigEntry<bool> DeadSeeGhosts { get; set; }

        public static ConfigEntry<bool> SeeSettingNotifier { get; set; }

        public static string RuntimeLocation;

        public override void Load()
        {
            RuntimeLocation = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TownOfUs)).Location);
            ReactorCredits.Register<TownOfUs>(ReactorCredits.AlwaysShow);
            System.Console.WriteLine("000.000.000.000/000000000000000000");

            _harmony = new Harmony("com.reactivated.townofus");

            Generate.GenerateAll();

            bundledAssets = new();

            var shortPath = "TownOfUs.Resources";

            UpdateTOUButton = CreateSprite($"{shortPath}.UpdateToUButton.png");
            UpdateSubmergedButton = CreateSprite($"{shortPath}.UpdateSubmergedButton.png");
            NextSprite = CreateSprite($"{shortPath}.NextButton.png");
            NextActiveSprite = CreateSprite($"{shortPath}.NextButtonActive.png");
            SwapperSwitch = CreateSprite($"{shortPath}.SwapperSwitch.png");
            SwapperSwitchDisabled = CreateSprite($"{shortPath}.SwapperSwitchDisabled.png");
            Footprint = CreateSprite($"{shortPath}.Footprint.png");
            Arrow = CreateSprite($"{shortPath}.Arrow.png");
            ButtonSprite = CreateSprite($"{shortPath}.Button.png");
            DisperseSprite = CreateSprite($"{shortPath}.Disperse.png");
            CycleBackSprite = CreateSprite($"{shortPath}.CycleBack.png");
            CycleForwardSprite = CreateSprite($"{shortPath}.CycleForward.png");
            GuessSprite = CreateSprite($"{shortPath}.Guess.png");
            BlackmailLetterSprite = CreateSprite($"{shortPath}.BlackmailLetter.png");
            BlackmailOverlaySprite = CreateSprite($"{shortPath}.BlackmailOverlay.png");
            LighterSprite = CreateSprite($"{shortPath}.Lighter.png");
            DarkerSprite = CreateSprite($"{shortPath}.Darker.png");
            RevealSprite = CreateSprite($"{shortPath}.Reveal.png");
            ImitateSelectSprite = CreateSprite($"{shortPath}.ImitateSelect.png");
            ImitateDeselectSprite = CreateSprite($"{shortPath}.ImitateDeselect.png");
            LockSprite = CreateSprite($"{shortPath}.Lock.png");
            CrimeSceneSprite = CreateSprite($"{shortPath}.CrimeScene.png");
            HysteriaSprite = CreateSprite($"{shortPath}.Hysteria.png");
            InJailSprite = CreateSprite($"{shortPath}.InJail.png");
            JailCellSprite = CreateSprite($"{shortPath}.JailCell.png");
            ExecuteSprite = CreateSprite($"{shortPath}.Execute.png");
            SoulSprite = CreateSprite($"{shortPath}.Soul.png");
            ShootSprite = CreateSprite($"{shortPath}.Shoot.png");
            BarricadeSprite = CreateSprite($"{shortPath}.Barricade.png");
            DetectSprite = CreateSprite($"{shortPath}.Detect.png");

            ToUBanner = CreateSprite($"{shortPath}.TouBanner.png", 125f);

            ZoomPlusButton = CreateSprite($"{shortPath}.Plus.png");
            ZoomMinusButton = CreateSprite($"{shortPath}.Minus.png");
            ZoomPlusActiveButton = CreateSprite($"{shortPath}.PlusActive.png");
            ZoomMinusActiveButton = CreateSprite($"{shortPath}.MinusActive.png");

            var crewPath = "TownOfUs.Resources.CrewButtons";
            EngineerFix = CreateSprite($"{crewPath}.Engineer.png");
            MedicSprite = CreateSprite($"{crewPath}.Medic.png");
            SeerSprite = CreateSprite($"{crewPath}.Seer.png");
            ReviveSprite = CreateSprite($"{crewPath}.Revive.png");
            AlertSprite = CreateSprite($"{crewPath}.Alert.png");
            TrackSprite = CreateSprite($"{crewPath}.Track.png");
            TransportSprite = CreateSprite($"{crewPath}.Transport.png");
            MediateSprite = CreateSprite($"{crewPath}.Mediate.png");
            TrapSprite = CreateSprite($"{crewPath}.Trap.png");
            InspectSprite = CreateSprite($"{crewPath}.Inspect.png");
            ExamineSprite = CreateSprite($"{crewPath}.Examine.png");
            ConfessSprite = CreateSprite($"{crewPath}.Confess.png");
            BlessSprite = CreateSprite($"{crewPath}.Bless.png");
            StalkSprite = CreateSprite($"{crewPath}.Stalk.png");
            CampaignSprite = CreateSprite($"{crewPath}.Campaign.png");
            FortifySprite = CreateSprite($"{crewPath}.Fortify.png");
            JailSprite = CreateSprite($"{crewPath}.Jail.png");
            WatchSprite = CreateSprite($"{crewPath}.Watch.png");
            CampSprite = CreateSprite($"{crewPath}.Camp.png");
            FlushSprite = CreateSprite($"{crewPath}.Flush.png");
            BlockSprite = CreateSprite($"{crewPath}.Block.png");
            BarrierSprite = CreateSprite($"{crewPath}.Barrier.png");
            CleanseSprite = CreateSprite($"{crewPath}.Cleanse.png");

            var impPath = "TownOfUs.Resources.ImpButtons";

            JanitorClean = CreateSprite($"{impPath}.Janitor.png");
            SampleSprite = CreateSprite($"{impPath}.Sample.png");
            MorphSprite = CreateSprite($"{impPath}.Morph.png");
            MineSprite = CreateSprite($"{impPath}.Mine.png");
            SwoopSprite = CreateSprite($"{impPath}.Swoop.png");
            DragSprite = CreateSprite($"{impPath}.Drag.png");
            DropSprite = CreateSprite($"{impPath}.Drop.png");
            FlashSprite = CreateSprite($"{impPath}.Flash.png");
            PlantSprite = CreateSprite($"{impPath}.Plant.png");
            DetonateSprite = CreateSprite($"{impPath}.Detonate.png");
            BlackmailSprite = CreateSprite($"{impPath}.Blackmail.png");
            EscapeSprite = CreateSprite($"{impPath}.Recall.png");
            MarkSprite = CreateSprite($"{impPath}.Mark.png");
            NoAbilitySprite = CreateSprite($"{impPath}.NoAbility.png");
            CamouflageSprite = CreateSprite($"{impPath}.Camouflage.png");
            CamoSprintSprite = CreateSprite($"{impPath}.CamoSprint.png");
            CamoSprintFreezeSprite = CreateSprite($"{impPath}.CamoSprintFreeze.png");
            HypnotiseSprite = CreateSprite($"{impPath}.Hypnotise.png");
            BlindSprite = CreateSprite($"{impPath}.Blind.png");

            var neutPath = "TownOfUs.Resources.NeutButtons";

            DouseSprite = CreateSprite($"{neutPath}.Douse.png");
            IgniteSprite = CreateSprite($"{neutPath}.Ignite.png");
            RememberSprite = CreateSprite($"{neutPath}.Remember.png");
            VestSprite = CreateSprite($"{neutPath}.Vest.png");
            ProtectSprite = CreateSprite($"{neutPath}.Protect.png");
            InfectSprite = CreateSprite($"{neutPath}.Infect.png");
            RampageSprite = CreateSprite($"{neutPath}.Rampage.png");
            ObserveSprite = CreateSprite($"{neutPath}.Observe.png");
            BiteSprite = CreateSprite($"{neutPath}.Bite.png");
            HackSprite = CreateSprite($"{neutPath}.Hack.png");
            MimicSprite = CreateSprite($"{neutPath}.Mimic.png");
            ReapSprite = CreateSprite($"{neutPath}.Reap.png");
            GuardSprite = CreateSprite($"{neutPath}.Guard.png");
            BribeSprite = CreateSprite($"{neutPath}.Bribe.png");

            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<CrimeScene>();
            ClassInjector.RegisterTypeInIl2Cpp<Soul>();

            // RegisterInIl2CppAttribute.Register();

            DeadSeeGhosts = Config.Bind("Settings", "Dead See Other Ghosts", true, "Whether you see other dead players' ghosts while you're dead");
            SeeSettingNotifier = Config.Bind("Settings", "See Setting Notifier", true, "Whether you see setting changes in lobby at bottom left");

            _harmony.PatchAll();
            SubmergedCompatibility.Initialize();
            IL2CPPChainloader.Instance.Finished += LevelImpostorCompatibility.Initialize; // LI has a circular dependency on TOU, so we need to wait for LI to finish loading before we can initialize it

            ServerManager.DefaultRegions = new Il2CppReferenceArray<IRegionInfo>(new IRegionInfo[0]);
        }

        public static Sprite CreateSprite(string name, float pixelsPerUnit = 100f)
        {
            var pivot = new Vector2(0.5f, 0.5f);

            var assembly = Assembly.GetExecutingAssembly();
            var tex = LoadTextureFromResourcePath(name, assembly);
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.name = name;
            sprite.DontDestroy();
            return sprite;
        }
        public static Texture2D LoadTextureFromResourcePath(string resourcePath, Assembly assembly)
        {
            var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
            };
            var myStream = assembly.GetManifestResourceStream(resourcePath);
            if (myStream != null)
            {
                var buttonTexture = myStream.ReadFully();
                tex.LoadImage(buttonTexture, false);
            }
            else
            {
                throw new ArgumentException($"Resource not found: {resourcePath}");
            }

            tex.name = resourcePath;
            return tex;
        }

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    }
}

using System;
using System.Linq;
using TownOfUs.Patches.Localization;

namespace TownOfUs.CustomOption
{
    public class Generate
    {
        public static CustomHeaderOption CrewInvestigativeRoles;
        public static CustomNumberOption AurialOn;
        public static CustomNumberOption DetectiveOn;
        public static CustomNumberOption HaunterOn;
        public static CustomNumberOption InvestigatorOn;
        public static CustomNumberOption MysticOn;
        public static CustomNumberOption OracleOn;
        public static CustomNumberOption SeerOn;
        public static CustomNumberOption SnitchOn;
        public static CustomNumberOption SpyOn;
        public static CustomNumberOption TrackerOn;
        public static CustomNumberOption TrapperOn;

        public static CustomHeaderOption CrewProtectiveRoles;
        public static CustomNumberOption AltruistOn;
        public static CustomNumberOption MedicOn;

        public static CustomHeaderOption CrewKillingRoles;
        public static CustomNumberOption SheriffOn;
        public static CustomNumberOption VampireHunterOn;
        public static CustomNumberOption VeteranOn;
        public static CustomNumberOption VigilanteOn;

        public static CustomHeaderOption CrewSupportRoles;
        public static CustomNumberOption EngineerOn;
        public static CustomNumberOption ImitatorOn;
        public static CustomNumberOption MayorOn;
        public static CustomNumberOption MediumOn;
        public static CustomNumberOption ProsecutorOn;
        public static CustomNumberOption SwapperOn;
        public static CustomNumberOption TransporterOn;

        public static CustomHeaderOption NeutralBenignRoles;
        public static CustomNumberOption AmnesiacOn;
        public static CustomNumberOption GuardianAngelOn;
        public static CustomNumberOption SurvivorOn;

        public static CustomHeaderOption NeutralEvilRoles;
        public static CustomNumberOption DoomsayerOn;
        public static CustomNumberOption ExecutionerOn;
        public static CustomNumberOption JesterOn;
        public static CustomNumberOption PhantomOn;

        public static CustomHeaderOption NeutralKillingRoles;
        public static CustomNumberOption ArsonistOn;
        public static CustomNumberOption PlaguebearerOn;
        public static CustomNumberOption GlitchOn;
        public static CustomNumberOption VampireOn;
        public static CustomNumberOption WerewolfOn;

        public static CustomHeaderOption ImpostorConcealingRoles;
        public static CustomNumberOption EscapistOn;
        public static CustomNumberOption MorphlingOn;
        public static CustomNumberOption SwooperOn;
        public static CustomNumberOption GrenadierOn;
        public static CustomNumberOption VenererOn;

        public static CustomHeaderOption ImpostorKillingRoles;
        public static CustomNumberOption BomberOn;
        public static CustomNumberOption TraitorOn;
        public static CustomNumberOption WarlockOn;

        public static CustomHeaderOption ImpostorSupportRoles;
        public static CustomNumberOption BlackmailerOn;
        public static CustomNumberOption JanitorOn;
        public static CustomNumberOption MinerOn;
        public static CustomNumberOption UndertakerOn;

        public static CustomHeaderOption CrewmateModifiers;
        public static CustomNumberOption AftermathOn;
        public static CustomNumberOption BaitOn;
        public static CustomNumberOption DiseasedOn;
        public static CustomNumberOption FrostyOn;
        public static CustomNumberOption MultitaskerOn;
        public static CustomNumberOption TorchOn;

        public static CustomHeaderOption GlobalModifiers;
        public static CustomNumberOption ButtonBarryOn;
        public static CustomNumberOption FlashOn;
        public static CustomNumberOption GiantOn;
        public static CustomNumberOption LoversOn;
        public static CustomNumberOption RadarOn;
        public static CustomNumberOption SleuthOn;
        public static CustomNumberOption TiebreakerOn;

        public static CustomHeaderOption ImpostorModifiers;
        public static CustomNumberOption DisperserOn;
        public static CustomNumberOption DoubleShotOn;
        public static CustomNumberOption UnderdogOn;

        public static CustomHeaderOption MapSettings;
        public static CustomToggleOption RandomMapEnabled;
        public static CustomNumberOption RandomMapSkeld;
        public static CustomNumberOption RandomMapMira;
        public static CustomNumberOption RandomMapPolus;
        public static CustomNumberOption RandomMapAirship;
        public static CustomNumberOption RandomMapSubmerged;
        public static CustomToggleOption AutoAdjustSettings;
        public static CustomToggleOption SmallMapHalfVision;
        public static CustomNumberOption SmallMapDecreasedCooldown;
        public static CustomNumberOption LargeMapIncreasedCooldown;
        public static CustomNumberOption SmallMapIncreasedShortTasks;
        public static CustomNumberOption SmallMapIncreasedLongTasks;
        public static CustomNumberOption LargeMapDecreasedShortTasks;
        public static CustomNumberOption LargeMapDecreasedLongTasks;

        public static CustomHeaderOption CustomGameSettings;
        public static CustomToggleOption ColourblindComms;
        public static CustomToggleOption ImpostorSeeRoles;
        public static CustomToggleOption DeadSeeRoles;
        public static CustomNumberOption InitialCooldowns;
        public static CustomToggleOption ParallelMedScans;
        public static CustomStringOption SkipButtonDisable;
        public static CustomToggleOption HiddenRoles;
        public static CustomToggleOption FirstDeathShield;
        public static CustomToggleOption NeutralEvilWinEndsGame;

        public static CustomHeaderOption BetterPolusSettings;
        public static CustomToggleOption VentImprovements;
        public static CustomToggleOption VitalsLab;
        public static CustomToggleOption ColdTempDeathValley;
        public static CustomToggleOption WifiChartCourseSwap;

        public static CustomHeaderOption GameModeSettings;
        public static CustomStringOption GameMode;

        public static CustomHeaderOption ClassicSettings;
        public static CustomNumberOption MinNeutralBenignRoles;
        public static CustomNumberOption MaxNeutralBenignRoles;
        public static CustomNumberOption MinNeutralEvilRoles;
        public static CustomNumberOption MaxNeutralEvilRoles;
        public static CustomNumberOption MinNeutralKillingRoles;
        public static CustomNumberOption MaxNeutralKillingRoles;

        public static CustomHeaderOption AllAnySettings;
        public static CustomToggleOption RandomNumberImps;

        public static CustomHeaderOption KillingOnlySettings;
        public static CustomNumberOption NeutralRoles;
        public static CustomNumberOption VeteranCount;
        public static CustomNumberOption VigilanteCount;
        public static CustomToggleOption AddArsonist;
        public static CustomToggleOption AddPlaguebearer;

        public static CustomHeaderOption CultistSettings;
        public static CustomNumberOption MayorCultistOn;
        public static CustomNumberOption SeerCultistOn;
        public static CustomNumberOption SheriffCultistOn;
        public static CustomNumberOption SurvivorCultistOn;
        public static CustomNumberOption NumberOfSpecialRoles;
        public static CustomNumberOption MaxChameleons;
        public static CustomNumberOption MaxEngineers;
        public static CustomNumberOption MaxInvestigators;
        public static CustomNumberOption MaxMystics;
        public static CustomNumberOption MaxSnitches;
        public static CustomNumberOption MaxSpies;
        public static CustomNumberOption MaxTransporters;
        public static CustomNumberOption MaxVigilantes;
        public static CustomNumberOption WhisperCooldown;
        public static CustomNumberOption IncreasedCooldownPerWhisper;
        public static CustomNumberOption WhisperRadius;
        public static CustomNumberOption ConversionPercentage;
        public static CustomNumberOption DecreasedPercentagePerConversion;
        public static CustomNumberOption ReviveCooldown;
        public static CustomNumberOption IncreasedCooldownPerRevive;
        public static CustomNumberOption MaxReveals;

        public static CustomHeaderOption TaskTrackingSettings;
        public static CustomToggleOption SeeTasksDuringRound;
        public static CustomToggleOption SeeTasksDuringMeeting;
        public static CustomToggleOption SeeTasksWhenDead;

        public static CustomHeaderOption Sheriff;
        public static CustomToggleOption SheriffKillOther;
        public static CustomToggleOption SheriffKillsDoomsayer;
        public static CustomToggleOption SheriffKillsExecutioner;
        public static CustomToggleOption SheriffKillsJester;
        public static CustomToggleOption SheriffKillsArsonist;
        public static CustomToggleOption SheriffKillsJuggernaut;
        public static CustomToggleOption SheriffKillsPlaguebearer;
        public static CustomToggleOption SheriffKillsGlitch;
        public static CustomToggleOption SheriffKillsVampire;
        public static CustomToggleOption SheriffKillsWerewolf;
        public static CustomNumberOption SheriffKillCd;
        public static CustomToggleOption SheriffBodyReport;

        public static CustomHeaderOption Engineer;
        public static CustomNumberOption MaxFixes;

        public static CustomHeaderOption Investigator;
        public static CustomNumberOption FootprintSize;
        public static CustomNumberOption FootprintInterval;
        public static CustomNumberOption FootprintDuration;
        public static CustomToggleOption AnonymousFootPrint;
        public static CustomToggleOption VentFootprintVisible;

        public static CustomHeaderOption Medic;
        public static CustomStringOption ShowShielded;
        public static CustomStringOption WhoGetsNotification;
        public static CustomToggleOption ShieldBreaks;
        public static CustomToggleOption MedicReportSwitch;
        public static CustomNumberOption MedicReportNameDuration;
        public static CustomNumberOption MedicReportColorDuration;

        public static CustomHeaderOption Seer;
        public static CustomNumberOption SeerCooldown;
        public static CustomToggleOption CrewKillingRed;
        public static CustomToggleOption NeutBenignRed;
        public static CustomToggleOption NeutEvilRed;
        public static CustomToggleOption NeutKillingRed;
        public static CustomToggleOption TraitorColourSwap;

        public static CustomHeaderOption Spy;
        public static CustomStringOption WhoSeesDead;

        public static CustomHeaderOption Swapper;
        public static CustomToggleOption SwapperButton;

        public static CustomHeaderOption Transporter;
        public static CustomNumberOption TransportCooldown;
        public static CustomNumberOption TransportMaxUses;
        public static CustomToggleOption TransporterVitals;

        public static CustomHeaderOption Jester;
        public static CustomToggleOption JesterButton;
        public static CustomToggleOption JesterVent;
        public static CustomToggleOption JesterImpVision;
        public static CustomToggleOption JesterHaunt;

        public static CustomHeaderOption TheGlitch;
        public static CustomNumberOption MimicCooldownOption;
        public static CustomNumberOption MimicDurationOption;
        public static CustomNumberOption HackCooldownOption;
        public static CustomNumberOption HackDurationOption;
        public static CustomNumberOption GlitchKillCooldownOption;
        public static CustomStringOption GlitchHackDistanceOption;
        public static CustomToggleOption GlitchVent;

        public static CustomHeaderOption Juggernaut;
        public static CustomNumberOption JuggKillCooldown;
        public static CustomNumberOption ReducedKCdPerKill;
        public static CustomToggleOption JuggVent;

        public static CustomHeaderOption Morphling;
        public static CustomNumberOption MorphlingCooldown;
        public static CustomNumberOption MorphlingDuration;
        public static CustomToggleOption MorphlingVent;

        public static CustomHeaderOption Executioner;
        public static CustomStringOption OnTargetDead;
        public static CustomToggleOption ExecutionerButton;
        public static CustomToggleOption ExecutionerTorment;

        public static CustomHeaderOption Phantom;
        public static CustomNumberOption PhantomTasksRemaining;
        public static CustomToggleOption PhantomSpook;

        public static CustomHeaderOption Snitch;
        public static CustomToggleOption SnitchSeesNeutrals;
        public static CustomNumberOption SnitchTasksRemaining;
        public static CustomToggleOption SnitchSeesImpInMeeting;
        public static CustomToggleOption SnitchSeesTraitor;

        public static CustomHeaderOption Altruist;
        public static CustomNumberOption ReviveDuration;
        public static CustomToggleOption AltruistTargetBody;

        public static CustomHeaderOption Miner;
        public static CustomNumberOption MineCooldown;

        public static CustomHeaderOption Swooper;
        public static CustomNumberOption SwoopCooldown;
        public static CustomNumberOption SwoopDuration;
        public static CustomToggleOption SwooperVent;

        public static CustomHeaderOption Arsonist;
        public static CustomNumberOption DouseCooldown;
        public static CustomNumberOption MaxDoused;
        public static CustomToggleOption ArsoImpVision;
        public static CustomToggleOption IgniteCdRemoved;

        public static CustomHeaderOption Undertaker;
        public static CustomNumberOption DragCooldown;
        public static CustomNumberOption UndertakerDragSpeed;
        public static CustomToggleOption UndertakerVent;
        public static CustomToggleOption UndertakerVentWithBody;

        public static CustomHeaderOption Assassin;
        public static CustomNumberOption NumberOfImpostorAssassins;
        public static CustomNumberOption NumberOfNeutralAssassins;
        public static CustomToggleOption AmneTurnImpAssassin;
        public static CustomToggleOption AmneTurnNeutAssassin;
        public static CustomToggleOption TraitorCanAssassin;
        public static CustomNumberOption AssassinKills;
        public static CustomToggleOption AssassinMultiKill;
        public static CustomToggleOption AssassinCrewmateGuess;
        public static CustomToggleOption AssassinGuessNeutralBenign;
        public static CustomToggleOption AssassinGuessNeutralEvil;
        public static CustomToggleOption AssassinGuessNeutralKilling;
        public static CustomToggleOption AssassinGuessImpostors;
        public static CustomToggleOption AssassinGuessModifiers;
        public static CustomToggleOption AssassinGuessLovers;
        public static CustomToggleOption AssassinateAfterVoting;

        public static CustomHeaderOption Underdog;
        public static CustomNumberOption UnderdogKillBonus;
        public static CustomToggleOption UnderdogIncreasedKC;

        public static CustomHeaderOption Vigilante;
        public static CustomNumberOption VigilanteKills;
        public static CustomToggleOption VigilanteMultiKill;
        public static CustomToggleOption VigilanteGuessNeutralBenign;
        public static CustomToggleOption VigilanteGuessNeutralEvil;
        public static CustomToggleOption VigilanteGuessNeutralKilling;
        public static CustomToggleOption VigilanteGuessLovers;
        public static CustomToggleOption VigilanteAfterVoting;

        public static CustomHeaderOption Haunter;
        public static CustomNumberOption HaunterTasksRemainingClicked;
        public static CustomNumberOption HaunterTasksRemainingAlert;
        public static CustomToggleOption HaunterRevealsNeutrals;
        public static CustomStringOption HaunterCanBeClickedBy;

        public static CustomHeaderOption Grenadier;
        public static CustomNumberOption GrenadeCooldown;
        public static CustomNumberOption GrenadeDuration;
        public static CustomToggleOption GrenadierIndicators;
        public static CustomToggleOption GrenadierVent;
        public static CustomNumberOption FlashRadius;

        public static CustomHeaderOption Veteran;
        public static CustomToggleOption KilledOnAlert;
        public static CustomNumberOption AlertCooldown;
        public static CustomNumberOption AlertDuration;
        public static CustomNumberOption MaxAlerts;

        public static CustomHeaderOption Tracker;
        public static CustomNumberOption UpdateInterval;
        public static CustomNumberOption TrackCooldown;
        public static CustomToggleOption ResetOnNewRound;
        public static CustomNumberOption MaxTracks;

        public static CustomHeaderOption Trapper;
        public static CustomNumberOption TrapCooldown;
        public static CustomToggleOption TrapsRemoveOnNewRound;
        public static CustomNumberOption MaxTraps;
        public static CustomNumberOption MinAmountOfTimeInTrap;
        public static CustomNumberOption TrapSize;
        public static CustomNumberOption MinAmountOfPlayersInTrap;

        public static CustomHeaderOption Traitor;
        public static CustomNumberOption LatestSpawn;
        public static CustomToggleOption NeutralKillingStopsTraitor;

        public static CustomHeaderOption Amnesiac;
        public static CustomToggleOption RememberArrows;
        public static CustomNumberOption RememberArrowDelay;

        public static CustomHeaderOption Medium;
        public static CustomNumberOption MediateCooldown;
        public static CustomToggleOption ShowMediatePlayer;
        public static CustomToggleOption ShowMediumToDead;
        public static CustomStringOption DeadRevealed;

        public static CustomHeaderOption Survivor;
        public static CustomNumberOption VestCd;
        public static CustomNumberOption VestDuration;
        public static CustomNumberOption VestKCReset;
        public static CustomNumberOption MaxVests;

        public static CustomHeaderOption GuardianAngel;
        public static CustomNumberOption ProtectCd;
        public static CustomNumberOption ProtectDuration;
        public static CustomNumberOption ProtectKCReset;
        public static CustomNumberOption MaxProtects;
        public static CustomStringOption ShowProtect;
        public static CustomStringOption GaOnTargetDeath;
        public static CustomToggleOption GATargetKnows;
        public static CustomToggleOption GAKnowsTargetRole;
        public static CustomNumberOption EvilTargetPercent;

        public static CustomHeaderOption Mystic;
        public static CustomNumberOption MysticArrowDuration;

        public static CustomHeaderOption Blackmailer;
        public static CustomNumberOption BlackmailCooldown;

        public static CustomHeaderOption Plaguebearer;
        public static CustomNumberOption InfectCooldown;
        public static CustomNumberOption PestKillCooldown;
        public static CustomToggleOption PestVent;

        public static CustomHeaderOption Werewolf;
        public static CustomNumberOption RampageCooldown;
        public static CustomNumberOption RampageDuration;
        public static CustomNumberOption RampageKillCooldown;
        public static CustomToggleOption WerewolfVent;

        public static CustomHeaderOption Detective;
        public static CustomNumberOption ExamineCooldown;
        public static CustomToggleOption DetectiveReportOn;
        public static CustomNumberOption DetectiveRoleDuration;
        public static CustomNumberOption DetectiveFactionDuration;
        public static CustomToggleOption CanDetectLastKiller;

        public static CustomHeaderOption Escapist;
        public static CustomNumberOption EscapeCooldown;
        public static CustomToggleOption EscapistVent;

        public static CustomHeaderOption Bomber;
        public static CustomNumberOption MaxKillsInDetonation;
        public static CustomNumberOption DetonateDelay;
        public static CustomNumberOption DetonateRadius;
        public static CustomToggleOption BomberVent;

        public static CustomHeaderOption Doomsayer;
        public static CustomNumberOption ObserveCooldown;
        public static CustomToggleOption DoomsayerGuessNeutralBenign;
        public static CustomToggleOption DoomsayerGuessNeutralEvil;
        public static CustomToggleOption DoomsayerGuessNeutralKilling;
        public static CustomToggleOption DoomsayerGuessImpostors;
        public static CustomToggleOption DoomsayerAfterVoting;
        public static CustomNumberOption DoomsayerGuessesToWin;

        public static CustomHeaderOption Vampire;
        public static CustomNumberOption BiteCooldown;
        public static CustomToggleOption VampImpVision;
        public static CustomToggleOption VampVent;
        public static CustomToggleOption NewVampCanAssassin;
        public static CustomNumberOption MaxVampiresPerGame;
        public static CustomToggleOption CanBiteNeutralBenign;
        public static CustomToggleOption CanBiteNeutralEvil;

        public static CustomHeaderOption VampireHunter;
        public static CustomNumberOption StakeCooldown;
        public static CustomNumberOption MaxFailedStakesPerGame;
        public static CustomToggleOption CanStakeRoundOne;
        public static CustomToggleOption SelfKillAfterFinalStake;
        public static CustomStringOption BecomeOnVampDeaths;

        public static CustomHeaderOption Prosecutor;
        public static CustomToggleOption ProsDiesOnIncorrectPros;

        public static CustomHeaderOption Warlock;
        public static CustomNumberOption ChargeUpDuration;
        public static CustomNumberOption ChargeUseDuration;

        public static CustomHeaderOption Oracle;
        public static CustomNumberOption ConfessCooldown;
        public static CustomNumberOption RevealAccuracy;
        public static CustomToggleOption NeutralBenignShowsEvil;
        public static CustomToggleOption NeutralEvilShowsEvil;
        public static CustomToggleOption NeutralKillingShowsEvil;

        public static CustomHeaderOption Venerer;
        public static CustomNumberOption AbilityCooldown;
        public static CustomNumberOption AbilityDuration;
        public static CustomNumberOption SprintSpeed;
        public static CustomNumberOption FreezeSpeed;

        public static CustomHeaderOption Aurial;
        public static CustomNumberOption RadiateRange;
        public static CustomNumberOption RadiateCooldown;
        public static CustomNumberOption RadiateSucceedChance;
        public static CustomNumberOption RadiateCount;
        public static CustomNumberOption RadiateInvis;

        public static CustomHeaderOption Giant;
        public static CustomNumberOption GiantSlow;

        public static CustomHeaderOption Flash;
        public static CustomNumberOption FlashSpeed;

        public static CustomHeaderOption Diseased;
        public static CustomNumberOption DiseasedKillMultiplier;

        public static CustomHeaderOption Bait;
        public static CustomNumberOption BaitMinDelay;
        public static CustomNumberOption BaitMaxDelay;

        public static CustomHeaderOption Lovers;
        public static CustomToggleOption BothLoversDie;
        public static CustomNumberOption LovingImpPercent;
        public static CustomToggleOption NeutralLovers;

        public static CustomHeaderOption Frosty;
        public static CustomNumberOption ChillDuration;
        public static CustomNumberOption ChillStartSpeed;

        public static CustomStringOption Language;

        public static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";
        private static Func<object, string> MultiplierFormat { get; } = value => $"{value:0.0#}x";


        public static void GenerateAll()
        {
            var num = 0;

            Patches.ExportButton = new Export(num++);
            Patches.ImportButton = new Import(num++);

            CrewInvestigativeRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("CrewmateInvestigativeRoles"));
            AurialOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#B34D99FF>{LocalizationManager.Instance.GetString("Aurial")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DetectiveOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#4D4DFFFF>{LocalizationManager.Instance.GetString("Detective")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            HaunterOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#D3D3D3FF>{LocalizationManager.Instance.GetString("Haunter")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            InvestigatorOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#00B3B3FF>{LocalizationManager.Instance.GetString("Investigator")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MysticOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#4D99E6FF>{LocalizationManager.Instance.GetString("Mystic")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            OracleOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#BF00BFFF>{LocalizationManager.Instance.GetString("Oracle")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SeerOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#FFCC80FF>{LocalizationManager.Instance.GetString("Seer")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SnitchOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#D4AF37FF>{LocalizationManager.Instance.GetString("Snitch")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SpyOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#CCA3CCFF>{LocalizationManager.Instance.GetString("Spy")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TrackerOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#009900FF>{LocalizationManager.Instance.GetString("Tracker")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TrapperOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#A7D1B3FF>{LocalizationManager.Instance.GetString("Trapper")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewKillingRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("CrewmateKillingRoles"));
            SheriffOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#FFFF00FF>{LocalizationManager.Instance.GetString("Sheriff")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VampireHunterOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#B3B3E6FF>{LocalizationManager.Instance.GetString("VampireHunter")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VeteranOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#998040FF>{LocalizationManager.Instance.GetString("Veteran")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VigilanteOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#FFFF99FF>{LocalizationManager.Instance.GetString("Vigilante")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewProtectiveRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("CrewmateProtectiveRoles"));
            AltruistOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#660000FF>{LocalizationManager.Instance.GetString("Altruist")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MedicOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#006600FF>{LocalizationManager.Instance.GetString("Medic")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewSupportRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("CrewmateSupportRoles"));
            EngineerOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#FFA60AFF>{LocalizationManager.Instance.GetString("Engineer")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ImitatorOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#B3D94DFF>{LocalizationManager.Instance.GetString("Imitator")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MayorOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#704FA8FF>{LocalizationManager.Instance.GetString("Mayor")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MediumOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#A680FFFF>{LocalizationManager.Instance.GetString("Medium")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ProsecutorOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#B38000FF>{LocalizationManager.Instance.GetString("Prosecutor")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwapperOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#66E666FF>{LocalizationManager.Instance.GetString("Swapper")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TransporterOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#00EEFFFF>{LocalizationManager.Instance.GetString("Transporter")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);


            NeutralBenignRoles = new CustomHeaderOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("NeutralBenignRoles"));
            AmnesiacOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#80B2FFFF>{LocalizationManager.Instance.GetString("Amnesiac")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GuardianAngelOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#B3FFFFFF>{LocalizationManager.Instance.GetString("GuardianAngel")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SurvivorOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#FFE64DFF>{LocalizationManager.Instance.GetString("Survivor")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            NeutralEvilRoles = new CustomHeaderOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("NeutralEvilRoles"));
            DoomsayerOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#00FF80FF>{LocalizationManager.Instance.GetString("Doomsayer")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ExecutionerOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#8C4005FF>{LocalizationManager.Instance.GetString("Executioner")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JesterOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#FFBFCCFF>{LocalizationManager.Instance.GetString("Jester")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PhantomOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#662962FF>{LocalizationManager.Instance.GetString("Phantom")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            NeutralKillingRoles = new CustomHeaderOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("NeutralKillingRoles"));
            ArsonistOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#FF4D00FF>{LocalizationManager.Instance.GetString("Arsonist")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PlaguebearerOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#E6FFB3FF>{LocalizationManager.Instance.GetString("Plaguebearer")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GlitchOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#00FF00FF>{LocalizationManager.Instance.GetString("TheGlitch")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VampireOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#262626FF>{LocalizationManager.Instance.GetString("Vampire")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            WerewolfOn = new CustomNumberOption(num++, MultiMenu.neutral, $"<color=#A86629FF>{LocalizationManager.Instance.GetString("Werewolf")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorConcealingRoles = new CustomHeaderOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("ImpostorConcealingRoles"));
            EscapistOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Escapist")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GrenadierOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Grenadier")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MorphlingOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Morphling")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwooperOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Swooper")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VenererOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Venerer")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorKillingRoles = new CustomHeaderOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("ImpostorKillingRoles"));
            BomberOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Bomber")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TraitorOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Traitor")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            WarlockOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Warlock")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorSupportRoles = new CustomHeaderOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("ImpostorSupportRoles"));
            BlackmailerOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Blackmailer")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JanitorOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Janitor")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MinerOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Miner")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            UndertakerOn = new CustomNumberOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Undertaker")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewmateModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("CrewmateModifiers"));
            AftermathOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#A6FFA6FF>{LocalizationManager.Instance.GetString("Aftermath")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            BaitOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#00B3B3FF>{LocalizationManager.Instance.GetString("Bait")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DiseasedOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#808080FF>{LocalizationManager.Instance.GetString("Diseased")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            FrostyOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#99FFFFFF>{LocalizationManager.Instance.GetString("Frosty")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MultitaskerOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#FF804DFF>{LocalizationManager.Instance.GetString("Multitasker")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TorchOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#FFFF99FF>{LocalizationManager.Instance.GetString("Torch")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            GlobalModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("GlobalModifiers"));
            ButtonBarryOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#E600FFFF>{LocalizationManager.Instance.GetString("ButtonBarry")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            FlashOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#FF8080FF>{LocalizationManager.Instance.GetString("Flash")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GiantOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#FFB34DFF>{LocalizationManager.Instance.GetString("Giant")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            LoversOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#FF66CCFF>{LocalizationManager.Instance.GetString("Lovers")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            RadarOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#FF0080FF>{LocalizationManager.Instance.GetString("Radar")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SleuthOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#803333FF>{LocalizationManager.Instance.GetString("Sleuth")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TiebreakerOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#99E699FF>{LocalizationManager.Instance.GetString("Tiebreaker")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("ImpostorModifiers"));
            DisperserOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Disperser")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DoubleShotOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("DoubleShot")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            UnderdogOn = new CustomNumberOption(num++, MultiMenu.modifiers, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Underdog")}</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            GameModeSettings =
                new CustomHeaderOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("GameModeSettings"));
            GameMode = new CustomStringOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("GameMode"),
                new[] { LocalizationManager.Instance.GetString("Classic"), LocalizationManager.Instance.GetString("All Any"), LocalizationManager.Instance.GetString("Killing Only"), LocalizationManager.Instance.GetString("Cultist") });

            ClassicSettings =
                new CustomHeaderOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("ClassicGameModeSettings"));
            MinNeutralBenignRoles =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MinNeutralBenignRoles"), 1, 0, 3, 1);
            MaxNeutralBenignRoles =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxNeutralBenignRoles"), 1, 0, 3, 1);
            MinNeutralEvilRoles =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MinNeutralEvilRoles"), 1, 0, 3, 1);
            MaxNeutralEvilRoles =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxNeutralEvilRoles"), 1, 0, 3, 1);
            MinNeutralKillingRoles =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MinNeutralKillingRoles"), 1, 0, 5, 1);
            MaxNeutralKillingRoles =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxNeutralKillingRoles"), 1, 0, 5, 1);

            AllAnySettings =
                new CustomHeaderOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("AllAnySettings"));
            RandomNumberImps = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("RandomNumberOfImpostors"), true);

            KillingOnlySettings =
                new CustomHeaderOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("KillingOnlySettings"));
            NeutralRoles =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("NeutralRoles"), 1, 0, 5, 1);
            VeteranCount =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("VeteranCount"), 1, 0, 5, 1);
            VigilanteCount =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("VigilanteCount"), 1, 0, 5, 1);
            AddArsonist = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("AddArsonist"), true);
            AddPlaguebearer = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("AddPlaguebearer"), true);

            CultistSettings =
                new CustomHeaderOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("CultistSettings"));
            MayorCultistOn = new CustomNumberOption(num++, MultiMenu.main, $"<color=#704FA8FF>{LocalizationManager.Instance.GetString("Mayor")}</color> ({LocalizationManager.Instance.GetString("CultistMode")})", 100f, 0f, 100f, 10f,
                PercentFormat);
            SeerCultistOn = new CustomNumberOption(num++, MultiMenu.main, $"<color=#FFCC80FF>{LocalizationManager.Instance.GetString("Seer")}</color> ({LocalizationManager.Instance.GetString("CultistMode")})", 100f, 0f, 100f, 10f,
                PercentFormat);
            SheriffCultistOn = new CustomNumberOption(num++, MultiMenu.main, $"<color=#FFFF00FF>{LocalizationManager.Instance.GetString("Sheriff")}</color> ({LocalizationManager.Instance.GetString("CultistMode")})", 100f, 0f, 100f, 10f,
                PercentFormat);
            SurvivorCultistOn = new CustomNumberOption(num++, MultiMenu.main, $"<color=#FFE64DFF>{LocalizationManager.Instance.GetString("Survivor")}</color> ({LocalizationManager.Instance.GetString("CultistMode")})", 100f, 0f, 100f, 10f,
                PercentFormat);
            NumberOfSpecialRoles =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("NumberOfSpecialRoles"), 4, 0, 4, 1);
            MaxChameleons =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxChameleons"), 3, 0, 5, 1);
            MaxEngineers =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxEngineers"), 3, 0, 5, 1);
            MaxInvestigators =
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxInvestigators"), 3, 0, 5, 1);
            MaxMystics = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxMystics"), 3, 0, 5, 1);
            MaxSnitches = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxSnitches"), 3, 0, 5, 1);
            MaxSpies = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxSpies"), 3, 0, 5, 1);
            MaxTransporters = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxTransporters"), 3, 0, 5, 1);
            MaxVigilantes = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaxVigilantes"), 3, 0, 5, 1);
            WhisperCooldown = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("InitialWhisperCooldown"), 25f, 10f, 60f, 2.5f, 
                CooldownFormat);
            IncreasedCooldownPerWhisper = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("IncreasedCooldownPerWhisper"), 5f, 0f, 15f, 0.5f, 
                CooldownFormat);
            WhisperRadius = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("WhisperRadius"), 1f, 0.25f, 5f, 0.25f, 
                MultiplierFormat);
            ConversionPercentage = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("ConversionPercentage"), 25f, 0f, 100f, 5f, 
                PercentFormat);
            DecreasedPercentagePerConversion = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("DecreasedConversionPercentagePerConversion"), 5f, 0f, 15f, 1f, 
                PercentFormat);
            ReviveCooldown = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("InitialReviveCooldown"), 25f, 10f, 60f, 2.5f, 
                CooldownFormat);
            IncreasedCooldownPerRevive = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("IncreasedCooldownPerRevive"), 25f, 10f, 60f, 2.5f,
                CooldownFormat);
            MaxReveals = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MaximumNumberOfReveals"), 5, 1, 15, 1);

            MapSettings = new CustomHeaderOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MapSettings"));
            RandomMapEnabled = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("ChooseRandomMap"), false);
            RandomMapSkeld = new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("SkeldChance"), 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapMira = new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MiraChance"), 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapPolus = new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("PolusChance"), 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapAirship = new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("AirshipChance"), 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapSubmerged = new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("SubmergedChance"), 0f, 0f, 100f, 10f, PercentFormat);
            AutoAdjustSettings = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("AutoAdjustSettings"), false);
            SmallMapHalfVision = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("HalfVisionOnSkeldMiraHQ"), false);
            SmallMapDecreasedCooldown = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("MiraHQDecreasedCooldowns"), 0f, 0f, 15f, 2.5f, CooldownFormat);
            LargeMapIncreasedCooldown = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("AirshipSubmergedIncreasedCooldowns"), 0f, 0f, 15f, 2.5f, CooldownFormat);
            SmallMapIncreasedShortTasks = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("SkeldMiraHQIncreasedShortTasks"), 0, 0, 5, 1);
            SmallMapIncreasedLongTasks = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("SkeldMiraHQIncreasedLongTasks"), 0, 0, 3, 1);
            LargeMapDecreasedShortTasks = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("AirshipSubmergedDecreasedShortTasks"), 0, 0, 5, 1);
            LargeMapDecreasedLongTasks = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("AirshipSubmergedDecreasedLongTasks"), 0, 0, 3, 1);


            BetterPolusSettings =
                new CustomHeaderOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("BetterPolusSettings"));
            VentImprovements = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("BetterPolusVentLayout"), false);
            VitalsLab = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("VitalsMovedToLab"), false);
            ColdTempDeathValley = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("ColdTempMovedToDeathValley"), false);
            WifiChartCourseSwap = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("RebootWifiAndChartCourseSwapped"), false);

            CustomGameSettings = new CustomHeaderOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("CustomGameSettings"));
            ColourblindComms = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("CamouflagedComms"), false);
            ImpostorSeeRoles = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("ImpostorsCanSeeTheRolesOfTheirTeam"), false);
            DeadSeeRoles = 
                new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("DeadCanSeeEveryonesRolesVotes"), false);
            InitialCooldowns = 
                new CustomNumberOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("GameStartCooldowns"), 10f, 10f, 30f, 2.5f, CooldownFormat);
            ParallelMedScans = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("ParallelMedbayScans"), false);
            SkipButtonDisable = 
                new CustomStringOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("DisableMeetingSkipButton"), new[] { LocalizationManager.Instance.GetString("No"), LocalizationManager.Instance.GetString("Emergency"), LocalizationManager.Instance.GetString("Always") });
            HiddenRoles = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("EnableHiddenRoles"), true);
            FirstDeathShield = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("FirstDeathShieldNextGame"), false);
            NeutralEvilWinEndsGame = new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("NeutralEvilWinEndsGame"), true);


            TaskTrackingSettings =
                new CustomHeaderOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("TaskTrackingSettings"));
            SeeTasksDuringRound =
                new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("SeeTasksDuringRound"), false);
            SeeTasksDuringMeeting =
                new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("SeeTasksDuringMeetings"), false);
            SeeTasksWhenDead =
                new CustomToggleOption(num++, MultiMenu.main, LocalizationManager.Instance.GetString("SeeTasksWhenDead"), true);

            Assassin =
                new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("AssassinAbility")}</color>");
            NumberOfImpostorAssassins =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("NumberOfImpostorAssassins"), 1, 0, 4, 1);
            NumberOfNeutralAssassins =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("NumberOfNeutralAssassins"), 1, 0, 5, 1);
            AmneTurnImpAssassin =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("AmnesiacTurnedImpostorGetsAbility"), false);
            AmneTurnNeutAssassin =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("AmnesiacTurnedNeutralKillingGetsAbility"), false);
            TraitorCanAssassin =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("TraitorGetsAbility"), false);
            AssassinKills =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("NumberOfAssassinKills"), 1, 1, 15, 1);

            Aurial =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#B34D99FF>{LocalizationManager.Instance.GetString("Aurial")}</color>");
            RadiateRange =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("RadiateRange"), 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            RadiateCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("RadiateCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            RadiateInvis =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("RadiateSeeDelay"), 10f, 0f, 15f, 1f, CooldownFormat);
            RadiateCount =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("RadiateUsesToSee"), 3, 1, 5, 1);
            RadiateSucceedChance =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("RadiateSucceedChance"), 100f, 0f, 100f, 10f, PercentFormat);

            Detective =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#4D4DFFFF>{LocalizationManager.Instance.GetString("Detective")}</color>");
            ExamineCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("ExamineCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            DetectiveReportOn = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("ShowDetectiveReports"), true);
            DetectiveRoleDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TimeWhereDetectiveWillHaveRole"), 15f, 0f, 60f, 2.5f,
                    CooldownFormat);
            DetectiveFactionDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TimeWhereDetectiveWill HaveFaction"), 30f, 0f, 60f, 2.5f,
                    CooldownFormat);
            CanDetectLastKiller = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("Can Detect Last Killer"), false);

            Haunter =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#d3d3d3FF>{LocalizationManager.Instance.GetString("Haunter")}</color>");
            HaunterTasksRemainingClicked =
                 new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TasksRemainingWhenHaunterCanBeClicked"), 5, 1, 15, 1);
            HaunterTasksRemainingAlert =
                 new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TasksRemainingWhenAlertIsSent"), 1, 1, 5, 1);
            HaunterRevealsNeutrals = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("HaunterRevealsNeutralRoles"), false);
            HaunterCanBeClickedBy = new CustomStringOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("WhoCanClickHaunter"), new[] { LocalizationManager.Instance.GetString("All"), LocalizationManager.Instance.GetString("Non-Crew"), LocalizationManager.Instance.GetString("ImpsOnly") });

            Investigator =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#00B3B3FF>{LocalizationManager.Instance.GetString("Investigator")}</color>");
            FootprintSize = new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("FootprintSize"), 4f, 1f, 10f, 1f);
            FootprintInterval =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("FootprintInterval"), 0.1f, 0.05f, 1f, 0.05f, CooldownFormat);
            FootprintDuration = new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("FootprintDuration"), 10f, 1f, 15f, 0.5f, CooldownFormat);
            AnonymousFootPrint = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("AnonymousFootprint"), false);
            VentFootprintVisible = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("FootprintVentVisible"), false);

            Mystic =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#4D99E6FF>{LocalizationManager.Instance.GetString("Mystic")}</color>");
            MysticArrowDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("DeadBodyArrowDuration"), 0.1f, 0f, 1f, 0.05f, CooldownFormat);

            Oracle =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#BF00BFFF>{LocalizationManager.Instance.GetString("Oracle")}</color>");
            ConfessCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("ConfessCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            RevealAccuracy = new CustomNumberOption(num++, MultiMenu.crewmate, "Reveal Accuracy", 80f, 0f, 100f, 10f,
                PercentFormat);
            NeutralBenignShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("NeutralBenignRolesShowEvil"), false);
            NeutralEvilShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("NeutralEvilRolesShowEvil"), false);
            NeutralKillingShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("NeutralKillingRolesShowEvil"), true);

            Seer =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#FFCC80FF>{LocalizationManager.Instance.GetString("Seer")}</color>");
            SeerCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SeerCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            CrewKillingRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("CrewmateKillingRolesAreRed"), false);
            NeutBenignRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("NeutralBenignRolesAreRed"), false);
            NeutEvilRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("NeutralEvilRolesAreRed"), false);
            NeutKillingRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("NeutralKillingRolesAreRed"), true);
            TraitorColourSwap =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TraitorDoesNotSwapColours"), false);

            Snitch = new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#D4AF37FF>{LocalizationManager.Instance.GetString("Snitch")}</color>");
            SnitchSeesNeutrals = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SnitchSeesNeutralRoles"), false);
            SnitchTasksRemaining =
                 new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TasksRemainingWhenRevealed"), 1, 1, 5, 1);
            SnitchSeesImpInMeeting = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SnitchSeesImpostorsInMeetings"), true);
            SnitchSeesTraitor = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SnitchSeesTraitor"), true);

            Spy =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#CCA3CCFF>{LocalizationManager.Instance.GetString("Spy")}</color>");
            WhoSeesDead = new CustomStringOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("WhoSeesDeadBodiesOnAdmin"),
                new[] { LocalizationManager.Instance.GetString("Nobody"), LocalizationManager.Instance.GetString("Spy"), LocalizationManager.Instance.GetString("Everyone But Spy"), LocalizationManager.Instance.GetString("Everyone") });

            Tracker =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#009900FF>{LocalizationManager.Instance.GetString("Tracker")}</color>");
            UpdateInterval =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("ArrowUpdateInterval"), 5f, 0.5f, 15f, 0.5f, CooldownFormat);
            TrackCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TrackCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            ResetOnNewRound = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TrackerArrowsResetAfterEachRound"), false);
            MaxTracks = new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("MaximumNumberOfTracksPerRound"), 5, 1, 15, 1);

            Trapper =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#A7D1B3FF>{LocalizationManager.Instance.GetString("Trapper")}</color>");
            MinAmountOfTimeInTrap =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("MinAmountOfTimeInTrapToRegister"), 1f, 0f, 15f, 0.5f, CooldownFormat);
            TrapCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TrapCooldown"), 25f, 10f, 40f, 2.5f, CooldownFormat);
            TrapsRemoveOnNewRound =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TrapsRemovedAfterEachRound"), true);
            MaxTraps =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("MaximumNumberOfTrapsPerGame"), 5, 1, 15, 1);
            TrapSize =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TrapSize"), 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat);
            MinAmountOfPlayersInTrap =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("MinimumNumberOfRolesRequiredToTriggerTrap"), 3, 1, 5, 1);

            Sheriff =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#FFFF00FF>{LocalizationManager.Instance.GetString("Sheriff")}</color>");
            SheriffKillOther =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffMiskillKillsCrewmate"), false);
            SheriffKillsDoomsayer =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffKillsDoomsayer"), false);
            SheriffKillsExecutioner =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffKillsExecutioner"), false);
            SheriffKillsJester =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffKillsJester"), false);
            SheriffKillsArsonist =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffKillsArsonist"), false);
            SheriffKillsGlitch =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffKillsTheGlitch"), false);
            SheriffKillsJuggernaut =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffKillsJuggernaut"), false);
            SheriffKillsPlaguebearer =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffKillsPlaguebearer"), false);
            SheriffKillsVampire =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffKillsVampire"), false);
            SheriffKillsWerewolf =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffKillsWerewolf"), false);
            SheriffKillCd =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffKillCooldown"), 25f, 10f, 40f, 2.5f, CooldownFormat);
            SheriffBodyReport = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SheriffCanReportWhoTheyveKilled"));

            VampireHunter =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#B3B3E6FF>{LocalizationManager.Instance.GetString("VampireHunter")}</color>");
            StakeCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("StakeCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxFailedStakesPerGame = new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("MaximumFailedStakesPerGame"), 5, 1, 15, 1);
            CanStakeRoundOne = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("CanStakeRoundOne"), false);
            SelfKillAfterFinalStake = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SelfKillOnFailureToKillAVampWithAllStakes"), false);
            BecomeOnVampDeaths =
                new CustomStringOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("WhatVampireHunterBecomesOnAllVampireDeaths"), 
                new[] { LocalizationManager.Instance.GetString("Crewmate"), LocalizationManager.Instance.GetString("Sheriff"), LocalizationManager.Instance.GetString("Veteran"), LocalizationManager.Instance.GetString("Vigilante") });

            Veteran =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#998040FF>{LocalizationManager.Instance.GetString("Veteran")}</color>");
            KilledOnAlert =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("CanBeKilledOnAlert"), false);
            AlertCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("AlertCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            AlertDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("AlertDuration"), 10f, 5f, 15f, 1f, CooldownFormat);
            MaxAlerts = new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("MaximumNumberOfAlerts"), 5, 1, 15, 1);

            Vigilante = new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#FFFF99FF>{LocalizationManager.Instance.GetString("Vigilante")}</color>");
            VigilanteKills = new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("NumberOfVigilanteKills"), 1, 1, 15, 1);
            VigilanteMultiKill = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("VigilanteCanKillMoreThanOncePerMeeting"), false);
            VigilanteGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("VigilanteCanGuessNeutralBenignRoles"), false);
            VigilanteGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("VigilanteCanGuessNeutralEvilRoles"), false);
            VigilanteGuessNeutralKilling = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("VigilanteCanGuessNeutralKillingRoles"), false);
            VigilanteGuessLovers = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("VigilanteCanGuessLovers"), false);
            VigilanteAfterVoting = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("VigilanteCanGuessAfterVoting"), false);

            Altruist = new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#660000FF>{LocalizationManager.Instance.GetString("Altruist")}</color>");
            ReviveDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("AltruistReviveDuration"), 10f, 1f, 15f, 1f, CooldownFormat);
            AltruistTargetBody =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TargetsBodyDisappearsOnBeginningOfRevive"), false);

            Medic =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#006600FF>{LocalizationManager.Instance.GetString("Medic")}</color>");
            ShowShielded =
                new CustomStringOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("ShowShieldedPlayer"),
                    new[] { LocalizationManager.Instance.GetString("Self"), LocalizationManager.Instance.GetString("Medic"), LocalizationManager.Instance.GetString("Self+Medic"), LocalizationManager.Instance.GetString("Everyone") });
            WhoGetsNotification =
                new CustomStringOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("WhoGetsMurderAttemptIndicator"),
                    new[] { LocalizationManager.Instance.GetString("Medic"), LocalizationManager.Instance.GetString("Shielded"), LocalizationManager.Instance.GetString("Everyone"), LocalizationManager.Instance.GetString("Nobody") });
            ShieldBreaks = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("ShieldBreaksOnMurderAttempt"), false);
            MedicReportSwitch = new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("ShowMedicReports"));
            MedicReportNameDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TimeWhereMedicWillHaveName"), 0f, 0f, 60f, 2.5f,
                    CooldownFormat);
            MedicReportColorDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TimeWhereMedicWillHaveColorType"), 15f, 0f, 60f, 2.5f,
                    CooldownFormat);

            Engineer =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#FFA60AFF>{LocalizationManager.Instance.GetString("Engineer")}</color>");
            MaxFixes =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("MaximumNumberOfFixes"), 5, 1, 15, 1);

            Medium =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#A680FFFF>{LocalizationManager.Instance.GetString("Medium")}</color>");
            MediateCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("MediateCooldown"), 10f, 1f, 15f, 1f, CooldownFormat);
            ShowMediatePlayer =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("RevealAppearanceOfMediateTarget"), true);
            ShowMediumToDead =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("RevealTheMediumToTheMediateTarget"), true);
            DeadRevealed =
                new CustomStringOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("WhoIsRevealedWithMediate"), 
                new[] { LocalizationManager.Instance.GetString("OldestDead"), LocalizationManager.Instance.GetString("NewestDead"), LocalizationManager.Instance.GetString("AllDead") });

            Prosecutor =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#B38000FF>{LocalizationManager.Instance.GetString("Prosecutor")}</color>");
            ProsDiesOnIncorrectPros =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("ProsecutorDiesWhenTheyExileACrewmate"), false);

            Swapper =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#66E666FF>{LocalizationManager.Instance.GetString("Swapper")}</color>");
            SwapperButton =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("SwapperCanButton"), true);

            Transporter =
                new CustomHeaderOption(num++, MultiMenu.crewmate, $"<color=#00EEFFFF>{LocalizationManager.Instance.GetString("Transporter")}</color>");
            TransportCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TransportCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            TransportMaxUses =
                new CustomNumberOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("MaximumNumberOfTransports"), 5, 1, 15, 1);
            TransporterVitals =
                new CustomToggleOption(num++, MultiMenu.crewmate, LocalizationManager.Instance.GetString("TransporterCanUseVitals"), false);

            Amnesiac = new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#80B2FFFF>{LocalizationManager.Instance.GetString("Amnesiac")}</color>");
            RememberArrows =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("AmnesiacGetsArrowsPointingToDeadBodies"), false);
            RememberArrowDelay =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("TimeAfterDeathArrowAppears"), 5f, 0f, 15f, 1f, CooldownFormat);

            GuardianAngel =
                new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#B3FFFFFF>{LocalizationManager.Instance.GetString("GuardianAngel")}</color>");
            ProtectCd =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("ProtectCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            ProtectDuration =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("ProtectDuration"), 10f, 5f, 15f, 1f, CooldownFormat);
            ProtectKCReset =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("KillCooldownResetWhenProtected"), 2.5f, 0f, 15f, 0.5f, CooldownFormat);
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("KillCooldownResetWhenProtected"), 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxProtects =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("MaximumNumberOfProtects"), 5, 1, 15, 1);
            ShowProtect =
                new CustomStringOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("ShowProtectedPlayer"),
                    new[] { LocalizationManager.Instance.GetString("Self"), LocalizationManager.Instance.GetString("GuardianAngel"), LocalizationManager.Instance.GetString("Self+GA"), LocalizationManager.Instance.GetString("Everyone") });
            GaOnTargetDeath = new CustomStringOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("GABecomes OnTargetDead"),
                new[] { LocalizationManager.Instance.GetString("Crew"), LocalizationManager.Instance.GetString("Amnesiac"), LocalizationManager.Instance.GetString("Survivor"), LocalizationManager.Instance.GetString("Jester") });
            GATargetKnows =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("TargetKnowsGExists"), false);
            GAKnowsTargetRole =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("GAKnowsTargetsRole"), false);
            EvilTargetPercent = new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("OddsOfTargetBeingEvil"), 20f, 0f, 100f, 10f,
                PercentFormat);

            Survivor =
                new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#FFE64DFF>{LocalizationManager.Instance.GetString("Survivor")}</color>");
            VestCd =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("VestCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            VestDuration =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("VestDuration"), 10f, 5f, 15f, 1f, CooldownFormat);
            VestKCReset =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("KillCooldownResetOnAttack"), 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxVests =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("MaximumNumberOfVests"), 5, 1, 15, 1);

            Doomsayer = new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#00FF80FF>{LocalizationManager.Instance.GetString("Doomsayer")}</color>");
            ObserveCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("ObserveCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            DoomsayerGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("DoomsayerCanGuessNeutralBenignRoles"), false);
            DoomsayerGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("DoomsayerCanGuessNeutralEvilRoles"), false);
            DoomsayerGuessNeutralKilling = new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("DoomsayerCanGuessNeutralKillingRoles"), false);
            DoomsayerGuessImpostors = new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("DoomsayerCanGuessImpostorRoles"), false);
            DoomsayerAfterVoting = new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("DoomsayerCanGuessAfterVoting"), false);
            DoomsayerGuessesToWin = new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("NumberOfDoomsayerKillsToWin"), 3, 1, 5, 1);

            Executioner =
                new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#8C4005FF>{LocalizationManager.Instance.GetString("Executioner")}</color>");
            OnTargetDead = new CustomStringOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("ExecutionerBecomesOnTargetDead"),
                new[] { "Crew", "Amnesiac", "Survivor", "Jester" });
            ExecutionerButton =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("ExecutionerCanButton"), true);
            ExecutionerTorment =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("ExecutionerTormentsPlayerOnVictory"), true);

            Jester =
                new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#FFBFCCFF>{LocalizationManager.Instance.GetString("Jester")}</color>");
            JesterButton =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("JesterCanButton"), true);
            JesterVent =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("JesterCanHideInVents"), false);
            JesterImpVision =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("JesterHasImpostorVision"), false);
            JesterHaunt =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("JesterHauntsPlayerOnVictory"), true);

            Phantom =
                new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#662962FF>{LocalizationManager.Instance.GetString("Phantom")}</color>");
            PhantomTasksRemaining =
                 new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("TasksRemainingWhenPhantomCanBeClicked"), 5, 1, 15, 1);
            PhantomSpook =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("PhantomSpooksPlayer OnVictory"), true);

            Arsonist = new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#FF4D00FF>{LocalizationManager.Instance.GetString("Arsonist")}</color>");
            DouseCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("DouseCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxDoused =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("MaximumAlivePlayersDoused"), 5, 1, 15, 1);
            ArsoImpVision =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("ArsonistHasImpostoVision"), false);
            IgniteCdRemoved =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("IgniteCooldownRemovedWhenArsonistIsLastKiller"), false);

            Juggernaut =
                new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#8C004DFF>{LocalizationManager.Instance.GetString("Juggernaut")}</color>");
            JuggKillCooldown = new CustomNumberOption(num++, MultiMenu.neutral, "Juggernaut Initial Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ReducedKCdPerKill = new CustomNumberOption(num++, MultiMenu.neutral, "Reduced Kill Cooldown Per Kill", 5f, 2.5f, 10f, 2.5f, CooldownFormat);
            JuggVent =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("JuggernautCanVent"), false);

            Plaguebearer = new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#E6FFB3FF>{LocalizationManager.Instance.GetString("Plaguebearer")}</color>");
            InfectCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("InfectCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            PestKillCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("PestilenceKillCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            PestVent =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("PestilenceCanVent"), false);

            TheGlitch =
                new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#00FF00FF>{LocalizationManager.Instance.GetString("TheGlitch")}</color>");
            MimicCooldownOption = new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("MimicCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            MimicDurationOption = new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("MimicDuration"), 10f, 1f, 15f, 1f, CooldownFormat);
            HackCooldownOption = new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("HackCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            HackDurationOption = new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("HackDuration"), 10f, 1f, 15f, 1f, CooldownFormat);
            GlitchKillCooldownOption =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("GlitchKillCooldown"), 25f, 10f, 120f, 2.5f, CooldownFormat);
            GlitchHackDistanceOption =
                new CustomStringOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("GlitchHackDistance"),
                new[] { LocalizationManager.Instance.GetString("Short"), LocalizationManager.Instance.GetString("Normal"), LocalizationManager.Instance.GetString("Long") });
            GlitchVent =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("GlitchCanVent"), false);

            Vampire = new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#262626FF>{LocalizationManager.Instance.GetString("Vampire")}</color>");
            BiteCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("VampireBiteCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            VampImpVision =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("VampiresHaveImpostorVision"), false);
            VampVent =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("VampiresCanVent"), false);
            NewVampCanAssassin =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("NewVampireCanAssassinate"), false);
            MaxVampiresPerGame =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("MaximumVampiresPerGame"), 2, 2, 5, 1);
            CanBiteNeutralBenign =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("CanConvertNeutralBenignRoles"), false);
            CanBiteNeutralEvil =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("CanConvertNeutralEvilRoles"), false);

            Werewolf = new CustomHeaderOption(num++, MultiMenu.neutral, $"<color=#A86629FF>{LocalizationManager.Instance.GetString("Werewolf")}</color>");
            RampageCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("RampageCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            RampageDuration =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("RampageDuration"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            RampageKillCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("RampageKillCooldown"), 10f, 0.5f, 15f, 0.5f, CooldownFormat);
            WerewolfVent =
                new CustomToggleOption(num++, MultiMenu.neutral, LocalizationManager.Instance.GetString("WerewolfCanVenWhenRampaged"), false);

            Escapist =
                new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Escapist")}</color>");
            EscapeCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("RecallCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            EscapistVent =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("EscapistCanVent"), false);

            Grenadier =
                new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Grenadier")}</color>");
            GrenadeCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("FlashGrenadeCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            GrenadeDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("FlashGrenadeDuration"), 10f, 5f, 15f, 1f, CooldownFormat);
            FlashRadius =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("FlashRadius"), 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            GrenadierIndicators =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("IndicateFlashedCrewmates"), false);
            GrenadierVent =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("GrenadierCanVent"), false);

            Morphling =
                new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Morphling")}</color>");
            MorphlingCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("MorphlingCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            MorphlingDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("MorphlingDuration"), 10f, 5f, 15f, 1f, CooldownFormat);
            MorphlingVent =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("MorphlingCanVent"), false);
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("MorphlingCanVent"), false);

            Swooper = new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Swooper")}</color>");
            SwoopCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("SwoopCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            SwoopDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("SwoopDuration"), 10f, 5f, 15f, 1f, CooldownFormat);
            SwooperVent =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("SwooperCanVent"), false);

            Venerer = new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Venerer")}</color>");
            AbilityCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("AbilityCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            AbilityDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("AbilityDuration"), 10f, 5f, 15f, 1f, CooldownFormat);
            SprintSpeed = new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("SprintSpeed"), 1.25f, 1.05f, 2.5f, 0.05f, MultiplierFormat);
            FreezeSpeed = new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("FreezeSpeed"), 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat);

            Bomber =
                new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Bomber")}</color>");
            DetonateDelay =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("DetonateDelay"), 5f, 1f, 15f, 1f, CooldownFormat);
            MaxKillsInDetonation =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("MaxKillsInDetonation"), 5, 1, 15, 1);
            DetonateRadius =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("DetonateRadius"), 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat);
            BomberVent =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("BomberCanVent"), false);

            Traitor = new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Traitor")}</color>");
            LatestSpawn = new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("MinimumPeopleAliveWhenTraitorCanSpawn"), 5, 3, 15, 1);
            NeutralKillingStopsTraitor =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("TraitorWon'tSpawnIfAnyNeutralKillingIsAlive"), false);

            Warlock = new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Warlock")}</color>");
            ChargeUpDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("TimeItTakesToFullyCharge"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            ChargeUseDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("TimeItTakesToUseFullCharge"), 1f, 0.05f, 5f, 0.05f, CooldownFormat);

            Blackmailer = new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Blackmailer")}</color>");
            BlackmailCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("InitialBlackmailCooldown"), 10f, 1f, 15f, 1f, CooldownFormat);

            Miner = new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Miner")}</color>");
            MineCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("MineCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);

            Undertaker = new CustomHeaderOption(num++, MultiMenu.imposter, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Undertaker")}</color>");
            DragCooldown = new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("DragCooldown"), 25f, 10f, 60f, 2.5f, CooldownFormat);
            UndertakerDragSpeed =
                new CustomNumberOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("UndertakerDragSpeed"), 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat);
            UndertakerVent =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("UndertakerCanVent"), false);
            UndertakerVentWithBody =
                new CustomToggleOption(num++, MultiMenu.imposter, LocalizationManager.Instance.GetString("UndertakerCanVentWhileDragging"), false);

            Bait = new CustomHeaderOption(num++, MultiMenu.modifiers, $"<color=#00B3B3FF>{LocalizationManager.Instance.GetString("Bait")}</color>");
            BaitMinDelay = new CustomNumberOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("MinimumDelayfortheBaitReport"), 0f, 0f, 15f, 0.5f, CooldownFormat);
            BaitMaxDelay = new CustomNumberOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("MaximumDelayfortheBaitReport"), 1f, 0f, 15f, 0.5f, CooldownFormat);

            Diseased = new CustomHeaderOption(num++, MultiMenu.modifiers, $"<color=#808080FF>{LocalizationManager.Instance.GetString("Diseased")}</color>");
            DiseasedKillMultiplier = new CustomNumberOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("DiseasedKillMultiplier"), 3f, 1.5f, 5f, 0.5f, MultiplierFormat);

            Frosty = new CustomHeaderOption(num++, MultiMenu.modifiers, $"<color=#99FFFFFF>{LocalizationManager.Instance.GetString("Frosty")}</color>");
            ChillDuration = new CustomNumberOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("ChillDuration"), 10f, 1f, 15f, 1f, CooldownFormat);
            ChillStartSpeed = new CustomNumberOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("ChillStartSpeed"), 0.75f, 0.25f, 0.95f, 0.05f, MultiplierFormat);

            Flash = new CustomHeaderOption(num++, MultiMenu.modifiers, $"<color=#FF8080FF>{LocalizationManager.Instance.GetString("Flash")}</color>");
            FlashSpeed = new CustomNumberOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("FlashSpeed"), 1.25f, 1.05f, 2.5f, 0.05f, MultiplierFormat);

            Giant = new CustomHeaderOption(num++, MultiMenu.modifiers, $"<color=#FFB34DFF>{LocalizationManager.Instance.GetString("Giant")}</color>");
            GiantSlow = new CustomNumberOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("GiantSpeed"), 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat);

            Lovers =
                new CustomHeaderOption(num++, MultiMenu.modifiers, $"<color=#FF66CCFF>{LocalizationManager.Instance.GetString("Lovers")}</color>");
            BothLoversDie = new CustomToggleOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("BothLoversDie"));
            LovingImpPercent = new CustomNumberOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("LovingImpostorProbability"), 20f, 0f, 100f, 10f,
                PercentFormat);
            NeutralLovers = new CustomToggleOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("NeutralRolesCanBeLovers"));

            Underdog = new CustomHeaderOption(num++, MultiMenu.modifiers, $"<color=#FF0000FF>{LocalizationManager.Instance.GetString("Underdog")}</color>");
            UnderdogKillBonus = new CustomNumberOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("KillCooldownBonus"), 5f, 2.5f, 10f, 2.5f, CooldownFormat);
            UnderdogIncreasedKC = new CustomToggleOption(num++, MultiMenu.modifiers, LocalizationManager.Instance.GetString("IncreasedKillCooldownWhen2PlusImps"), true);
        }
    }
}
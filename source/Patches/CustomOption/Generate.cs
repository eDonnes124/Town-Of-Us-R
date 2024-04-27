using System;

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
        public static CustomNumberOption HunterOn;

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
        public static CustomNumberOption MiniOn;
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
        public static CustomNumberOption RandomMapFungle;
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
        public static CustomToggleOption GhostsDoTasks;

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

        public static CustomHeaderOption Hunter;
        public static CustomNumberOption HunterKillCd;
        public static CustomNumberOption HunterStalkCd;
        public static CustomNumberOption HunterStalkDuration;
        public static CustomNumberOption HunterStalkUses;
        public static CustomToggleOption HunterBodyReport;

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
        public static CustomToggleOption BlackmailInvisible;

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
        public static CustomToggleOption DoomsayerCantObserve;

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

        public static CustomHeaderOption Mini;
        public static CustomNumberOption MiniSpeed;

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

        public static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";
        private static Func<object, string> MultiplierFormat { get; } = value => $"{value:0.0#}x";


        public static void GenerateAll()
        {
            var num = 0;

            Patches.ExportButton = new Export(num++);
            Patches.ImportButton = new Import(num++);

            CrewInvestigativeRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "Crewmate Investigative Roles");
            AurialOn = new CustomNumberOption(num++, MultiMenu.crewmate, $"<color=#B34D99FF>Aurial</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DetectiveOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#4D4DFFFF>Detective</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            HaunterOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#D3D3D3FF>Haunter</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            InvestigatorOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#00B3B3FF>Investigator</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MysticOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#4D99E6FF>Mystic</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            OracleOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#BF00BFFF>Oracle</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SeerOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFCC80FF>Seer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SnitchOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SpyOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#CCA3CCFF>Spy</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TrackerOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#009900FF>Tracker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TrapperOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#A7D1B3FF>Trapper</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewKillingRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "Crewmate Killing Roles");
            HunterOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#29AB87FF>Hunter</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SheriffOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Sheriff</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VampireHunterOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#B3B3E6FF>Vampire Hunter</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VeteranOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#998040FF>Veteran</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VigilanteOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFFF99FF>Vigilante</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewProtectiveRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "Crewmate Protective Roles");
            AltruistOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#660000FF>Altruist</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MedicOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#006600FF>Medic</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewSupportRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "Crewmate Support Roles");
            EngineerOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFA60AFF>Engineer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ImitatorOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#B3D94DFF>Imitator</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MayorOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#704FA8FF>Mayor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MediumOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#A680FFFF>Medium</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ProsecutorOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#B38000FF>Prosecutor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwapperOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#66E666FF>Swapper</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TransporterOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#00EEFFFF>Transporter</color>", 0f, 0f, 100f, 10f,
                PercentFormat);


            NeutralBenignRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "Neutral Benign Roles");
            AmnesiacOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#80B2FFFF>Amnesiac</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GuardianAngelOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#B3FFFFFF>Guardian Angel</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SurvivorOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#FFE64DFF>Survivor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            NeutralEvilRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "Neutral Evil Roles");
            DoomsayerOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#00FF80FF>Doomsayer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ExecutionerOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#8C4005FF>Executioner</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JesterOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#FFBFCCFF>Jester</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PhantomOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            NeutralKillingRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "Neutral Killing Roles");
            ArsonistOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#FF4D00FF>Arsonist</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PlaguebearerOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#E6FFB3FF>Plaguebearer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GlitchOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#00FF00FF>The Glitch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VampireOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#262626FF>Vampire</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            WerewolfOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#A86629FF>Werewolf</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorConcealingRoles = new CustomHeaderOption(num++, MultiMenu.imposter, "Impostor Concealing Roles");
            EscapistOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Escapist</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GrenadierOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Grenadier</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MorphlingOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Morphling</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwooperOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Swooper</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VenererOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Venerer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorKillingRoles = new CustomHeaderOption(num++, MultiMenu.imposter, "Impostor Killing Roles");
            BomberOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Bomber</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TraitorOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Traitor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            WarlockOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Warlock</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorSupportRoles = new CustomHeaderOption(num++, MultiMenu.imposter, "Impostor Support Roles");
            BlackmailerOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Blackmailer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JanitorOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Janitor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MinerOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Miner</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            UndertakerOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Undertaker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewmateModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, "Crewmate Modifiers");
            AftermathOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#A6FFA6FF>Aftermath</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            BaitOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#00B3B3FF>Bait</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DiseasedOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#808080FF>Diseased</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            FrostyOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#99FFFFFF>Frosty</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MultitaskerOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF804DFF>Multitasker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TorchOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FFFF99FF>Torch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            GlobalModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, "Global Modifiers");
            ButtonBarryOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#E600FFFF>Button Barry</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            FlashOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF8080FF>Flash</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GiantOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FFB34DFF>Giant</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            LoversOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF66CCFF>Lovers</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MiniOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#b0a1ff>Mini</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            RadarOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF0080FF>Radar</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SleuthOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#803333FF>Sleuth</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TiebreakerOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#99E699FF>Tiebreaker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, "Impostor Modifiers");
            DisperserOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF0000FF>Disperser</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DoubleShotOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF0000FF>Double Shot</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            UnderdogOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF0000FF>Underdog</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            GameModeSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Game Mode Settings");
            GameMode = new CustomStringOption(num++, MultiMenu.main, "Game Mode", new[] {"Classic", "All Any", "Killing Only", "Cultist" });

            ClassicSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Classic Game Mode Settings", GameMode, 0);
            MinNeutralBenignRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Min Neutral Benign Roles", 1, 0, 3, 1, dependsOn: GameMode, stringoptionneedstobe: 0);
            MaxNeutralBenignRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Max Neutral Benign Roles", 1, 0, 3, 1, dependsOn: GameMode, stringoptionneedstobe: 0);
            MinNeutralEvilRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Min Neutral Evil Roles", 1, 0, 3, 1, dependsOn: GameMode, stringoptionneedstobe: 0);
            MaxNeutralEvilRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Max Neutral Evil Roles", 1, 0, 3, 1, dependsOn: GameMode, stringoptionneedstobe: 0);
            MinNeutralKillingRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Min Neutral Killing Roles", 1, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 0);
            MaxNeutralKillingRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Max Neutral Killing Roles", 1, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 0);

            AllAnySettings = new CustomHeaderOption(num++, MultiMenu.main, "All Any Settings", GameMode, 1);
            RandomNumberImps = new CustomToggleOption(num++, MultiMenu.main, "Random Number Of Impostors", true, GameMode, 1);

            KillingOnlySettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Killing Only Settings", GameMode, 2);
            NeutralRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Neutral Roles", 1, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 2);
            VeteranCount =
                new CustomNumberOption(num++, MultiMenu.main, "Veteran Count", 1, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 2);
            VigilanteCount =
                new CustomNumberOption(num++, MultiMenu.main, "Vigilante Count", 1, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 2);
            AddArsonist = new CustomToggleOption(num++, MultiMenu.main, "Add Arsonist", true, GameMode, 2);
            AddPlaguebearer = new CustomToggleOption(num++, MultiMenu.main, "Add Plaguebearer", true, GameMode, 2);

            CultistSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Cultist Settings", GameMode, 3);
            MayorCultistOn = new CustomNumberOption(num++, MultiMenu.main, "<color=#704FA8FF>Mayor</color> (Cultist Mode)", 100f, 0f, 100f, 10f,
                PercentFormat, GameMode, 3);
            SeerCultistOn = new CustomNumberOption(num++, MultiMenu.main, "<color=#FFCC80FF>Seer</color> (Cultist Mode)", 100f, 0f, 100f, 10f,
                PercentFormat, GameMode, 3);
            SheriffCultistOn = new CustomNumberOption(num++, MultiMenu.main, "<color=#FFFF00FF>Sheriff</color> (Cultist Mode)", 100f, 0f, 100f, 10f,
                PercentFormat, GameMode, 3);
            SurvivorCultistOn = new CustomNumberOption(num++, MultiMenu.main, "<color=#FFE64DFF>Survivor</color> (Cultist Mode)", 100f, 0f, 100f, 10f,
                PercentFormat, GameMode, 3);
            NumberOfSpecialRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Number Of Special Roles", 4, 0, 4, 1, dependsOn: GameMode, stringoptionneedstobe: 3);
            MaxChameleons =
                new CustomNumberOption(num++, MultiMenu.main, "Max Chameleons", 3, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 3);
            MaxEngineers =
                new CustomNumberOption(num++, MultiMenu.main, "Max Engineers", 3, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 3);
            MaxInvestigators =
                new CustomNumberOption(num++, MultiMenu.main, "Max Investigators", 3, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 3);
            MaxMystics =
                new CustomNumberOption(num++, MultiMenu.main, "Max Mystics", 3, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 3);
            MaxSnitches =
                new CustomNumberOption(num++, MultiMenu.main, "Max Snitches", 3, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 3);
            MaxSpies =
                new CustomNumberOption(num++, MultiMenu.main, "Max Spies", 3, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 3);
            MaxTransporters =
                new CustomNumberOption(num++, MultiMenu.main, "Max Transporters", 3, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 3);
            MaxVigilantes =
                new CustomNumberOption(num++, MultiMenu.main, "Max Vigilantes", 3, 0, 5, 1, dependsOn: GameMode, stringoptionneedstobe: 3);
            WhisperCooldown =
                new CustomNumberOption(num++, MultiMenu.main, "Initial Whisper Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GameMode, 3);
            IncreasedCooldownPerWhisper =
                new CustomNumberOption(num++, MultiMenu.main, "Increased Cooldown Per Whisper", 5f, 0f, 15f, 0.5f, CooldownFormat, GameMode, 3);
            WhisperRadius =
                new CustomNumberOption(num++, MultiMenu.main, "Whisper Radius", 1f, 0.25f, 5f, 0.25f, MultiplierFormat, GameMode, 3);
            ConversionPercentage = new CustomNumberOption(num++, MultiMenu.main, "Conversion Percentage", 25f, 0f, 100f, 5f,
                PercentFormat, GameMode, 3);
            DecreasedPercentagePerConversion = new CustomNumberOption(num++, MultiMenu.main, "Decreased Conversion Percentage Per Conversion", 5f, 0f, 15f, 1f,
                PercentFormat, GameMode, 3);
            ReviveCooldown =
                new CustomNumberOption(num++, MultiMenu.main, "Initial Revive Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GameMode, 3);
            IncreasedCooldownPerRevive =
                new CustomNumberOption(num++, MultiMenu.main, "Increased Cooldown Per Revive", 25f, 10f, 60f, 2.5f, CooldownFormat, GameMode, 3);
            MaxReveals = new CustomNumberOption(num++, MultiMenu.main, "Maximum Number Of Reveals", 5, 1, 15, 1, dependsOn: GameMode, stringoptionneedstobe: 3);

            MapSettings = new CustomHeaderOption(num++, MultiMenu.main, "Map Settings");
            RandomMapEnabled = new CustomToggleOption(num++, MultiMenu.main, "Choose Random Map", false);
            RandomMapSkeld = new CustomNumberOption(num++, MultiMenu.main, "Skeld Chance", 0f, 0f, 100f, 10f, PercentFormat, RandomMapEnabled);
            RandomMapMira = new CustomNumberOption(num++, MultiMenu.main, "Mira Chance", 0f, 0f, 100f, 10f, PercentFormat, RandomMapEnabled);
            RandomMapPolus = new CustomNumberOption(num++, MultiMenu.main, "Polus Chance", 0f, 0f, 100f, 10f, PercentFormat, RandomMapEnabled);
            RandomMapAirship = new CustomNumberOption(num++, MultiMenu.main, "Airship Chance", 0f, 0f, 100f, 10f, PercentFormat, RandomMapEnabled);
            RandomMapFungle = new CustomNumberOption(num++, MultiMenu.main, "Fungle Chance", 0f, 0f, 100f, 10f, PercentFormat, RandomMapEnabled);
            RandomMapSubmerged = new CustomNumberOption(num++, MultiMenu.main, "Submerged Chance", 0f, 0f, 100f, 10f, PercentFormat, RandomMapEnabled);
            AutoAdjustSettings = new CustomToggleOption(num++, MultiMenu.main, "Auto Adjust Settings", false, RandomMapEnabled);
            SmallMapHalfVision = new CustomToggleOption(num++, MultiMenu.main, "Half Vision On Skeld/Mira HQ", false, RandomMapEnabled);
            SmallMapDecreasedCooldown =
                new CustomNumberOption(num++, MultiMenu.main, "Mira HQ Decreased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat, RandomMapEnabled);
            LargeMapIncreasedCooldown =
                new CustomNumberOption(num++, MultiMenu.main, "Airship/Submerged Increased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat, RandomMapEnabled);
            SmallMapIncreasedShortTasks =
                 new CustomNumberOption(num++, MultiMenu.main, "Skeld/Mira HQ Increased Short Tasks", 0, 0, 5, 1, dependsOn: RandomMapEnabled);
            SmallMapIncreasedLongTasks =
                 new CustomNumberOption(num++, MultiMenu.main, "Skeld/Mira HQ Increased Long Tasks", 0, 0, 3, 1, dependsOn: RandomMapEnabled);
            LargeMapDecreasedShortTasks =
                 new CustomNumberOption(num++, MultiMenu.main, "Airship/Submerged Decreased Short Tasks", 0, 0, 5, 1, dependsOn: RandomMapEnabled);
            LargeMapDecreasedLongTasks =
                 new CustomNumberOption(num++, MultiMenu.main, "Airship/Submerged Decreased Long Tasks", 0, 0, 3, 1, dependsOn: RandomMapEnabled);

            BetterPolusSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Better Polus Settings");
            VentImprovements = new CustomToggleOption(num++, MultiMenu.main, "Better Polus Vent Layout", false);
            VitalsLab = new CustomToggleOption(num++, MultiMenu.main, "Vitals Moved To Lab", false);
            ColdTempDeathValley = new CustomToggleOption(num++, MultiMenu.main, "Cold Temp Moved To Death Valley", false);
            WifiChartCourseSwap =
                new CustomToggleOption(num++, MultiMenu.main, "Reboot Wifi And Chart Course Swapped", false);

            CustomGameSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Custom Game Settings");
            ColourblindComms = new CustomToggleOption(num++, MultiMenu.main, "Camouflaged Comms", false);
            ImpostorSeeRoles = new CustomToggleOption(num++, MultiMenu.main, "Impostors Can See The Roles Of Their Team", false);
            DeadSeeRoles =
                new CustomToggleOption(num++, MultiMenu.main, "Dead Can See Everyone's Roles/Votes", false);
            InitialCooldowns =
                new CustomNumberOption(num++, MultiMenu.main, "Game Start Cooldowns", 10f, 10f, 30f, 2.5f, CooldownFormat);
            ParallelMedScans = new CustomToggleOption(num++, MultiMenu.main, "Parallel Medbay Scans", false);
            SkipButtonDisable = new CustomStringOption(num++, MultiMenu.main, "Disable Meeting Skip Button", new[] { "No", "Emergency", "Always" });
            HiddenRoles = new CustomToggleOption(num++, MultiMenu.main, "Enable Hidden Roles", true);
            FirstDeathShield = new CustomToggleOption(num++, MultiMenu.main, "First Death Shield Next Game", false);
            NeutralEvilWinEndsGame = new CustomToggleOption(num++, MultiMenu.main, "Neutral Evil Win Ends Game", true);
            GhostsDoTasks = new CustomToggleOption(num++, MultiMenu.main, "Ghosts Do Tasks", true);

            TaskTrackingSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Task Tracking Settings");
            SeeTasksDuringRound = new CustomToggleOption(num++, MultiMenu.main, "See Tasks During Round", false);
            SeeTasksDuringMeeting = new CustomToggleOption(num++, MultiMenu.main, "See Tasks During Meetings", false);
            SeeTasksWhenDead = new CustomToggleOption(num++, MultiMenu.main, "See Tasks When Dead", true);

            Assassin = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Assassin Ability</color>");
            NumberOfImpostorAssassins = new CustomNumberOption(num++, MultiMenu.imposter, "Number Of Impostor Assassins", 1, 0, 4, 1);
            NumberOfNeutralAssassins = new CustomNumberOption(num++, MultiMenu.imposter, "Number Of Neutral Assassins", 1, 0, 5, 1);
            AmneTurnImpAssassin = new CustomToggleOption(num++, MultiMenu.imposter, "Amnesiac Turned Impostor Gets Ability", false);
            AmneTurnNeutAssassin = new CustomToggleOption(num++, MultiMenu.imposter, "Amnesiac Turned Neutral Killing Gets Ability", false);
            TraitorCanAssassin = new CustomToggleOption(num++, MultiMenu.imposter, "Traitor Gets Ability", false);
            AssassinKills = new CustomNumberOption(num++, MultiMenu.imposter, "Number Of Assassin Kills", 1, 1, 15, 1);
            AssassinMultiKill = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Kill More Than Once Per Meeting", false);
            AssassinCrewmateGuess = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess \"Crewmate\"", false);
            AssassinGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Neutral Benign Roles", false);
            AssassinGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Neutral Evil Roles", false);
            AssassinGuessNeutralKilling = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Neutral Killing Roles", false);
            AssassinGuessImpostors = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Impostor Roles", false);
            AssassinGuessModifiers = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Crewmate Modifiers", false);
            AssassinGuessLovers = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Lovers", false);
            AssassinateAfterVoting = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess After Voting", false);

            Aurial =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#B34D99FF>Aurial</color>", AurialOn);
            RadiateRange =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Radiate Range", 1f, 0.25f, 5f, 0.25f, MultiplierFormat, AurialOn);
            RadiateCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Radiate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, AurialOn);
            RadiateInvis =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Radiate See Delay", 10f, 0f, 15f, 1f, CooldownFormat, AurialOn);
            RadiateCount =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Radiate Uses To See", 3, 1, 5, 1, dependsOn: AurialOn);
            RadiateSucceedChance =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Radiate Succeed Chance", 100f, 0f, 100f, 10f, PercentFormat, AurialOn);

            Detective =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#4D4DFFFF>Detective</color>", DetectiveOn);
            ExamineCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Examine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, DetectiveOn);
            DetectiveReportOn = new CustomToggleOption(num++, MultiMenu.crewmate, "Show Detective Reports", true, DetectiveOn);
            DetectiveRoleDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Time Where Detective Will Have Role", 15f, 0f, 60f, 2.5f,
                    CooldownFormat, DetectiveOn);
            DetectiveFactionDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Time Where Detective Will Have Faction", 30f, 0f, 60f, 2.5f,
                    CooldownFormat, DetectiveOn);
            CanDetectLastKiller = new CustomToggleOption(num++, MultiMenu.crewmate, "Can Detect Last Killer", false, DetectiveOn);

            Haunter =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#d3d3d3FF>Haunter</color>", HaunterOn);
            HaunterTasksRemainingClicked =
                 new CustomNumberOption(num++, MultiMenu.crewmate, "Tasks Remaining When Haunter Can Be Clicked", 5, 1, 15, 1, dependsOn: HaunterOn);
            HaunterTasksRemainingAlert =
                 new CustomNumberOption(num++, MultiMenu.crewmate, "Tasks Remaining When Alert Is Sent", 1, 1, 5, 1, dependsOn: HaunterOn);
            HaunterRevealsNeutrals = new CustomToggleOption(num++, MultiMenu.crewmate, "Haunter Reveals Neutral Roles", false, HaunterOn);
            HaunterCanBeClickedBy = new CustomStringOption(num++, MultiMenu.crewmate, "Who Can Click Haunter", new[] { "All", "Non-Crew", "Imps Only" }, HaunterOn);

            Investigator =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#00B3B3FF>Investigator</color>", InvestigatorOn);
            FootprintSize = new CustomNumberOption(num++, MultiMenu.crewmate, "Footprint Size", 4f, 1f, 10f, 1f, dependsOn: InvestigatorOn);
            FootprintInterval =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Footprint Interval", 0.1f, 0.05f, 1f, 0.05f, CooldownFormat, InvestigatorOn);
            FootprintDuration = new CustomNumberOption(num++, MultiMenu.crewmate, "Footprint Duration", 10f, 1f, 15f, 0.5f, CooldownFormat, InvestigatorOn);
            AnonymousFootPrint = new CustomToggleOption(num++, MultiMenu.crewmate, "Anonymous Footprint", false, InvestigatorOn);
            VentFootprintVisible = new CustomToggleOption(num++, MultiMenu.crewmate, "Footprint Vent Visible", false, InvestigatorOn);

            Mystic =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#4D99E6FF>Mystic</color>", MysticOn);
            MysticArrowDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Dead Body Arrow Duration", 0.1f, 0f, 1f, 0.05f, CooldownFormat, MysticOn);

            Oracle =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#BF00BFFF>Oracle</color>", OracleOn);
            ConfessCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Confess Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, OracleOn);
            RevealAccuracy = new CustomNumberOption(num++, MultiMenu.crewmate, "Reveal Accuracy", 80f, 0f, 100f, 10f,
                PercentFormat, OracleOn);
            NeutralBenignShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Benign Roles Show Evil", false, OracleOn);
            NeutralEvilShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Evil Roles Show Evil", false, OracleOn);
            NeutralKillingShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Killing Roles Show Evil", true, OracleOn);

            Seer =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFCC80FF>Seer</color>", SeerOn);
            SeerCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Seer Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SeerOn);
            CrewKillingRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Crewmate Killing Roles Are Red", false, SeerOn);
            NeutBenignRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Benign Roles Are Red", false, SeerOn);
            NeutEvilRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Evil Roles Are Red", false, SeerOn);
            NeutKillingRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Killing Roles Are Red", true, SeerOn);
            TraitorColourSwap =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Traitor Does Not Swap Colours", false, SeerOn);

            Snitch = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color>", SnitchOn);
            SnitchSeesNeutrals = new CustomToggleOption(num++, MultiMenu.crewmate, "Snitch Sees Neutral Roles", false, SnitchOn);
            SnitchTasksRemaining =
                 new CustomNumberOption(num++, MultiMenu.crewmate, "Tasks Remaining When Revealed", 1, 1, 5, 1, dependsOn: SnitchOn);
            SnitchSeesImpInMeeting = new CustomToggleOption(num++, MultiMenu.crewmate, "Snitch Sees Impostors In Meetings", true, SnitchOn);
            SnitchSeesTraitor = new CustomToggleOption(num++, MultiMenu.crewmate, "Snitch Sees Traitor", true, SnitchOn);

            Spy =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#CCA3CCFF>Spy</color>", SpyOn);
            WhoSeesDead = new CustomStringOption(num++, MultiMenu.crewmate, "Who Sees Dead Bodies On Admin",
                new[] { "Nobody", "Spy", "Everyone But Spy", "Everyone" }, SpyOn);

            Tracker =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#009900FF>Tracker</color>", TrackerOn);
            UpdateInterval =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Arrow Update Interval", 5f, 0.5f, 15f, 0.5f, CooldownFormat, TrackerOn);
            TrackCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Track Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, TrackerOn);
            ResetOnNewRound = new CustomToggleOption(num++, MultiMenu.crewmate, "Tracker Arrows Reset After Each Round", false, TrackerOn);
            MaxTracks = new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Tracks Per Round", 5, 1, 15, 1, dependsOn: TrackerOn);

            Trapper =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#A7D1B3FF>Trapper</color>", TrapperOn);
            MinAmountOfTimeInTrap =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Min Amount Of Time In Trap To Register", 1f, 0f, 15f, 0.5f, CooldownFormat, TrapperOn);
            TrapCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Trap Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat, TrapperOn);
            TrapsRemoveOnNewRound =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Traps Removed After Each Round", true, TrapperOn);
            MaxTraps =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Traps Per Game", 5, 1, 15, 1, dependsOn: TrapperOn);
            TrapSize =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Trap Size", 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat, TrapperOn);
            MinAmountOfPlayersInTrap =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Minimum Number Of Roles Required To Trigger Trap", 3, 1, 5, 1, dependsOn: TrapperOn);

            Hunter =
               new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#29AB87FF>Hunter</color>", HunterOn);
            HunterKillCd =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Hunter Kill Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat, HunterOn);
            HunterStalkCd =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Hunter Stalk Cooldown", 10f, 0f, 40f, 2.5f, CooldownFormat, HunterOn);
            HunterStalkDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Hunter Stalk Duration", 25f, 5f, 40f, 1f, CooldownFormat, HunterOn);
            HunterStalkUses =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Stalk Uses", 5, 1, 15, 1, dependsOn: HunterOn);
            HunterBodyReport =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Hunter Can Report Who They've Killed", dependsOn: HunterOn);

            Hunter =
               new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#29AB87FF>Hunter</color>");
            HunterKillCd =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Hunter Kill Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            HunterStalkCd =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Hunter Stalk Cooldown", 10f, 0f, 40f, 2.5f, CooldownFormat);
            HunterStalkDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Hunter Stalk Duration", 25f, 5f, 40f, 1f, CooldownFormat);
            HunterStalkUses =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Stalk Uses", 5, 1, 15, 1);
            HunterBodyReport =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Hunter Can Report Who They've Killed");

            Sheriff =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Sheriff</color>", SheriffOn);
            SheriffKillOther =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Miskill Kills Crewmate", false, SheriffOn);
            SheriffKillsDoomsayer =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Doomsayer", false, SheriffOn);
            SheriffKillsExecutioner =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Executioner", false, SheriffOn);
            SheriffKillsJester =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Jester", false, SheriffOn);
            SheriffKillsArsonist =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Arsonist", false, SheriffOn);
            SheriffKillsGlitch =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills The Glitch", false, SheriffOn);
            SheriffKillsJuggernaut =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Juggernaut", false, SheriffOn);
            SheriffKillsPlaguebearer =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Plaguebearer", false, SheriffOn);
            SheriffKillsVampire =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Vampire", false, SheriffOn);
            SheriffKillsWerewolf =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Werewolf", false, SheriffOn);
            SheriffKillCd =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Sheriff Kill Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat, SheriffOn);
            SheriffBodyReport = new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Can Report Who They've Killed", dependsOn: SheriffOn);

            VampireHunter =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#B3B3E6FF>Vampire Hunter</color>", VampireHunterOn);
            StakeCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Stake Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat,  VampireHunterOn);
            MaxFailedStakesPerGame = new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Failed Stakes Per Game", 5, 1, 15, 1, dependsOn:  VampireHunterOn);
            CanStakeRoundOne = new CustomToggleOption(num++, MultiMenu.crewmate, "Can Stake Round One", false, dependsOn:  VampireHunterOn);
            SelfKillAfterFinalStake = new CustomToggleOption(num++, MultiMenu.crewmate, "Self Kill On Failure To Kill A Vamp With All Stakes", false,  VampireHunterOn);
            BecomeOnVampDeaths =
                new CustomStringOption(num++, MultiMenu.crewmate, "What Vampire Hunter Becomes On All Vampire Deaths", new[] { "Crewmate", "Sheriff", "Veteran", "Vigilante", "Hunter"},  VampireHunterOn);

            Veteran =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#998040FF>Veteran</color>", VeteranOn);
            KilledOnAlert =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Can Be Killed On Alert", false, VeteranOn);
            AlertCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Alert Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, VeteranOn);
            AlertDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Alert Duration", 10f, 5f, 15f, 1f, CooldownFormat, VeteranOn);
            MaxAlerts = new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Alerts", 5, 1, 15, 1, dependsOn: VeteranOn);

            Vigilante = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFFF99FF>Vigilante</color>", VigilanteOn);
            VigilanteKills = new CustomNumberOption(num++, MultiMenu.crewmate, "Number Of Vigilante Kills", 1, 1, 15, 1, dependsOn: VigilanteOn);
            VigilanteMultiKill = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Kill More Than Once Per Meeting", false, VigilanteOn);
            VigilanteGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Neutral Benign Roles", false, VigilanteOn);
            VigilanteGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Neutral Evil Roles", false, VigilanteOn);
            VigilanteGuessNeutralKilling = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Neutral Killing Roles", false, VigilanteOn);
            VigilanteGuessLovers = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Lovers", false, VigilanteOn);
            VigilanteAfterVoting = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess After Voting", false, VigilanteOn);

            Altruist = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#660000FF>Altruist</color>", AltruistOn);
            ReviveDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Altruist Revive Duration", 10f, 1f, 15f, 1f, CooldownFormat, AltruistOn);
            AltruistTargetBody =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Target's Body Disappears On Beginning Of Revive", false, AltruistOn);

            Medic =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#006600FF>Medic</color>", MedicOn);
            ShowShielded =
                new CustomStringOption(num++, MultiMenu.crewmate, "Show Shielded Player",
                    new[] { "Self", "Medic", "Self+Medic", "Everyone" }, MedicOn);
            WhoGetsNotification =
                new CustomStringOption(num++, MultiMenu.crewmate, "Who Gets Murder Attempt Indicator",
                    new[] { "Medic", "Shielded", "Everyone", "Nobody" }, MedicOn);
            ShieldBreaks = new CustomToggleOption(num++, MultiMenu.crewmate, "Shield Breaks On Murder Attempt", false, MedicOn);
            MedicReportSwitch = new CustomToggleOption(num++, MultiMenu.crewmate, "Show Medic Reports", dependsOn: MedicOn);
            MedicReportNameDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Time Where Medic Will Have Name", 0f, 0f, 60f, 2.5f,
                    CooldownFormat, MedicOn);
            MedicReportColorDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Time Where Medic Will Have Color Type", 15f, 0f, 60f, 2.5f,
                    CooldownFormat, MedicOn);

            Engineer =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFA60AFF>Engineer</color>", EngineerOn);
            MaxFixes =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Fixes", 5, 1, 15, 1, dependsOn: EngineerOn);

            Medium =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#A680FFFF>Medium</color>", MediumOn);
            MediateCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Mediate Cooldown", 10f, 1f, 15f, 1f, CooldownFormat, MediumOn);
            ShowMediatePlayer =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Reveal Appearance Of Mediate Target", true, MediumOn);
            ShowMediumToDead =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Reveal The Medium To The Mediate Target", true, MediumOn);
            DeadRevealed =
                new CustomStringOption(num++, MultiMenu.crewmate, "Who Is Revealed With Mediate", new[] { "Oldest Dead", "Newest Dead", "All Dead" }, MediumOn);

            Prosecutor =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#B38000FF>Prosecutor</color>", ProsecutorOn);
            ProsDiesOnIncorrectPros =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Prosecutor Dies When They Exile A Crewmate", false, ProsecutorOn);

            Swapper =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#66E666FF>Swapper</color>", SwapperOn);
            SwapperButton =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Swapper Can Button", true, SwapperOn);

            Transporter =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#00EEFFFF>Transporter</color>", TransporterOn);
            TransportCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Transport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, TransporterOn);
            TransportMaxUses =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Transports", 5, 1, 15, 1, dependsOn: TransporterOn);
            TransporterVitals =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Transporter Can Use Vitals", false, TransporterOn);

            Amnesiac = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#80B2FFFF>Amnesiac</color>", AmnesiacOn);
            RememberArrows =
                new CustomToggleOption(num++, MultiMenu.neutral, "Amnesiac Gets Arrows Pointing To Dead Bodies", false, AmnesiacOn);
            RememberArrowDelay =
                new CustomNumberOption(num++, MultiMenu.neutral, "Time After Death Arrow Appears", 5f, 0f, 15f, 1f, CooldownFormat, AmnesiacOn);

            GuardianAngel =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3FFFFFF>Guardian Angel</color>", GuardianAngelOn);
            ProtectCd =
                new CustomNumberOption(num++, MultiMenu.neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GuardianAngelOn);
            ProtectDuration =
                new CustomNumberOption(num++, MultiMenu.neutral, "Protect Duration", 10f, 5f, 15f, 1f, CooldownFormat, GuardianAngelOn);
            ProtectKCReset =
                new CustomNumberOption(num++, MultiMenu.neutral, "Kill Cooldown Reset When Protected", 2.5f, 0f, 15f, 0.5f, CooldownFormat, GuardianAngelOn);
            MaxProtects =
                new CustomNumberOption(num++, MultiMenu.neutral, "Maximum Number Of Protects", 5, 1, 15, 1, dependsOn: GuardianAngelOn);
            ShowProtect =
                new CustomStringOption(num++, MultiMenu.neutral, "Show Protected Player",
                    new[] { "Self", "Guardian Angel", "Self+GA", "Everyone" }, GuardianAngelOn);
            GaOnTargetDeath = new CustomStringOption(num++, MultiMenu.neutral, "GA Becomes On Target Dead",
                new[] { "Crew", "Amnesiac", "Survivor", "Jester" }, GuardianAngelOn);
            GATargetKnows =
                new CustomToggleOption(num++, MultiMenu.neutral, "Target Knows GA Exists", false, GuardianAngelOn);
            GAKnowsTargetRole =
                new CustomToggleOption(num++, MultiMenu.neutral, "GA Knows Targets Role", false, GuardianAngelOn);
            EvilTargetPercent = new CustomNumberOption(num++, MultiMenu.neutral, "Odds Of Target Being Evil", 20f, 0f, 100f, 10f,
                PercentFormat, GuardianAngelOn);

            Survivor =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#FFE64DFF>Survivor</color>", SurvivorOn);
            VestCd =
                new CustomNumberOption(num++, MultiMenu.neutral, "Vest Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SurvivorOn);
            VestDuration =
                new CustomNumberOption(num++, MultiMenu.neutral, "Vest Duration", 10f, 5f, 15f, 1f, CooldownFormat, SurvivorOn);
            VestKCReset =
                new CustomNumberOption(num++, MultiMenu.neutral, "Kill Cooldown Reset On Attack", 2.5f, 0f, 15f, 0.5f, CooldownFormat, SurvivorOn);
            MaxVests =
                new CustomNumberOption(num++, MultiMenu.neutral, "Maximum Number Of Vests", 5, 1, 15, 1, dependsOn: SurvivorOn);

            Doomsayer = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#00FF80FF>Doomsayer</color>", DoomsayerOn);
            ObserveCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Observe Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, DoomsayerOn);
            DoomsayerGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Neutral Benign Roles", false, DoomsayerOn);
            DoomsayerGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Neutral Evil Roles", false, DoomsayerOn);
            DoomsayerGuessNeutralKilling = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Neutral Killing Roles", false, DoomsayerOn);
            DoomsayerGuessImpostors = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Impostor Roles", false, DoomsayerOn);
            DoomsayerAfterVoting = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess After Voting", false, DoomsayerOn);
            DoomsayerGuessesToWin = new CustomNumberOption(num++, MultiMenu.neutral, "Number Of Doomsayer Kills To Win", 3, 1, 5, 1, dependsOn: DoomsayerOn);
            DoomsayerCantObserve = new CustomToggleOption(num++, MultiMenu.neutral, "(Experienced) Doomsayer can't observe", false);

            Executioner =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#8C4005FF>Executioner</color>", ExecutionerOn);
            OnTargetDead = new CustomStringOption(num++, MultiMenu.neutral, "Executioner Becomes On Target Dead",
                new[] { "Crew", "Amnesiac", "Survivor", "Jester" }, ExecutionerOn);
            ExecutionerButton =
                new CustomToggleOption(num++, MultiMenu.neutral, "Executioner Can Button", true,ExecutionerOn);
            ExecutionerTorment =
                new CustomToggleOption(num++, MultiMenu.neutral, "Executioner Torments Player On Victory", true, ExecutionerOn);

            Jester =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#FFBFCCFF>Jester</color>", JesterOn);
            JesterButton =
                new CustomToggleOption(num++, MultiMenu.neutral, "Jester Can Button", true, JesterOn);
            JesterVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Jester Can Hide In Vents", false, JesterOn);
            JesterImpVision =
                new CustomToggleOption(num++, MultiMenu.neutral, "Jester Has Impostor Vision", false, JesterOn);
            JesterHaunt =
                new CustomToggleOption(num++, MultiMenu.neutral, "Jester Haunts Player On Victory", true, JesterOn);

            Phantom =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color>", PhantomOn);
            PhantomTasksRemaining =
                 new CustomNumberOption(num++, MultiMenu.neutral, "Tasks Remaining When Phantom Can Be Clicked", 5, 1, 15, 1, dependsOn: PhantomOn);
            PhantomSpook =
                new CustomToggleOption(num++, MultiMenu.neutral, "Phantom Spooks Player On Victory", true, PhantomOn);

            Arsonist = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#FF4D00FF>Arsonist</color>", ArsonistOn);
            DouseCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, ArsonistOn);
            MaxDoused =
                new CustomNumberOption(num++, MultiMenu.neutral, "Maximum Alive Players Doused", 5, 1, 15, 1, dependsOn: ArsonistOn);
            ArsoImpVision =
                new CustomToggleOption(num++, MultiMenu.neutral, "Arsonist Has Impostor Vision", false, ArsonistOn);
            IgniteCdRemoved =
                new CustomToggleOption(num++, MultiMenu.neutral, "Ignite Cooldown Removed When Arsonist Is Last Killer", false, ArsonistOn);

            Juggernaut =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#8C004DFF>Juggernaut</color>", HiddenRoles);
            JuggKillCooldown = new CustomNumberOption(num++, MultiMenu.neutral, "Juggernaut Initial Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, HiddenRoles);
            ReducedKCdPerKill = new CustomNumberOption(num++, MultiMenu.neutral, "Reduced Kill Cooldown Per Kill", 5f, 2.5f, 10f, 2.5f, CooldownFormat, HiddenRoles);
            JuggVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Juggernaut Can Vent", false, HiddenRoles);

            Plaguebearer = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#E6FFB3FF>Plaguebearer</color>", PlaguebearerOn);
            InfectCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Infect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, PlaguebearerOn);
            PestKillCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Pestilence Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, PlaguebearerOn);
            PestVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Pestilence Can Vent", false, PlaguebearerOn);

            TheGlitch =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#00FF00FF>The Glitch</color>", GlitchOn);
            MimicCooldownOption = new CustomNumberOption(num++, MultiMenu.neutral, "Mimic Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GlitchOn);
            MimicDurationOption = new CustomNumberOption(num++, MultiMenu.neutral, "Mimic Duration", 10f, 1f, 15f, 1f, CooldownFormat, GlitchOn);
            HackCooldownOption = new CustomNumberOption(num++, MultiMenu.neutral, "Hack Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GlitchOn);
            HackDurationOption = new CustomNumberOption(num++, MultiMenu.neutral, "Hack Duration", 10f, 1f, 15f, 1f, CooldownFormat, GlitchOn);
            GlitchKillCooldownOption =
                new CustomNumberOption(num++, MultiMenu.neutral, "Glitch Kill Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat, GlitchOn);
            GlitchHackDistanceOption =
                new CustomStringOption(num++, MultiMenu.neutral, "Glitch Hack Distance", new[] { "Short", "Normal", "Long" }, GlitchOn);
            GlitchVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Glitch Can Vent", false, GlitchOn);

            Vampire = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#262626FF>Vampire</color>", VampireOn);
            BiteCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Vampire Bite Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, VampireOn);
            VampImpVision =
                new CustomToggleOption(num++, MultiMenu.neutral, "Vampires Have Impostor Vision", false, VampireOn);
            VampVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Vampires Can Vent", false, VampireOn);
            NewVampCanAssassin =
                new CustomToggleOption(num++, MultiMenu.neutral, "New Vampire Can Assassinate", false, VampireOn);
            MaxVampiresPerGame =
                new CustomNumberOption(num++, MultiMenu.neutral, "Maximum Vampires Per Game", 2, 2, 5, 1, dependsOn: VampireOn);
            CanBiteNeutralBenign =
                new CustomToggleOption(num++, MultiMenu.neutral, "Can Convert Neutral Benign Roles", false, VampireOn);
            CanBiteNeutralEvil =
                new CustomToggleOption(num++, MultiMenu.neutral, "Can Convert Neutral Evil Roles", false, VampireOn);

            Werewolf = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#A86629FF>Werewolf</color>",  WerewolfOn);
            RampageCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Rampage Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, WerewolfOn);
            RampageDuration =
                new CustomNumberOption(num++, MultiMenu.neutral, "Rampage Duration", 25f, 10f, 60f, 2.5f, CooldownFormat, WerewolfOn);
            RampageKillCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Rampage Kill Cooldown", 10f, 0.5f, 15f, 0.5f, CooldownFormat, WerewolfOn);
            WerewolfVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Werewolf Can Vent When Rampaged", false, WerewolfOn);

            Escapist =
                new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Escapist</color>", EscapistOn);
            EscapeCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Recall Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, EscapistOn);
            EscapistVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Escapist Can Vent", false, EscapistOn);

            Grenadier =
                new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Grenadier</color>", GrenadierOn);
            GrenadeCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Flash Grenade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, GrenadierOn);
            GrenadeDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Flash Grenade Duration", 10f, 5f, 15f, 1f, CooldownFormat, GrenadierOn);
            FlashRadius =
                new CustomNumberOption(num++, MultiMenu.imposter, "Flash Radius", 1f, 0.25f, 5f, 0.25f, MultiplierFormat, GrenadierOn);
            GrenadierIndicators =
                new CustomToggleOption(num++, MultiMenu.imposter, "Indicate Flashed Crewmates", false, GrenadierOn);
            GrenadierVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Grenadier Can Vent", false, GrenadierOn);

            Morphling =
                new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Morphling</color>", MorphlingOn);
            MorphlingCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Morphling Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, MorphlingOn);
            MorphlingDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Morphling Duration", 10f, 5f, 15f, 1f, CooldownFormat, MorphlingOn);
            MorphlingVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Morphling Can Vent", false, MorphlingOn);

            Swooper = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Swooper</color>", SwooperOn);
            SwoopCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Swoop Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, SwooperOn);
            SwoopDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Swoop Duration", 10f, 5f, 15f, 1f, CooldownFormat, SwooperOn);
            SwooperVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Swooper Can Vent", false, SwooperOn);

            Venerer = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Venerer</color>", VenererOn);
            AbilityCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Ability Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, VenererOn);
            AbilityDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Ability Duration", 10f, 5f, 15f, 1f, CooldownFormat, VenererOn);
            SprintSpeed = new CustomNumberOption(num++, MultiMenu.imposter, "Sprint Speed", 1.25f, 1.05f, 2.5f, 0.05f, MultiplierFormat, VenererOn);
            FreezeSpeed = new CustomNumberOption(num++, MultiMenu.imposter, "Freeze Speed", 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat, VenererOn);

            Bomber =
                new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Bomber</color>", BomberOn);
            DetonateDelay =
                new CustomNumberOption(num++, MultiMenu.imposter, "Detonate Delay", 5f, 1f, 15f, 1f, CooldownFormat, BomberOn);
            MaxKillsInDetonation =
                new CustomNumberOption(num++, MultiMenu.imposter, "Max Kills In Detonation", 5, 1, 15, 1, dependsOn: BomberOn);
            DetonateRadius =
                new CustomNumberOption(num++, MultiMenu.imposter, "Detonate Radius", 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat, BomberOn);
            BomberVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Bomber Can Vent", false, BomberOn);

            Traitor = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Traitor</color>", TraitorOn);
            LatestSpawn = new CustomNumberOption(num++, MultiMenu.imposter, "Minimum People Alive When Traitor Can Spawn", 5, 3, 15, 1, dependsOn: TraitorOn);
            NeutralKillingStopsTraitor =
                new CustomToggleOption(num++, MultiMenu.imposter, "Traitor Won't Spawn If Any Neutral Killing Is Alive", false, TraitorOn);

            Warlock = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Warlock</color>", WarlockOn);
            ChargeUpDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Time It Takes To Fully Charge", 25f, 10f, 60f, 2.5f, CooldownFormat, WarlockOn);
            ChargeUseDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Time It Takes To Use Full Charge", 1f, 0.05f, 5f, 0.05f, CooldownFormat, WarlockOn);

            Blackmailer = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Blackmailer</color>", BlackmailerOn);
            BlackmailCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Initial Blackmail Cooldown", 10f, 1f, 15f, 1f, CooldownFormat, BlackmailerOn);
            BlackmailInvisible =
                new CustomToggleOption(num++, MultiMenu.imposter, "Only Target Sees Blackmail", false, dependsOn: BlackmailerOn);

            Miner = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Miner</color>", MinerOn);
            MineCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Mine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, MinerOn);

            Undertaker = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Undertaker</color>", UndertakerOn);
            DragCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Drag Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat, UndertakerOn);
            UndertakerDragSpeed =
                new CustomNumberOption(num++, MultiMenu.imposter, "Undertaker Drag Speed", 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat, UndertakerOn);
            UndertakerVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Undertaker Can Vent", false, UndertakerOn);
            UndertakerVentWithBody =
                new CustomToggleOption(num++, MultiMenu.imposter, "Undertaker Can Vent While Dragging", false, UndertakerOn);

            Bait = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#00B3B3FF>Bait</color>", BaitOn);
            BaitMinDelay = new CustomNumberOption(num++, MultiMenu.modifiers, "Minimum Delay for the Bait Report", 0f, 0f, 15f, 0.5f, CooldownFormat, BaitOn);
            BaitMaxDelay = new CustomNumberOption(num++, MultiMenu.modifiers, "Maximum Delay for the Bait Report", 1f, 0f, 15f, 0.5f, CooldownFormat, BaitOn);

            Diseased = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#808080FF>Diseased</color>", DiseasedOn);
            DiseasedKillMultiplier = new CustomNumberOption(num++, MultiMenu.modifiers, "Diseased Kill Multiplier", 3f, 1.5f, 5f, 0.5f, MultiplierFormat, DiseasedOn);

            Frosty = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#99FFFFFF>Frosty</color>", FrostyOn);
            ChillDuration = new CustomNumberOption(num++, MultiMenu.modifiers, "Chill Duration", 10f, 1f, 15f, 1f, CooldownFormat, FrostyOn);
            ChillStartSpeed = new CustomNumberOption(num++, MultiMenu.modifiers, "Chill Start Speed", 0.75f, 0.25f, 0.95f, 0.05f, MultiplierFormat, FrostyOn);

            Flash = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FF8080FF>Flash</color>", FlashOn);
            FlashSpeed = new CustomNumberOption(num++, MultiMenu.modifiers, "Flash Speed", 1.25f, 1.05f, 2.5f, 0.05f, MultiplierFormat, FlashOn);

            Giant = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FFB34DFF>Giant</color>", GiantOn);
            GiantSlow = new CustomNumberOption(num++, MultiMenu.modifiers, "Giant Speed", 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat, GiantOn);

            Lovers =
                new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FF66CCFF>Lovers</color>", LoversOn);
            BothLoversDie = new CustomToggleOption(num++, MultiMenu.modifiers, "Both Lovers Die", dependsOn: LoversOn);
            LovingImpPercent = new CustomNumberOption(num++, MultiMenu.modifiers, "Loving Impostor Probability", 20f, 0f, 100f, 10f,
                PercentFormat, LoversOn);
            NeutralLovers = new CustomToggleOption(num++, MultiMenu.modifiers, "Neutral Roles Can Be Lovers", dependsOn: LoversOn);

            Mini = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#b0a1ff>Mini</color>", MiniOn);
            MiniSpeed = new CustomNumberOption(num++, MultiMenu.modifiers, "Mini Speed", 1.25f, 1.05f, 2.5f, 0.05f, MultiplierFormat, MiniOn);

            Underdog = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FF0000FF>Underdog</color>", UnderdogOn);
            UnderdogKillBonus = new CustomNumberOption(num++, MultiMenu.modifiers, "Kill Cooldown Bonus", 5f, 2.5f, 10f, 2.5f, CooldownFormat, UnderdogOn);
            UnderdogIncreasedKC = new CustomToggleOption(num++, MultiMenu.modifiers, "Increased Kill Cooldown When 2+ Imps", true, UnderdogOn);
        }
    }
}
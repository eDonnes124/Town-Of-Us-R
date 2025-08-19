using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using System;

namespace TownOfUs.Roles
{
    public class Doomsayer : Role, IGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; } = new();
        public Dictionary<byte, TMP_Text> RoleGuess { get; set; } = new();

        private Dictionary<string, Color> ColorMapping = new();

        public Dictionary<string, Color> SortedColorMapping;

        public Dictionary<byte, string> Guesses = new();
        public DateTime LastObserved;
        public PlayerControl ClosestPlayer;
        public PlayerControl LastObservedPlayer;

        public Doomsayer(PlayerControl player) : base(player)
        {
            Name = "Doomsayer";
            ImpostorText = () => "Guess People's Roles To Win!";
            TaskText = () => "Win by guessing players' roles\nFake Tasks:";
            Color = Patches.Colors.Doomsayer;
            RoleType = RoleEnum.Doomsayer;
            LastObserved = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;

            ColorMapping.Add("Crewmate", Colors.Crewmate);
            if (CustomGameOptions.PoliticianOn > 0) ColorMapping.Add("Politician", Colors.Crewmate);
            if (CustomGameOptions.SheriffOn > 0) ColorMapping.Add("Sheriff", Colors.Crewmate);
            if (CustomGameOptions.EngineerOn > 0) ColorMapping.Add("Engineer", Colors.Crewmate);
            if (CustomGameOptions.SwapperOn > 0) ColorMapping.Add("Swapper", Colors.Crewmate);
            if (CustomGameOptions.InvestigatorOn > 0) ColorMapping.Add("Investigator", Colors.Crewmate);
            if (CustomGameOptions.MedicOn > 0) ColorMapping.Add("Medic", Colors.Crewmate);
            if (CustomGameOptions.SeerOn > 0) ColorMapping.Add("Seer", Colors.Crewmate);
            if (CustomGameOptions.SpyOn > 0) ColorMapping.Add("Spy", Colors.Crewmate);
            if (CustomGameOptions.SnitchOn > 0) ColorMapping.Add("Snitch", Colors.Crewmate);
            if (CustomGameOptions.AltruistOn > 0) ColorMapping.Add("Altruist", Colors.Crewmate);
            if (CustomGameOptions.VigilanteOn > 0) ColorMapping.Add("Vigilante", Colors.Crewmate);
            if (CustomGameOptions.VeteranOn > 0) ColorMapping.Add("Veteran", Colors.Crewmate);
            if (CustomGameOptions.HunterOn > 0) ColorMapping.Add("Hunter", Colors.Crewmate);
            if (CustomGameOptions.TrackerOn > 0) ColorMapping.Add("Tracker", Colors.Crewmate);
            if (CustomGameOptions.TrapperOn > 0) ColorMapping.Add("Trapper", Colors.Crewmate);
            if (CustomGameOptions.TransporterOn > 0) ColorMapping.Add("Transporter", Colors.Crewmate);
            if (CustomGameOptions.MediumOn > 0) ColorMapping.Add("Medium", Colors.Crewmate);
            if (CustomGameOptions.MysticOn > 0) ColorMapping.Add("Mystic", Colors.Crewmate);
            if (CustomGameOptions.DetectiveOn > 0) ColorMapping.Add("Detective", Colors.Crewmate);
            if (CustomGameOptions.ImitatorOn > 0) ColorMapping.Add("Imitator", Colors.Crewmate);
            if (CustomGameOptions.ProsecutorOn > 0) ColorMapping.Add("Prosecutor", Colors.Crewmate);
            if (CustomGameOptions.OracleOn > 0) ColorMapping.Add("Oracle", Colors.Crewmate);
            if (CustomGameOptions.AurialOn > 0) ColorMapping.Add("Aurial", Colors.Crewmate);
            if (CustomGameOptions.WardenOn > 0) ColorMapping.Add("Warden", Colors.Crewmate);
            if (CustomGameOptions.JailorOn > 0) ColorMapping.Add("Jailor", Colors.Crewmate);
            if (CustomGameOptions.LookoutOn > 0) ColorMapping.Add("Lookout", Colors.Crewmate);
            if (CustomGameOptions.DeputyOn > 0) ColorMapping.Add("Deputy", Colors.Crewmate);
            if (CustomGameOptions.PlumberOn > 0) ColorMapping.Add("Plumber", Colors.Crewmate);
            if (CustomGameOptions.ClericOn > 0) ColorMapping.Add("Cleric", Colors.Crewmate);

            ColorMapping.Add("Impostor", Colors.Impostor);
            if (CustomGameOptions.JanitorOn > 0) ColorMapping.Add("Janitor", Colors.Impostor);
            if (CustomGameOptions.MorphlingOn > 0) ColorMapping.Add("Morphling", Colors.Impostor);
            if (CustomGameOptions.MinerOn > 0) ColorMapping.Add("Miner", Colors.Impostor);
            if (CustomGameOptions.SwooperOn > 0) ColorMapping.Add("Swooper", Colors.Impostor);
            if (CustomGameOptions.UndertakerOn > 0) ColorMapping.Add("Undertaker", Colors.Impostor);
            if (CustomGameOptions.EscapistOn > 0) ColorMapping.Add("Escapist", Colors.Impostor);
            if (CustomGameOptions.GrenadierOn > 0) ColorMapping.Add("Grenadier", Colors.Impostor);
            if (CustomGameOptions.TraitorOn > 0) ColorMapping.Add("Traitor", Colors.Impostor);
            if (CustomGameOptions.BlackmailerOn > 0) ColorMapping.Add("Blackmailer", Colors.Impostor);
            if (CustomGameOptions.BomberOn > 0) ColorMapping.Add("Bomber", Colors.Impostor);
            if (CustomGameOptions.WarlockOn > 0) ColorMapping.Add("Warlock", Colors.Impostor);
            if (CustomGameOptions.VenererOn > 0) ColorMapping.Add("Venerer", Colors.Impostor);
            if (CustomGameOptions.HypnotistOn > 0) ColorMapping.Add("Hypnotist", Colors.Impostor);
            if (CustomGameOptions.ScavengerOn > 0) ColorMapping.Add("Scavenger", Colors.Impostor);
            if (CustomGameOptions.EclipsalOn > 0) ColorMapping.Add("Eclipsal", Colors.Impostor);

            if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac)) ColorMapping.Add("Amnesiac", Colors.Amnesiac);
            if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
            if (CustomGameOptions.MercenaryOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Mercenary) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Mercenary)) ColorMapping.Add("Mercenary", Colors.Mercenary);
            if (CustomGameOptions.SurvivorOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Survivor) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Survivor)) ColorMapping.Add("Survivor", Colors.Survivor);
            if (!CustomGameOptions.UniqueRoles) ColorMapping.Add("Doomsayer", Colors.Doomsayer);
            if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", Colors.Executioner);
            if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester)) ColorMapping.Add("Jester", Colors.Jester);
            if (CustomGameOptions.ArsonistOn > 0) ColorMapping.Add("Arsonist", Colors.Arsonist);
            if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("Glitch", Colors.Glitch);
            if (CustomGameOptions.PlaguebearerOn > 0) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
            if (CustomGameOptions.VampireOn > 0) ColorMapping.Add("Vampire", Colors.Vampire);
            if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", Colors.Werewolf);
            if (CustomGameOptions.JuggernautOn > 0) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
            if (CustomGameOptions.SoulCollectorOn > 0) ColorMapping.Add("Soul Collector", Colors.SoulCollector);

            SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public float ObserveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastObserved;
            var num = CustomGameOptions.ObserveCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public int NumberOfGuesses = 0;
        public int IncorrectGuesses = 0;
        public bool WonByGuessing = false;

        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var doomTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            doomTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = doomTeam;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!CustomGameOptions.DoomsayerWinEndsGame) return true;
            if (!WonByGuessing) return true;
            Utils.EndGame();
            return false;
        }
    }
}

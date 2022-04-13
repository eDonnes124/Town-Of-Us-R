using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Vigilante : Role
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons = new Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)>();

        private Dictionary<string, Color> ColorMapping = new Dictionary<string, Color>();

        public Dictionary<string, Color> SortedColorMapping;

        public Dictionary<byte, string> Guesses = new Dictionary<byte, string>();

        public Vigilante(PlayerControl player) : base(player)
        {
            Name = "Vigilante";
            ImpostorText = () => "Kill impostors during meetings if you can guess their roles";
            TaskText = () => "Guess the roles of impostors mid-meeting to kill them!";
            Color = Patches.Colors.Vigilante;
            RoleType = RoleEnum.Vigilante;
            AddToRoleHistory(RoleType);

            RemainingKills = CustomGameOptions.VigilanteKills;

            // Adds all the roles that have a non-zero chance of being in the game.
            ColorMapping.Add("Impostor", Colors.Impostor);
            if (CustomGameOptions.JanitorOn > 0) ColorMapping.Add("Janitor", Colors.Impostor);
            if (CustomGameOptions.MorphlingOn > 0) ColorMapping.Add("Morphling", Colors.Impostor);
            if (CustomGameOptions.CamouflagerOn > 0) ColorMapping.Add("Camouflager", Colors.Impostor);
            if (CustomGameOptions.MinerOn > 0) ColorMapping.Add("Miner", Colors.Impostor);
            if (CustomGameOptions.SwooperOn > 0) ColorMapping.Add("Swooper", Colors.Impostor);
            if (CustomGameOptions.UndertakerOn > 0) ColorMapping.Add("Undertaker", Colors.Impostor);
            //if (CustomGameOptions.AssassinOn > 0) ColorMapping.Add("Assassin", Colors.Impostor);
            if (CustomGameOptions.UnderdogOn > 0) ColorMapping.Add("Underdog", Colors.Impostor);
            if (CustomGameOptions.GrenadierOn > 0) ColorMapping.Add("Grenadier", Colors.Impostor);
            if (CustomGameOptions.PoisonerOn > 0) ColorMapping.Add("Poisoner", Colors.Impostor);
            if (CustomGameOptions.TraitorOn > 0) ColorMapping.Add("Traitor", Colors.Impostor);
            // Add Neutral roles if enabled
            if (CustomGameOptions.VigilanteGuessNeutrals)
            {
                if (CustomGameOptions.JesterOn > 0 || CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Jester", Colors.Jester);
                if (CustomGameOptions.AmnesiacOn > 0 || CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Amnesiac", Colors.Amnesiac);
                if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", Colors.Executioner);
                if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("The Glitch", Colors.Glitch);
                if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                if (CustomGameOptions.ArsonistOn > 0) ColorMapping.Add("Arsonist", Colors.Arsonist);
            }

            // Sorts the list alphabetically. 
            SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public bool GuessedThisMeeting { get; set; } = false;

        public int RemainingKills { get; set; }

        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();
    }
}

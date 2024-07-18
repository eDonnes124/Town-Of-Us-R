using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;

namespace TownOfUs.Roles
{
    public class Vigilante : Role, IGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; } = new();

        private Dictionary<string, Color> ColorMapping = new();

        public Dictionary<string, Color> SortedColorMapping;

        public Dictionary<byte, string> Guesses = new();

        public Vigilante(PlayerControl player) : base(player)
        {
            Name = "Vigilante";
            ImpostorText = () => "Kill Impostors If You Can Guess Their Roles";
            TaskText = () => "Guess the roles of impostors mid-meeting to kill them!";
            Color = Patches.Colors.Vigilante;
            RoleType = RoleEnum.Vigilante;
            AddToRoleHistory(RoleType);

            RemainingKills = CustomGameOptions.VigilanteKills;

            if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny)
            {
                ColorMapping.Add("Impostor", Colors.Impostor);
                if (CustomGameOptions.JanitorOn > 0) ColorMapping.Add("Janitor", Colors.Janitor);
                if (CustomGameOptions.MorphlingOn > 0) ColorMapping.Add("Morphling", Colors.Morphling);
                if (CustomGameOptions.MinerOn > 0) ColorMapping.Add("Miner", Colors.Miner);
                if (CustomGameOptions.SwooperOn > 0) ColorMapping.Add("Swooper", Colors.Swooper);
                if (CustomGameOptions.UndertakerOn > 0) ColorMapping.Add("Undertaker", Colors.Undertaker);
                if (CustomGameOptions.EscapistOn > 0) ColorMapping.Add("Escapist", Colors.Escapist);
                if (CustomGameOptions.GrenadierOn > 0) ColorMapping.Add("Grenadier", Colors.Grenadier);
                if (CustomGameOptions.TraitorOn > 0) ColorMapping.Add("Traitor", Colors.Traitor);
                if (CustomGameOptions.BlackmailerOn > 0) ColorMapping.Add("Blackmailer", Colors.Blackmailer);
                if (CustomGameOptions.BomberOn > 0) ColorMapping.Add("Bomber", Colors.Bomber);
                if (CustomGameOptions.WarlockOn > 0) ColorMapping.Add("Warlock", Colors.Warlock);
                if (CustomGameOptions.VenererOn > 0) ColorMapping.Add("Venerer", Colors.Venerer);

                if (CustomGameOptions.VigilanteGuessNeutralBenign)
                {
                    if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac)) ColorMapping.Add("Amnesiac", Colors.Amnesiac);
                    if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                    if (CustomGameOptions.SurvivorOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Survivor) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Survivor)) ColorMapping.Add("Survivor", Colors.Survivor);
                }
                if (CustomGameOptions.VigilanteGuessNeutralEvil)
                {
                    if (CustomGameOptions.DoomsayerOn > 0) ColorMapping.Add("Doomsayer", Colors.Doomsayer);
                    if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", Colors.Executioner);
                    if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester)) ColorMapping.Add("Jester", Colors.Jester);
                }
                if (CustomGameOptions.VigilanteGuessNeutralKilling)
                {
                    if (CustomGameOptions.ArsonistOn > 0) ColorMapping.Add("Arsonist", Colors.Arsonist);
                    if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("The Glitch", Colors.Glitch);
                    if (CustomGameOptions.PlaguebearerOn > 0) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                    if (CustomGameOptions.GameMode == GameMode.Classic && CustomGameOptions.VampireOn > 0) ColorMapping.Add("Vampire", Colors.Vampire);
                    if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", Colors.Werewolf);
                    if (CustomGameOptions.HiddenRoles) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                }
                if (CustomGameOptions.VigilanteGuessLovers && CustomGameOptions.LoversOn > 0) ColorMapping.Add("Lover", Colors.Lovers);
            }
            else if (CustomGameOptions.GameMode == GameMode.KillingOnly)
            {
                ColorMapping.Add("Morphling", Colors.Morphling);
                ColorMapping.Add("Miner", Colors.Miner);
                ColorMapping.Add("Swooper", Colors.Swooper);
                ColorMapping.Add("Undertaker", Colors.Undertaker);
                ColorMapping.Add("Grenadier", Colors.Grenadier);
                ColorMapping.Add("Traitor", Colors.Traitor);
                ColorMapping.Add("Escapist", Colors.Escapist);

                if (CustomGameOptions.VigilanteGuessNeutralKilling)
                {
                    if (CustomGameOptions.AddArsonist) ColorMapping.Add("Arsonist", Colors.Arsonist);
                    if (CustomGameOptions.AddPlaguebearer) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                    ColorMapping.Add("The Glitch", Colors.Glitch);
                    ColorMapping.Add("Werewolf", Colors.Werewolf);
                    if (CustomGameOptions.HiddenRoles) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                }
            }
            else
            {
                ColorMapping.Add("Necromancer", Colors.Impostor);
                ColorMapping.Add("Whisperer", Colors.Impostor);
                if (CustomGameOptions.MaxChameleons > 0) ColorMapping.Add("Swooper", Colors.Swooper);
                if (CustomGameOptions.MaxEngineers > 0) ColorMapping.Add("Demolitionist", Colors.Impostor);
                if (CustomGameOptions.MaxInvestigators > 0) ColorMapping.Add("Consigliere", Colors.Impostor);
                if (CustomGameOptions.MaxMystics > 0) ColorMapping.Add("Clairvoyant", Colors.Impostor);
                if (CustomGameOptions.MaxSnitches > 0) ColorMapping.Add("Informant", Colors.Impostor);
                if (CustomGameOptions.MaxSpies > 0) ColorMapping.Add("Rogue Agent", Colors.Impostor);
                if (CustomGameOptions.MaxTransporters > 0) ColorMapping.Add("Escapist", Colors.Escapist);
                if (CustomGameOptions.MaxVigilantes > 1) ColorMapping.Add("Assassin", Colors.Impostor);
                ColorMapping.Add("Impostor", Colors.Impostor);
            }

            SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public bool GuessedThisMeeting { get; set; } = false;

        public int RemainingKills { get; set; }

        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();
    }
}

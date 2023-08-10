using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using System;
using TownOfUs.Extensions;
using TownOfUs.Roles.Modifiers;
using TownOfUs.CrewmateRoles.VigilanteMod;

namespace TownOfUs.Roles
{
    public class Vigilante : Role, IRoleGuesser<Vigilante>
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; } = new();

        public Dictionary<string, Color> ColorMapping { get; set; } = new();

        public Dictionary<string, Color> SortedColorMapping { get; set; }

        public Dictionary<byte, string> Guesses { get; set; } = new();

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
                ColorMapping.Add("Morphling", Colors.Impostor);
                ColorMapping.Add("Miner", Colors.Impostor);
                ColorMapping.Add("Swooper", Colors.Impostor);
                ColorMapping.Add("Undertaker", Colors.Impostor);
                ColorMapping.Add("Grenadier", Colors.Impostor);
                ColorMapping.Add("Traitor", Colors.Impostor);
                ColorMapping.Add("Escapist", Colors.Impostor);

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
                if (CustomGameOptions.MaxChameleons > 0) ColorMapping.Add("Swooper", Colors.Impostor);
                if (CustomGameOptions.MaxEngineers > 0) ColorMapping.Add("Demolitionist", Colors.Impostor);
                if (CustomGameOptions.MaxInvestigators > 0) ColorMapping.Add("Consigliere", Colors.Impostor);
                if (CustomGameOptions.MaxMystics > 0) ColorMapping.Add("Clairvoyant", Colors.Impostor);
                if (CustomGameOptions.MaxSnitches > 0) ColorMapping.Add("Informant", Colors.Impostor);
                if (CustomGameOptions.MaxSpies > 0) ColorMapping.Add("Rogue Agent", Colors.Impostor);
                if (CustomGameOptions.MaxTransporters > 0) ColorMapping.Add("Escapist", Colors.Impostor);
                if (CustomGameOptions.MaxVigilantes > 1) ColorMapping.Add("Assassin", Colors.Impostor);
                ColorMapping.Add("Impostor", Colors.Impostor);
            }

            SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public bool GuessedThisMeeting { get; set; } = false;

        public int RemainingKills { get; set; }

        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();

        private void HideSingle(IGuesser role, byte targetId, bool killedSelf)
        {
            if (killedSelf || RemainingKills == 0 || (!CustomGameOptions.VigilanteMultiKill))
            {
                role.HideAllButtons(role);
                GuessedThisMeeting = true;
            } 
            else role.HideTarget(role, targetId);
        }

        public Action Guess(IGuesser role, PlayerVoteArea voteArea)
        {
            void Listener()
            {
                if (
                    MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion ||
                    IsUnGuessable(voteArea) || PlayerControl.LocalPlayer.Data.IsDead
                ) return;
                var targetId = voteArea.TargetPlayerId;
                var currentGuess = role.Guesses[targetId];
                if (currentGuess == "None") return;

                var playerRole = GetRole(voteArea);
                var playerModifier = Modifier.GetModifier(voteArea);

                var toDie = playerRole.Name == currentGuess ? playerRole.Player : Player;
                if (playerModifier != null)
                    toDie = (playerRole.Name == currentGuess || playerModifier.Name == currentGuess) ? playerRole.Player : Player;
                
                if (toDie.Is(RoleEnum.Necromancer) || toDie.Is(RoleEnum.Whisperer))
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.Data.IsImpostor()) Utils.RpcMurderPlayer(player, player);
                    }
                }

                if (!toDie.Is(RoleEnum.Pestilence))
                {
                    VigilanteKill.RpcMurderPlayer(toDie, PlayerControl.LocalPlayer);
                    RemainingKills--;
                    HideSingle(this, targetId, toDie == Player);
                    if (toDie.IsLover() && CustomGameOptions.BothLoversDie)
                    {
                        var lover = ((Lover)playerModifier).OtherLover.Player;
                        if (!lover.Is(RoleEnum.Pestilence)) HideSingle(this, lover.PlayerId, false);
                    }
                }
            }

            return Listener;
        }

        public bool IsUnGuessable(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead) return true;
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors))
            {
                if (
                    player == null ||
                    player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            }
            else
            {
                if (
                    player == null ||
                    player.Data.IsImpostor() ||
                    player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            }
            var role = GetRole(player);
            return role != null && role.Criteria();
        }
    }
}

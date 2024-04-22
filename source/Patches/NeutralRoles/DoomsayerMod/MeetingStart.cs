using HarmonyLib;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.DoomsayerMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer)) return;
            var doomsayerRole = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
            if (doomsayerRole.LastObservedPlayer != null && !CustomGameOptions.DoomsayerCantObserve)
            {
                var playerResults = PlayerReportFeedback(doomsayerRole.LastObservedPlayer);
                var roleResults = RoleReportFeedback(doomsayerRole.LastObservedPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
                if (!string.IsNullOrWhiteSpace(roleResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, roleResults);
            }
        }

        public static string PlayerReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Aurial) || player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                 || player.Is(RoleEnum.Spy) || player.Is(RoleEnum.Glitch))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an altered perception of reality";
            else if (player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Detective) || player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Snitch) || player.Is(RoleEnum.Trapper))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an insight for private information";
            else if (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Vampire))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an unusual obsession with dead bodies";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Tracker)
                || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.Werewolf) || player.Is(RoleEnum.Hunter))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} is well trained in hunting down prey";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  || player.Is(RoleEnum.Prosecutor) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} spreads fear amonst the group";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Survivor))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} hides to guard themself or others";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Mayor)
                 || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Traitor) || player.Is(RoleEnum.Veteran))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has a trick up their sleeve";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Warlock))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} is capable of performing relentless attacks";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} appears to be roleless";
            else
                return "Error";
        }

        public static string RoleReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Aurial) || player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                 || player.Is(RoleEnum.Spy) || player.Is(RoleEnum.Glitch))
                return "(Aurial, Imitator, Morphling, Mystic, Spy or The Glitch)";
            else if (player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Detective) || player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Snitch) || player.Is(RoleEnum.Trapper))
                return "(Blackmailer, Detective, Doomsayer, Oracle, Snitch or Trapper)";
            else if (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Vampire))
                return "(Altruist, Amnesiac, Janitor, Medium, Undertaker or Vampire)";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Tracker)
                || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.Werewolf) || player.Is(RoleEnum.Hunter))
                return "(Investigator, Swooper, Tracker, Vampire Hunter, Venerer, Werewolf or Hunter)";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  || player.Is(RoleEnum.Prosecutor) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter))
                return "(Arsonist, Miner, Plaguebearer, Prosecutor, Seer or Transporter)";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Survivor))
                return "(Engineer, Escapist, Grenadier, Guardian Angel, Medic or Survivor)";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Mayor)
                 || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Traitor) || player.Is(RoleEnum.Veteran))
                return "(Executioner, Jester, Mayor, Swapper, Traitor or Veteran)";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Warlock))
                return "(Bomber, Juggernaut, Pestilence, Sheriff, Vigilante or Warlock)";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return "(Crewmate or Impostor)";
            else return "Error";
        }
    }
}
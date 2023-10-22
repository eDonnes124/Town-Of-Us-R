using HarmonyLib;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.Extensions;
using TownOfUs.Patches.Localization;
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
            if (doomsayerRole.LastObservedPlayer != null)
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
                return $"{string.Format(LocalizationManager.Instance.GetString("ObservationHasAlteredPerceptionOfReality"), player.GetDefaultOutfit().PlayerName)}";
            else if (player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Detective) || player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Snitch) || player.Is(RoleEnum.Trapper))
                return $"{string.Format(LocalizationManager.Instance.GetString("ObservationHasInsightForPrivateInformation"), player.GetDefaultOutfit().PlayerName)}";
            else if (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Vampire))
                return $"{string.Format(LocalizationManager.Instance.GetString("ObservationHasUnusualObsessionWithDeadBodies"), player.GetDefaultOutfit().PlayerName)}";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Tracker)
                || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.Werewolf))
                return $"{string.Format(LocalizationManager.Instance.GetString("ObservationIsWellTrainedInHuntingDownPrey"), player.GetDefaultOutfit().PlayerName)}";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  || player.Is(RoleEnum.Prosecutor) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter))
                return $"{string.Format(LocalizationManager.Instance.GetString("ObservationSpreadsFearAmongTheGroup"), player.GetDefaultOutfit().PlayerName)}";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Survivor))
                return $"{string.Format(LocalizationManager.Instance.GetString("ObservationHidesToGuardThemselvesOrOthers"), player.GetDefaultOutfit().PlayerName)}";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Mayor)
                 || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Traitor) || player.Is(RoleEnum.Veteran))
                return $"{string.Format(LocalizationManager.Instance.GetString("ObservationHasATrickUpTheirSleeve"), player.GetDefaultOutfit().PlayerName)}";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Warlock))
                return $"{string.Format(LocalizationManager.Instance.GetString("ObservationIsCapableOfPerformingRelentlessAttacks"), player.GetDefaultOutfit().PlayerName)}";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return $"{string.Format(LocalizationManager.Instance.GetString("ObservationAppearsToBeRoleless"), player.GetDefaultOutfit().PlayerName)}";
            else
                return "Error";
        }

        public static string RoleReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Aurial) || player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                 || player.Is(RoleEnum.Spy) || player.Is(RoleEnum.Glitch))
                return $"({LocalizationManager.Instance.GetString("Aurial")}, {LocalizationManager.Instance.GetString("Imitator")}, {LocalizationManager.Instance.GetString("Morphling")}, {LocalizationManager.Instance.GetString("Mystic")}, {LocalizationManager.Instance.GetString("Spy")} {LocalizationManager.Instance.GetString("Or")} {LocalizationManager.Instance.GetString("TheGlitch")})";
            else if (player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Detective) || player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Snitch) || player.Is(RoleEnum.Trapper))
                return $"({LocalizationManager.Instance.GetString("Blackmailer")}, {LocalizationManager.Instance.GetString("Detective")}, {LocalizationManager.Instance.GetString("Doomsayer")}, {LocalizationManager.Instance.GetString("Oracle")}, {LocalizationManager.Instance.GetString("Snitch")} {LocalizationManager.Instance.GetString("Or")} {LocalizationManager.Instance.GetString("Trapper")})";
            else if (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Vampire))
                return $"({LocalizationManager.Instance.GetString("Altruist")}, {LocalizationManager.Instance.GetString("Amnesiac")}, {LocalizationManager.Instance.GetString("Janitor")}, {LocalizationManager.Instance.GetString("Medium")}, {LocalizationManager.Instance.GetString("Undertaker")} {LocalizationManager.Instance.GetString("Or")} {LocalizationManager.Instance.GetString("Vampire")})";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Tracker)
                || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.Werewolf))
                return $"({LocalizationManager.Instance.GetString("Investigator")}, {LocalizationManager.Instance.GetString("Swooper")}, {LocalizationManager.Instance.GetString("Tracker")}, {LocalizationManager.Instance.GetString("Vampire")} {LocalizationManager.Instance.GetString("Hunter")}, {LocalizationManager.Instance.GetString("Venerer")} {LocalizationManager.Instance.GetString("Or")} {LocalizationManager.Instance.GetString("Werewolf")})";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  || player.Is(RoleEnum.Prosecutor) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter))
                return $"({LocalizationManager.Instance.GetString("Arsonist")}, {LocalizationManager.Instance.GetString("Miner")}, {LocalizationManager.Instance.GetString("Plaguebearer")}, {LocalizationManager.Instance.GetString("Prosecutor")}, {LocalizationManager.Instance.GetString("Seer")} {LocalizationManager.Instance.GetString("Or")} {LocalizationManager.Instance.GetString("Transporter")})";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Survivor))
                return $"({LocalizationManager.Instance.GetString("Engineer")}, {LocalizationManager.Instance.GetString("Escapist")}, {LocalizationManager.Instance.GetString("Grenadier")}, {LocalizationManager.Instance.GetString("GuardianAngel")}, {LocalizationManager.Instance.GetString("Medic")} {LocalizationManager.Instance.GetString("Or")} {LocalizationManager.Instance.GetString("Survivor")})";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Mayor)
                 || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Traitor) || player.Is(RoleEnum.Veteran))
                return $"({LocalizationManager.Instance.GetString("Executioner")}, {LocalizationManager.Instance.GetString("Jester")}, {LocalizationManager.Instance.GetString("Mayor")}, {LocalizationManager.Instance.GetString("Swapper")}, {LocalizationManager.Instance.GetString("Traitor")} {LocalizationManager.Instance.GetString("Or")} {LocalizationManager.Instance.GetString("Veteran")})";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Warlock))
                return $"({LocalizationManager.Instance.GetString("Bomber")}, {LocalizationManager.Instance.GetString("Juggernaut")}, {LocalizationManager.Instance.GetString("Pestilence")}, {LocalizationManager.Instance.GetString("Sheriff")}, {LocalizationManager.Instance.GetString("Vigilante")} {LocalizationManager.Instance.GetString("Or")} {LocalizationManager.Instance.GetString("Warlock")})";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return $"({LocalizationManager.Instance.GetString("Crewmate")} {LocalizationManager.Instance.GetString("Or")} {LocalizationManager.Instance.GetString("Impostor")})";
            else return "Error";
        }
    }
}
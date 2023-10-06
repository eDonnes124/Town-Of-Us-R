using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.OracleMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStartOracle
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Oracle)) return;
            var oracleRole = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);
            if (oracleRole.Confessor != null)
            {
                var playerResults = PlayerReportFeedback(oracleRole.Confessor);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
            }
        }

        public static string PlayerReportFeedback(PlayerControl player)
        {
            if (player.Data.IsDead || player.Data.Disconnected) return "Your confessor failed to survive so you received no confession";
            var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != PlayerControl.LocalPlayer && x != player).ToList();
            if (allPlayers.Count < 2) return "Too few people alive to receive a confessional";
            var evilPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected &&
            (x.Is(Faction.Impostors) || (x.Is(Faction.NeutralKilling) && CustomGameOptions.NeutralKillingShowsEvil) ||
            (x.Is(Faction.NeutralEvil) && CustomGameOptions.NeutralEvilShowsEvil) || (x.Is(Faction.NeutralBenign) && CustomGameOptions.NeutralBenignShowsEvil))).ToList();
            if (evilPlayers.Count == 0) return $"{player.GetDefaultOutfit().PlayerName} confesses to knowing that there are no more evil players!"; 
            allPlayers.Shuffle();
            evilPlayers.Shuffle();
            var secondPlayer = allPlayers[0];
            var firstTwoEvil = false;
            foreach (var evilPlayer in evilPlayers)
            {
                if (evilPlayer == player || evilPlayer == secondPlayer) firstTwoEvil = true;
            }
            if (firstTwoEvil)
            {
                var thirdPlayer = allPlayers[1];
                return $"{player.GetDefaultOutfit().PlayerName} confesses to knowing that they, {secondPlayer.GetDefaultOutfit().PlayerName} and/or {thirdPlayer.GetDefaultOutfit().PlayerName} is evil!";
            }
            else
            {
                var thirdPlayer = evilPlayers[0];
                return $"{player.GetDefaultOutfit().PlayerName} confesses to knowing that they, {secondPlayer.GetDefaultOutfit().PlayerName} and/or {thirdPlayer.GetDefaultOutfit().PlayerName} is evil!";
            }
        }
    }
}
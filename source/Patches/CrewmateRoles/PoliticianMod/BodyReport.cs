using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.PoliticianMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public class BodyReport
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo info)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (info == null) return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!info.Disconnected && player.PlayerId == info.PlayerId)
                {
                    if (PlayerControl.LocalPlayer.IsInfected() || player.IsInfected())
                    {
                        foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(PlayerControl.LocalPlayer, player);
                    }
                    if (PlayerControl.LocalPlayer.IsCampaigned() || player.IsCampaigned())
                    {
                        foreach (var pn in Role.GetRoles(RoleEnum.Politician)) ((Politician)pn).RpcSpreadCampaign(PlayerControl.LocalPlayer, player);
                    }
                }
            }
        }
    }
}
using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using AmongUs.GameOptions;

namespace TownOfUs.NeutralRoles.PoliticianMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Politician)) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Politician>(PlayerControl.LocalPlayer);
            if (role.CampaignTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            if (role.CampaignedPlayers.Contains(role.ClosestPlayer.PlayerId)) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[0] == true)
            {
                role.LastCampaigned = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastCampaigned = DateTime.UtcNow;
                role.LastCampaigned.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.CampaignCd);
                return false;
            }
            /* Mercenary compatibility
            else if (interact[5] == true)
            {
                role.LastCampaigned = DateTime.UtcNow;
                role.LastCampaigned.AddSeconds(-CustomGameOptions.CampaignCd + 10);
                return false;
            }*/
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
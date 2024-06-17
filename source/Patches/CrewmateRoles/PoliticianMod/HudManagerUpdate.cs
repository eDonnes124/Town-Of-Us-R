using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.NeutralRoles.PoliticianMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Politician)) return;
            var isDead = PlayerControl.LocalPlayer.Data.IsDead;
            var infectButton = __instance.KillButton;
            var role = Role.GetRole<Politician>(PlayerControl.LocalPlayer);

            foreach (var playerId in role.CampaignedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                var data = player?.Data;
                if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead || playerId == PlayerControl.LocalPlayer.PlayerId)
                    continue;

                player.myRend().material.SetColor("_VisorColor", role.Color);
                player.nameText().color = Color.cyan;
            }

            infectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            infectButton.SetCoolDown(role.CampaignTimer(), CustomGameOptions.CampaignCd);

            var notCampaigned = PlayerControl.AllPlayerControls.ToArray().Where(
                player => !role.CampaignedPlayers.Contains(player.PlayerId)
            ).ToList();

            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, notCampaigned);

            if (role.IsElected && !isDead)
            {
                role.TurnMayor();
                Utils.Rpc(CustomRPC.TurnMayor, PlayerControl.LocalPlayer.PlayerId);
            }
        }
    }
}
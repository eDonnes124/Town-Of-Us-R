using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) return;
            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);

            foreach (var playerId in role.DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                var data = player?.Data;
                if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                    continue;

                player.myRend().material.SetColor("_VisorColor", role.Color);
                player.nameText().color = Color.black;
            }

            __instance.AbilityButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && GameManager.Instance.GameHasStarted);

            if (!role.LastKiller || !CustomGameOptions.IgniteCdRemoved) __instance.AbilityButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.DouseCd);
            else __instance.AbilityButton.SetCoolDown(0f, CustomGameOptions.DouseCd);
            if (role.DousedAlive < CustomGameOptions.MaxDoused)
            {
                __instance.KillButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.DouseCd);
            }

            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(
                player => !role.DousedPlayers.Contains(player.PlayerId)
            ).ToList();
            var doused = PlayerControl.AllPlayerControls.ToArray().Where(
                player => role.DousedPlayers.Contains(player.PlayerId)
            ).ToList();

            if (role.DousedAlive < CustomGameOptions.MaxDoused)
            {
                Utils.SetTarget(ref role.ClosestPlayerDouse, __instance.KillButton, targets: notDoused);
            }

            if (role.DousedAlive > 0)
            {
                var player = PlayerControl.LocalPlayer;
                role.ClosestPlayerIgnite = Utils.SetClosestPlayer(ref player, targets: doused);
            }

            if (role.ClosestPlayerIgnite == null)
            {
                __instance.AbilityButton.graphic.color = Palette.DisabledClear;
                __instance.AbilityButton.graphic.material.SetFloat("_Desat", 1f);
            }
            else if (role.ClosestPlayerIgnite != null)
            {
                __instance.AbilityButton.graphic.color = Palette.EnabledColor;
                __instance.AbilityButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}

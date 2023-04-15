using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.WerewolfMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static Sprite RampageSprite => TownOfUs.RampageSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf)) return;
            var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.RampageKillCd);

            __instance.AbilityButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            if (role.Rampaged)
            {
                __instance.AbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.RampageDuration);
                Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN);
            }
            else
            {
                __instance.AbilityButton.SetCoolDown(role.RampageTimer(), CustomGameOptions.RampageCd);

                __instance.AbilityButton.graphic.color = Palette.EnabledColor;
                __instance.AbilityButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}

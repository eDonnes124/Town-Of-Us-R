using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.CamouflagerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Camouflage => TownOfUs.CamouflageSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Camouflager)) return;
            var role = Role.GetRole<Camouflager>(PlayerControl.LocalPlayer);
            if (role.CamouflagerButton == null)
            {
                role.CamouflagerButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.CamouflagerButton.graphic.enabled = true;
                role.CamouflagerButton.graphic.sprite = Camouflage;
                role.CamouflagerButton.gameObject.SetActive(false);
            }

            role.CamouflagerButton.graphic.sprite = Camouflage;

            role.CamouflagerButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var position = __instance.KillButton.transform.localPosition;
            role.CamouflagerButton.transform.localPosition = new Vector3(__instance.SabotageButton.transform.localPosition.x,
                __instance.ImpostorVentButton.transform.localPosition.y, position.z);

            if (role.Enabled)
            {
                role.CamouflagerButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.CamouflagerDuration);
                return;
            }

            role.CamouflagerButton.SetCoolDown(role.CamouflageTimer(), CustomGameOptions.CamouflagerCd);
            role.CamouflagerButton.graphic.color = Palette.EnabledColor;
            role.CamouflagerButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}
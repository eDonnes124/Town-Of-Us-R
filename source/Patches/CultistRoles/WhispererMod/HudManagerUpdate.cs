using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using UnityEngine;

namespace TownOfUs.CultistRoles.WhispererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite WhisperSprite => TownOfUs.WhisperSprite;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer)) return;
            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);
            if (role.WhisperButton == null)
            {
                role.WhisperButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.WhisperButton.graphic.enabled = true;
                role.WhisperButton.gameObject.SetActive(false);
            }

            role.WhisperButton.graphic.sprite = WhisperSprite;
            role.WhisperButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && GameManager.Instance.GameHasStarted);

            role.WhisperButton.SetCoolDown(role.WhisperTimer(),
                CustomGameOptions.WhisperCooldown + CustomGameOptions.IncreasedCooldownPerWhisper * role.WhisperCount);

            var renderer = role.WhisperButton.graphic;
            if (!role.WhisperButton.isCoolingDown && role.WhisperButton.gameObject.active)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }
}
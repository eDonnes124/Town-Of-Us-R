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
        public static void Postfix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer)) return;
            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            role.RoleAbilityButton.graphic.sprite = WhisperSprite;

            role.RoleAbilityButton.SetCoolDown(role.WhisperTimer(),
                CustomGameOptions.WhisperCooldown + CustomGameOptions.IncreasedCooldownPerWhisper * role.WhisperCount);

            var renderer = role.RoleAbilityButton.graphic;
            if (!role.RoleAbilityButton.isCoolingDown && role.RoleAbilityButton.gameObject.active)
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
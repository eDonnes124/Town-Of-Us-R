using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.BomberMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite PlantSprite => TownOfUs.PlantSprite;
        public static Sprite DetonateSprite => TownOfUs.DetonateSprite;

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Bomber)) return;
            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);

            if (role.Detonating)
            {
                role.RoleAbilityButton.graphic.sprite = DetonateSprite;
                role.DetonateTimer();
                role.RoleAbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.DetonateDelay);
            }
            else
            {
                role.RoleAbilityButton.graphic.sprite = PlantSprite;
                if (!role.Detonated) role.DetonateKillStart();
                if (PlayerControl.LocalPlayer.killTimer > 0)
                {
                    role.RoleAbilityButton.graphic.color = Palette.DisabledClear;
                    role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 1f);
                }
                else
                {
                    role.RoleAbilityButton.graphic.color = Palette.EnabledColor;
                    role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 0f);
                }
                role.RoleAbilityButton.SetCoolDown(PlayerControl.LocalPlayer.killTimer,
                    GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }

            role.RoleAbilityButton.graphic.color = Palette.EnabledColor;
            role.RoleAbilityButton.graphic.material.SetFloat("_Desat", 0f);
            if (role.RoleAbilityButton.graphic.sprite == PlantSprite) role.RoleAbilityButton.SetCoolDown(PlayerControl.LocalPlayer.killTimer, 
                GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            else role.RoleAbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.DetonateDelay);
        }
    }
}

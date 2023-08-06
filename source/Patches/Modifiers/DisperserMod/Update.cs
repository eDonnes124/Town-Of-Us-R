using HarmonyLib;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using TMPro;

namespace TownOfUs.Modifiers.DisperserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        public static Sprite DisperseButton => TownOfUs.DisperseSprite;

        public static void Postfix(HudManager __instance)
        {
            UpdateButtonButton(__instance);
        }

        private static void UpdateButtonButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Disperser)) return;

            var role = Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer);

            role.RoleAbilityButton.graphic.sprite = DisperseButton;

            role.RoleAbilityButton.SetCoolDown(role.StartTimer(), 10f);
            var renderer = role.RoleAbilityButton.graphic;


            if (__instance.UseButton != null)
            {
                var position1 = __instance.UseButton.transform.position;
                role.RoleAbilityButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }
            else
            {
                var position1 = __instance.PetButton.transform.position;
                role.RoleAbilityButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }

            if (!role.ButtonUsed)
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
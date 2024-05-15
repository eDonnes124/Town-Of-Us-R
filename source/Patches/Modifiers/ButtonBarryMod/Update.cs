using HarmonyLib;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using TMPro;

namespace TownOfUs.Modifiers.ButtonBarryMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        public static Sprite Button => TownOfUs.ButtonSprite;

        public static void Postfix(HudManager __instance)
        {
            UpdateButtonButton(__instance);
        }

        private static void UpdateButtonButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.ButtonBarry)) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) return;

            var role = Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer);

            if (role.ButtonButton == null)
            {
                role.ButtonButton = Object.Instantiate(__instance.KillButton, __instance.transform.parent);
                role.ButtonButton.GetComponentsInChildren<TextMeshPro>()[0].text = "";
                role.ButtonButton.graphic.enabled = true;
                role.ButtonButton.graphic.sprite = Button;
            }

            role.ButtonButton.graphic.sprite = Button;

            role.ButtonButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && GameManager.Instance.GameHasStarted
                    && !(CustomGameOptions.ButtonBarryGetsCooldown && EmergencyCooldownPatch.Time < CustomGameOptions.InitialCooldowns));

            role.ButtonButton.SetCoolDown(role.StartTimer(), 10f);
            var renderer = role.ButtonButton.graphic;

            if (__instance.UseButton != null)
            {
                var position1 = __instance.UseButton.transform.position;
                role.ButtonButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }
            else
            {
                var position1 = __instance.PetButton.transform.position;
                role.ButtonButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }

            if (!role.ButtonUsed && PlayerControl.LocalPlayer.RemainingEmergencies > 0)
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
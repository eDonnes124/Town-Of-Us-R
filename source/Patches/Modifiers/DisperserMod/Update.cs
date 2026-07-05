using HarmonyLib;
using TownOfUs.Modifiers;
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

            if (role.DisperseButton == null)
            {
                role.DisperseButton = Object.Instantiate(__instance.KillButton, __instance.transform.parent);
                foreach (var text in role.DisperseButton.GetComponentsInChildren<TextMeshPro>()) text.text = "";
                role.DisperseButton.graphic.enabled = true;
                role.DisperseButton.graphic.sprite = DisperseButton;
            }

            role.DisperseButton.graphic.sprite = DisperseButton;

            role.DisperseButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.DisperseButton.SetCoolDown(role.StartTimer(), 10f);
            var renderer = role.DisperseButton.graphic;

            if (PlayerControl.LocalPlayer.Is(ModifierEnum.ButtonBarry) || PlayerControl.LocalPlayer.Is(ModifierEnum.Satellite))
            {
                var position = __instance.KillButton.transform.position;
                role.DisperseButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position.y,
                    position.z);
            }
            else
            {
                if (__instance.UseButton != null)
                {
                    var position = __instance.UseButton.transform.position;
                    role.DisperseButton.transform.position = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position.y,
                        position.z);
                }
                else
                {
                    var position = __instance.PetButton.transform.position;
                    role.DisperseButton.transform.position = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position.y,
                        position.z);
                }
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
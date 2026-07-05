using HarmonyLib;
using TownOfUs.Modifiers;
using UnityEngine;
using TMPro;

namespace TownOfUs.Modifiers.SatelliteMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        public static Sprite Detect => TownOfUs.DetectSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Satellite)) return;

            var role = Modifier.GetModifier<Satellite>(PlayerControl.LocalPlayer);

            if (role.DetectButton == null)
            {
                role.DetectButton = Object.Instantiate(__instance.KillButton, __instance.transform.parent);
                foreach (var text in role.DetectButton.GetComponentsInChildren<TextMeshPro>()) text.text = "";
                role.DetectButton.graphic.enabled = true;
                role.DetectButton.graphic.sprite = Detect;
            }

            role.DetectButton.graphic.sprite = Detect;

            role.DetectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.DetectButton.SetCoolDown(role.StartTimer(), 10f);
            var renderer = role.DetectButton.graphic;

            if (__instance.UseButton != null)
            {
                var position1 = __instance.UseButton.transform.position;
                role.DetectButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }
            else
            {
                var position1 = __instance.PetButton.transform.position;
                role.DetectButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }

            if (!role.DetectUsed)
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
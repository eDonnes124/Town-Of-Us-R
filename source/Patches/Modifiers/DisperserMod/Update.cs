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


            if (__instance.UseButton != null)
            {
                var position1 = __instance.UseButton.transform.position;
                role.DisperseButton.transform.position = new Vector3(
                __instance.UseButton.transform.position.x,    
                __instance.UseButton.transform.position.y + 2f, 
                __instance.ReportButton.transform.position.z);
            }
            else
            {
                var position1 = __instance.PetButton.transform.position;
                role.DisperseButton.transform.position = new Vector3(
                __instance.UseButton.transform.position.x,    
                __instance.UseButton.transform.position.y + 2f, 
                __instance.ReportButton.transform.position.z);
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
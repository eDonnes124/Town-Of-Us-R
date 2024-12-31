using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.TimeLordMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDRewind
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateRewindButton(__instance);
        }

        public static void UpdateRewindButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var rewindButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<TimeLord>(PlayerControl.LocalPlayer);


            if (isDead)
            {
                rewindButton.gameObject.SetActive(false);
              //  rewindButton.isActive = false;
            }
            else
            {
                rewindButton.gameObject.SetActive(!MeetingHud.Instance);
                //  rewindButton.isActive = !MeetingHud.Instance;
                if (role.ButtonUsable)
                    rewindButton.SetCoolDown(role.TimeLordRewindTimer(), role.GetCooldown());
            }

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(rewindButton.cooldownTimerText, rewindButton.transform);
                role.UsesText.gameObject.SetActive(true);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.UsesLeft + "";
            }

            var renderer = rewindButton.graphic;
            if (!rewindButton.isCoolingDown & !RecordRewind.rewinding & rewindButton.enabled && role.ButtonUsable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
            role.UsesText.color = Palette.DisabledClear;
            role.UsesText.material.SetFloat("_Desat", 0f);
        }
    }
}
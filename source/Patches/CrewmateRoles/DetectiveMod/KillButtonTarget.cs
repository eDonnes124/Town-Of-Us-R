using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class KillButtonTarget
    {
        public static byte DontRevive = byte.MaxValue;

        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Detective)) return true;
            else
            {
                var detective = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                if (__instance == detective.ExamineButton) return true;
                else return false;
            }
        }

        public static void SetTarget(KillButton __instance, CrimeScene target, Detective role)
        {
            if (role.CurrentTarget && role.CurrentTarget != target)
            {
                role.CurrentTarget.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Outline", 0f);
            }

            if (target != null && target.DeadPlayer.PlayerId == DontRevive) target = null;
            role.CurrentTarget = target;
            if (role.CurrentTarget && __instance.enabled)
            {
                SpriteRenderer component = role.CurrentTarget.gameObject.GetComponent<SpriteRenderer>();
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.yellow);
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 1f);
        }
    }
}
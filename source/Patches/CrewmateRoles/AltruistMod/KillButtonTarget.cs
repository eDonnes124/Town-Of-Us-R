using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.AltruistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class KillButtonTarget
    {
        public static byte DontRevive = byte.MaxValue;
        public static bool Prefix()
        {
            return !PlayerControl.LocalPlayer.Is(RoleEnum.Altruist);
        }
    }
}
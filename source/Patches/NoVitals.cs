using HarmonyLib;
using UnityEngine;
using TownOfUs.Roles;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    public class NoVitals
    {
        public static bool Prefix(VitalsMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return true;
            if (CustomGameOptions.GameMode == GameMode.Cultist ||
                (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter) && !CustomGameOptions.TransporterVitals))
            {
                Object.Destroy(__instance.gameObject);
                return false;
            }

           

            return true;
        }
    }
}
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.Roles
{
    public class RoleColorIndicators
    {
        [HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
        class SetVentOutlinePatch
        {
            public static void Postfix(Vent __instance, [HarmonyArgument(1)] ref bool mainTarget)
            {
                var player = PlayerControl.LocalPlayer;
                var role = Role.GetRole(player);
                Color color = role.Color;
                __instance.myRend.material.SetColor("_OutlineColor", color);
                __instance.myRend.material.SetColor("_AddColor", mainTarget ? color : Color.clear);
            }
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ToggleHighlight))]
        class ToggleHighlightPatch
        {
            public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] bool active, [HarmonyArgument(1)] RoleTeamTypes team)
            {
                var player = PlayerControl.LocalPlayer;
                var role = Role.GetRole(player);
                {
                    __instance.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", role.Color);
                }
            }
        }
    }
}

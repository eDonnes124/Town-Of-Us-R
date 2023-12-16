using HarmonyLib;
using System;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    public static class SpeedPatch
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixPhysics(PlayerPhysics __instance)
        {
            if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove && !__instance.myPlayer.Data.IsDead)
            {
                __instance.body.velocity *= __instance.myPlayer.GetAppearance().SpeedFactor;
                foreach (var role in Role.GetRoles(RoleEnum.Venerer))
                {
                    var venerer = (Venerer)role;
                    if (venerer.Enabled)
                    {
                        if (venerer.KillsAtStartAbility >= 2 && venerer.Player == PlayerControl.LocalPlayer) __instance.body.velocity *= CustomGameOptions.SprintSpeed;
                        else if (venerer.KillsAtStartAbility >= 3) __instance.body.velocity *= CustomGameOptions.FreezeSpeed;
                    }
                }
                foreach (var modifier in Modifier.GetModifiers(ModifierEnum.Frosty))
                {
                    var frosty = (Frosty)modifier;
                    if (frosty.IsChilled && frosty.Chilled == PlayerControl.LocalPlayer)
                    {
                        var utcNow = DateTime.UtcNow;
                        var timeSpan = utcNow - frosty.LastChilled;
                        var duration = CustomGameOptions.ChillDuration * 1000f;
                        if ((float)timeSpan.TotalMilliseconds < duration)
                        {
                            __instance.body.velocity *= 1 - (duration - (float)timeSpan.TotalMilliseconds)
                                * (1 - CustomGameOptions.ChillStartSpeed) / duration;
                        }
                        else frosty.IsChilled = false;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CustomNetworkTransform), nameof(CustomNetworkTransform.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostfixNetwork(CustomNetworkTransform __instance)
        {
            if (!__instance.AmOwner && GameData.Instance && __instance.gameObject.GetComponent<PlayerControl>().CanMove && !__instance.gameObject.GetComponent<PlayerControl>().Data.IsDead)
            {
                var player = __instance.gameObject.GetComponent<PlayerControl>();
                __instance.body.velocity *= player.GetAppearance().SpeedFactor;

                foreach (var role in Role.GetRoles(RoleEnum.Venerer))
                {
                    var venerer = (Venerer)role;
                    if (venerer.Enabled)
                    {
                        if (venerer.KillsAtStartAbility >= 2 && venerer.Player == player) __instance.body.velocity *= CustomGameOptions.SprintSpeed;
                        else if (venerer.KillsAtStartAbility >= 3) __instance.body.velocity *= CustomGameOptions.FreezeSpeed;
                    }
                }
                foreach (var modifier in Modifier.GetModifiers(ModifierEnum.Frosty))
                {
                    var frosty = (Frosty)modifier;
                    if (frosty.IsChilled && frosty.Chilled == player)
                    {
                        var utcNow = DateTime.UtcNow;
                        var timeSpan = utcNow - frosty.LastChilled;
                        var duration = CustomGameOptions.ChillDuration * 1000f;
                        if ((float)timeSpan.TotalMilliseconds < duration)
                        {
                            __instance.body.velocity *= 1 - (duration - (float)timeSpan.TotalMilliseconds)
                                * (1 - CustomGameOptions.ChillStartSpeed) / duration;
                        }
                        else frosty.IsChilled = false;
                    }
                }
            }
        }
    }
}
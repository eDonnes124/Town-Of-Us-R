using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.MercenaryMod
{
    public enum ShieldOptions
    {
        Mercenary = 0,
        Self = 1,
        SelfAndMerc = 2,
        Everyone = 3
    }

    public enum NotificationOptions
    {
        Mercenary = 0,
        Shielded = 1,
        MercAndShielded = 2,
        Everyone = 3,
        Nobody = 4
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowMercShield
    {
        public static Color ProtectedColor = Color.cyan;

        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Mercenary))
            {
                var merc = (Mercenary) role;

                var exPlayer = merc.exShielded;
                if (exPlayer != null)
                {
                    System.Console.WriteLine(exPlayer.name + " is ex-Shielded and unvisored");
                    exPlayer.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    exPlayer.myRend().material.SetFloat("_Outline", 0f);
                    merc.exShielded = null;
                    continue;
                }

                var player = merc.ShieldedPlayer;
                if (player == null) continue;

                if (player.Data.IsDead || merc.Player.Data.IsDead || merc.Player.Data.Disconnected)
                {
                    StopAbility.BreakShield(merc.Player.PlayerId, player.PlayerId);
                    continue;
                }

                var showShielded = CustomGameOptions.ShowMercShielded;
                if (showShielded == ShieldOptions.Everyone)
                {
                    player.myRend().material.SetColor("_VisorColor", ProtectedColor);
                    player.myRend().material.SetFloat("_Outline", 1f);
                    player.myRend().material.SetColor("_OutlineColor", ProtectedColor);
                }
                else if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId && (showShielded == ShieldOptions.Self ||
                    showShielded == ShieldOptions.SelfAndMerc))
                {
                    player.myRend().material.SetColor("_VisorColor", ProtectedColor);
                    player.myRend().material.SetFloat("_Outline", 1f);
                    player.myRend().material.SetColor("_OutlineColor", ProtectedColor);
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Mercenary) && merc.ShieldedPlayer.PlayerId == player.PlayerId && 
                         (showShielded == ShieldOptions.Mercenary || showShielded == ShieldOptions.SelfAndMerc))
                {
                    player.myRend().material.SetColor("_VisorColor", ProtectedColor);
                    player.myRend().material.SetFloat("_Outline", 1f);
                    player.myRend().material.SetColor("_OutlineColor", ProtectedColor);
                }
            }
        }
    }
}
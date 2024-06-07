using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.MercenaryMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopAbility
    {
        public static void BreakShield(byte mercId, byte playerId)
        {
            if (CustomGameOptions.NotificationMercShield == NotificationOptions.Everyone)
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));
            else if (PlayerControl.LocalPlayer.PlayerId == playerId &&
                (CustomGameOptions.NotificationMercShield == NotificationOptions.Shielded || CustomGameOptions.NotificationMercShield == NotificationOptions.MercAndShielded))
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));
            else if (PlayerControl.LocalPlayer.PlayerId == mercId &&
                (CustomGameOptions.NotificationMercShield == NotificationOptions.Mercenary || CustomGameOptions.NotificationMercShield == NotificationOptions.MercAndShielded))
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));

            var player = Utils.PlayerById(playerId);
            foreach (var role in Role.GetRoles(RoleEnum.Mercenary))
                if (((Mercenary)role).ShieldedPlayer.PlayerId == playerId && ((Mercenary)role).Player.PlayerId == mercId)
                {
                    var merc = (Mercenary)role;
                    merc.ShieldedPlayer = null;
                    merc.exShielded = player;
                    merc.Brilders += 1;
                    merc.RegenTask();
                    System.Console.WriteLine(player.name + " Is Ex-Shielded");
                }

            player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.myRend().material.SetFloat("_Outline", 0f);
        }
    }
}
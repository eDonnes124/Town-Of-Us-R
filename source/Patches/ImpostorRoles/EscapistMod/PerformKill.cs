using HarmonyLib;
using Hazel;
using Reactor.Networking.Extensions;
using System;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.EscapistMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformKill
    {
        public static Sprite MarkSprite => TownOfUs.MarkSprite;
        public static Sprite EscapeSprite => TownOfUs.EscapeSprite;

        public static bool Prefix(AbilityButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Escapist);
            if (!flag) return true;
            if (RoleManager.IsGhostRole(PlayerControl.LocalPlayer.Data.RoleType)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Escapist>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            if (!__instance.isActiveAndEnabled) return false;
            if (__instance.graphic.sprite == MarkSprite)
            {
                role.EscapePoint = PlayerControl.LocalPlayer.transform.position;
                __instance.graphic.sprite = EscapeSprite;
                HudManager.Instance.KillButton.SetTarget(null);
                if (role.EscapeTimer() < 5f)
                    role.LastEscape = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.EscapeCd);
            }
            else
            {
                if (__instance.isCoolingDown) return false;
                if (role.EscapeTimer() != 0) return false;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.Escape, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(role.EscapePoint);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.LastEscape = DateTime.UtcNow;
                Escapist.Escape(role.Player);
            }

            return false;
        }
    }
}

using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Utilities;
using System;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Detective)) return true;
            var role = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.ExamineTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                var hasKilled = false;
                foreach (var player in Murder.KilledPlayers)
                {
                    if (player.KillerId == role.ClosestPlayer.PlayerId && (float)(DateTime.UtcNow - player.KillTime).TotalSeconds < CustomGameOptions.RecentKill)
                    {
                        hasKilled = true;
                    }
                }
                if (hasKilled) Coroutines.Start(Utils.FlashCoroutine(Color.red));
                else Coroutines.Start(Utils.FlashCoroutine(Color.green));
                role.LastExaminedPlayer = role.ClosestPlayer;
            }
            if (interact[0] == true)
            {
                role.LastExamined = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastExamined = DateTime.UtcNow;
                role.LastExamined = role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ExamineCd);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}

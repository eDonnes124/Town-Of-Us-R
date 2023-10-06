using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.AurialMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Aurial)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
            if (!(role.RadiateTimer() == 0f)) return false;
            if (!__instance.enabled) return false;
            role.LastRadiated = System.DateTime.UtcNow;

            var players = Utils.GetClosestPlayers(PlayerControl.LocalPlayer.GetTruePosition(), CustomGameOptions.RadiateRange, false);
            foreach ( var player in players)
            {
                if (UnityEngine.Random.Range(0, 100) > CustomGameOptions.RadiateChance) continue;

                if (role.knownPlayerRoles.TryGetValue(player.PlayerId, out int val)) {
                    role.knownPlayerRoles[player.PlayerId] = val + 1;
                } else role.knownPlayerRoles.Add(player.PlayerId, 1);
            }

            return false;
        }
    }
}
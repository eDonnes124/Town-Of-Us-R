using HarmonyLib;
using TownOfUs.Extensions;
using AmongUs.GameOptions;

namespace TownOfUs
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopImpKill
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Data.IsImpostor()) return true;
            var target = __instance.currentTarget;
            if (target == null) return true;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return true;
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                if (!target.inVent) Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, target);
                return false;
            }
            var interact = Utils.Interact(PlayerControl.LocalPlayer, target, true);
            if (interact[4] == true) return false;
            else if (interact[0] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                return false;
            }
            else if (interact[1] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset + 0.01f);
                return false;
            }
            else if (interact[2] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.VestKCReset + 0.01f);
                return false;
            }
            else if (interact[3] == true)
            {
                PlayerControl.LocalPlayer.SetKillTimer(0.01f);
                return false;
            }
            return false;
        }
    }
}
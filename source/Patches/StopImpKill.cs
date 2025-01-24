using HarmonyLib;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using TownOfUs.Modifiers.UnderdogMod;
using TownOfUs.CrewmateRoles.SheriffMod;
using TownOfUs.Roles;
using Reactor.Utilities;
using static TownOfUs.Roles.Glitch;

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
            HUDKill.ImpKillTarget(__instance);
            target = __instance.currentTarget;
            if (target == null) return false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Warlock) && PlayerControl.LocalPlayer.IsHacked())
            {
                Coroutines.Start(AbilityCoroutine.Hack(PlayerControl.LocalPlayer));
                return false;
            }
            var interact = Utils.Interact(PlayerControl.LocalPlayer, target, true);
            if (interact[4] == true) return false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Warlock))
            {
                var warlock = Role.GetRole<Warlock>(PlayerControl.LocalPlayer);
                if (warlock.Charging)
                {
                    warlock.UsingCharge = true;
                    warlock.ChargeUseDuration = warlock.ChargePercent * CustomGameOptions.ChargeUseDuration / 100f;
                    if (warlock.ChargeUseDuration == 0f) warlock.ChargeUseDuration += 0.01f;
                }
                PlayerControl.LocalPlayer.SetKillTimer(0.01f);
            }
            else if (interact[0] == true)
            {
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Scavenger))
                {
                    var scav = Role.GetRole<Scavenger>(PlayerControl.LocalPlayer);
                    if (scav.Target == target) PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ScavengeCorrectKillCooldown);
                    else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * CustomGameOptions.ScavengeIncorrectKillCooldown);
                }
                else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    PlayerControl.LocalPlayer.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                }
                else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
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
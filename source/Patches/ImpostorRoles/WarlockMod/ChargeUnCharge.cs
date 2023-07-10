using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Modifiers.UnderdogMod;

namespace TownOfUs.ImpostorRoles.WarlockMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ChargeUnCharge
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Warlock)) return;
            foreach (var role in Role.GetRoles(RoleEnum.Warlock))
            {
                var warlock = (Warlock) role;
                if (warlock.Charging)
                    warlock.ChargePercent = warlock.ChargeUpTimer();
                else if (warlock.UsingCharge)
                {
                    warlock.ChargePercent = warlock.ChargeUseTimer();
                    if (warlock.ChargePercent <= 0f)
                    {
                        warlock.UsingCharge = false;
                        if (warlock.Player.Is(ModifierEnum.Underdog))
                        {
                            var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                            var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                            var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                            warlock.Player.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                        }
                        else warlock.Player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                    }
                }
            }
        }
    }
}
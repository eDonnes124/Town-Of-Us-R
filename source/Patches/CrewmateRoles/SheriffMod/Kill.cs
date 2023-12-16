using System;
using HarmonyLib;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using TownOfUs.Patches;

namespace TownOfUs.CrewmateRoles.SheriffMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class Kill
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff);
            if (!flag) return true;
            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var flag2 = role.SheriffKillTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled || role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;

            var flag4 = role.ClosestPlayer.Data.IsImpostor() ||
                        role.ClosestPlayer.Is(RoleEnum.Doomsayer) && CustomGameOptions.SheriffKillsDoomsayer ||
                        role.ClosestPlayer.Is(RoleEnum.Vampire) && CustomGameOptions.SheriffKillsVampire ||
                        role.ClosestPlayer.Is(RoleEnum.Jester) && CustomGameOptions.SheriffKillsJester ||
                        role.ClosestPlayer.Is(RoleEnum.Glitch) && CustomGameOptions.SheriffKillsGlitch ||
                        role.ClosestPlayer.Is(RoleEnum.Juggernaut) && CustomGameOptions.SheriffKillsJuggernaut ||
                        role.ClosestPlayer.Is(RoleEnum.Executioner) && CustomGameOptions.SheriffKillsExecutioner ||
                        role.ClosestPlayer.Is(RoleEnum.Arsonist) && CustomGameOptions.SheriffKillsArsonist ||
                        role.ClosestPlayer.Is(RoleEnum.Werewolf) && CustomGameOptions.SheriffKillsWerewolf ||
                        role.ClosestPlayer.Is(RoleEnum.Plaguebearer) && CustomGameOptions.SheriffKillsPlaguebearer;

            if (role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                return false;
            }
            if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }
            if (role.ClosestPlayer.IsOnAlert())
            {
                if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    Utils.Rpc(CustomRPC.AttemptSound, medic, role.ClosestPlayer.PlayerId);

                    if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                    Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                }
                else if (role.Player.IsShielded())
                {
                    var medic = role.Player.GetMedic().Player.PlayerId;
                    Utils.Rpc(CustomRPC.AttemptSound, medic, role.Player.PlayerId);
                    if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;
                    StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                    if (CustomGameOptions.SheriffKillOther && !role.ClosestPlayer.IsProtected() && CustomGameOptions.KilledOnAlert)
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                }
                else
                {
                    Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    if (CustomGameOptions.KilledOnAlert && CustomGameOptions.SheriffKillOther)
                    {
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    }
                }

                return false;
            }
            else if (role.ClosestPlayer == ShowRoundOneShield.FirstRoundShielded) return false;
            else if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                Utils.Rpc(CustomRPC.AttemptSound, medic, role.ClosestPlayer.PlayerId);

                if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;

                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                return false;
            }
            else if (role.ClosestPlayer.IsVesting())
            {
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);

                return false;
            }
            else if (role.ClosestPlayer.IsProtected())
            {
                if (!flag4)
                {
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                }
                role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                return false;
            }

            if (role.ClosestPlayer.Is(RoleEnum.Necromancer) || role.ClosestPlayer.Is(RoleEnum.Whisperer))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor() && !player.Is(RoleEnum.Necromancer)
                        && !player.Is(RoleEnum.Whisperer)) Utils.RpcMurderPlayer(player, player);
                }
            }

            if (!flag4)
            {
                if (CustomGameOptions.SheriffKillOther)
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                role.LastKilled = DateTime.UtcNow;
            }
            else
            {
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                role.LastKilled = DateTime.UtcNow;
            }
            return false;
        }
    }
}

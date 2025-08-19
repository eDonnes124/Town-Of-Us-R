using System;
using HarmonyLib;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using TownOfUs.Patches;
using Reactor.Utilities;
using UnityEngine;
using TownOfUs.CrewmateRoles.ClericMod;

namespace TownOfUs.CrewmateRoles.SheriffMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class Kill
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButton __instance)
        {
            if (__instance != HudManager.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff);
            if (!flag) return true;
            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var flag2 = role.SheriffKillTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled || role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < LegacyGameOptions.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;

            var flag4 = role.ClosestPlayer.Data.IsImpostor() ||
                        (role.ClosestPlayer.Is(Faction.NeutralEvil) && CustomGameOptions.SheriffKillsNE) ||
                        (role.ClosestPlayer.Is(Faction.NeutralKilling) && CustomGameOptions.SheriffKillsNK);

            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            if (role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                return false;
            }
            if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }
            if (role.ClosestPlayer.IsFortified())
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Crewmate));
                foreach (var warden in role.ClosestPlayer.GetWarden())
                {
                    Utils.Rpc(CustomRPC.Fortify, (byte)1, warden.Player.PlayerId);
                }
                return false;
            }
            else if (role.ClosestPlayer == ShowShield.FirstRoundShielded) return false;
            else if (role.ClosestPlayer.IsOnAlert())
            {
                if (role.ClosestPlayer.IsShielded())
                {
                    foreach (var medic in role.ClosestPlayer.GetMedic())
                    {
                        Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, role.ClosestPlayer.PlayerId);
                        StopKill.BreakShield(medic.Player.PlayerId, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                    }

                    if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;

                    Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));

                    if (!PlayerControl.LocalPlayer.IsProtected() && !PlayerControl.LocalPlayer.IsBarriered())
                    {
                        Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    }
                    else if (role.SheriffKillTimer() <= 0f)
                    {
                        if (PlayerControl.LocalPlayer.IsBarriered())
                        {
                            foreach (var cleric in PlayerControl.LocalPlayer.GetCleric())
                            {
                                StopAttack.NotifyCleric(cleric.Player.PlayerId, false);
                            }
                        }
                        role.LastKilled = DateTime.UtcNow;
                        role.LastKilled = role.LastKilled.AddSeconds(CustomGameOptions.TempSaveCdReset - CustomGameOptions.SheriffKillCd);
                    }
                }
                else if (role.Player.IsShielded())
                {
                    foreach (var medic in role.Player.GetMedic())
                    {
                        Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, role.Player.PlayerId);
                        StopKill.BreakShield(medic.Player.PlayerId, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                    role.LastKilled = DateTime.UtcNow;
                    if (CustomGameOptions.SheriffKillOther && !role.ClosestPlayer.IsProtected() && !role.ClosestPlayer.IsBarriered() && CustomGameOptions.KilledOnAlert)
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    if (role.ClosestPlayer.IsBarriered())
                    {
                        foreach (var cleric in role.ClosestPlayer.GetCleric())
                        {
                            StopAttack.NotifyCleric(cleric.Player.PlayerId);
                        }
                    }
                }
                else
                {
                    if (!PlayerControl.LocalPlayer.IsProtected() && !PlayerControl.LocalPlayer.IsBarriered())
                    {
                        Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    }
                    else
                    {
                        if (PlayerControl.LocalPlayer.IsBarriered())
                        {
                            foreach (var cleric in PlayerControl.LocalPlayer.GetCleric())
                            {
                                StopAttack.NotifyCleric(cleric.Player.PlayerId, false);
                            }
                        }
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                    }
                    if (CustomGameOptions.SheriffKillOther && !role.ClosestPlayer.IsProtected() && !role.ClosestPlayer.IsBarriered() && CustomGameOptions.KilledOnAlert)
                    {
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    }
                    else if (role.ClosestPlayer.IsBarriered())
                    {
                        foreach (var cleric in role.ClosestPlayer.GetCleric())
                        {
                            StopAttack.NotifyCleric(cleric.Player.PlayerId);
                        }
                    }
                    role.LastKilled = DateTime.UtcNow;
                }

                return false;
            }
            else if (role.ClosestPlayer.IsShielded())
            {
                foreach (var medic in role.ClosestPlayer.GetMedic())
                {
                    Utils.Rpc(CustomRPC.AttemptSound, medic.Player.PlayerId, role.ClosestPlayer.PlayerId);
                    StopKill.BreakShield(medic.Player.PlayerId, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                }

                if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;

                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));

                return false;
            }
            else if (role.ClosestPlayer.IsVesting())
            {
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);

                return false;
            }
            else if (role.ClosestPlayer.IsProtected() || role.ClosestPlayer.IsBarriered())
            {
                role.LastKilled = DateTime.UtcNow;
                if (!flag4) Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                else
                {
                    if (role.ClosestPlayer.IsBarriered())
                    {
                        foreach (var cleric in role.ClosestPlayer.GetCleric())
                        {
                            StopAttack.NotifyCleric(cleric.Player.PlayerId);
                        }
                    }
                    role.LastKilled = role.LastKilled.AddSeconds(CustomGameOptions.TempSaveCdReset - CustomGameOptions.SheriffKillCd);
                }
                return false;
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

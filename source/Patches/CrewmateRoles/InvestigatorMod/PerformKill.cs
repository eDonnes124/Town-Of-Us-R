using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Reactor.Utilities;
using AmongUs.GameOptions;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return true;
            var role = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];

            var flag2 = role.InvestigateTimer() == 0f;
                if (!flag2) return false;
                if (role.ClosestPlayer == null) return false;
                if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    if (!role.ClosestPlayer.Is(RoleEnum.Amnesiac) && !role.ClosestPlayer.Is(RoleEnum.Mayor) && !role.ClosestPlayer.Is(RoleEnum.Altruist) && 
                    !role.ClosestPlayer.Is(RoleEnum.Aurial) && !role.ClosestPlayer.Is(RoleEnum.Vigilante) && !role.ClosestPlayer.Is(RoleEnum.Doomsayer) &&
                    !role.ClosestPlayer.Is(RoleEnum.Engineer) && !role.ClosestPlayer.Is(RoleEnum.Executioner) && !role.ClosestPlayer.Is(RoleEnum.GuardianAngel) &&
                    !role.ClosestPlayer.Is(RoleEnum.Jester) && !role.ClosestPlayer.Is(RoleEnum.Medium) && !role.ClosestPlayer.Is(RoleEnum.Crewmate) && 
                    !role.ClosestPlayer.Is(RoleEnum.Mystic) && !role.ClosestPlayer.Is(RoleEnum.Prosecutor) && !role.ClosestPlayer.Is(RoleEnum.Snitch) &&
                    !role.ClosestPlayer.Is(RoleEnum.Spy) && !role.ClosestPlayer.Is(RoleEnum.Survivor) && !role.ClosestPlayer.Is(RoleEnum.Swapper) &&
                    !role.ClosestPlayer.Is(RoleEnum.Transporter) && !role.ClosestPlayer.Is(RoleEnum.Trapper) && !role.ClosestPlayer.Is(RoleEnum.Veteran))
                    {
                        if (role.InvestigatedPlayer != null)
                        {
                            role.PreviouslyInvestigated.Add(role.InvestigatedPlayer.PlayerId);
                            foreach (RoleEnum role1 in MeetingStart.RoleList) Role.GetRole(role.InvestigatedPlayer).PossibleRoles.Add(role1);
                        }

                        role.InvestigatedPlayer = role.ClosestPlayer;
                        Utils.Rpc(CustomRPC.Investigate, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                        role.LastInvestigated = DateTime.UtcNow;
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        role.StartRemoving = false;
                        role.ShouldNotify = false;
                        Utils.Rpc(CustomRPC.NotifyInvestigator, PlayerControl.LocalPlayer.PlayerId);
                        role.ShouldRemove = false;
                        MeetingStart.RoleList.Clear();
                    }
                    else if (role.ClosestPlayer.Is(RoleEnum.Doomsayer) && !CustomGameOptions.DoomsayerCantObserve)
                    {
                        if (role.InvestigatedPlayer != null)
                        {
                            role.PreviouslyInvestigated.Add(role.InvestigatedPlayer.PlayerId);
                            foreach (RoleEnum role1 in MeetingStart.RoleList) Role.GetRole(role.InvestigatedPlayer).PossibleRoles.Add(role1);
                        }

                        role.InvestigatedPlayer = role.ClosestPlayer;
                        Utils.Rpc(CustomRPC.Investigate, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                        role.LastInvestigated = DateTime.UtcNow;
                        Coroutines.Start(Utils.FlashCoroutine(Color.green)); 
                        role.StartRemoving = false;
                        role.ShouldNotify = false;
                        Utils.Rpc(CustomRPC.NotifyInvestigator, PlayerControl.LocalPlayer.PlayerId);
                        role.ShouldRemove = false;
                        MeetingStart.RoleList.Clear();
                        MeetingStart.OnCrewRoles.Clear();
                        MeetingStart.OnNeutRoles.Clear();
                        MeetingStart.OnImpRoles.Clear();
                        MeetingStart.SetRole = false;
                    }
                    else Coroutines.Start(Utils.FlashCoroutine(Color.red));
                }
                if (interact[0] == true)
                {
                    role.LastInvestigated = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastInvestigated = DateTime.UtcNow;
                    role.LastInvestigated = role.LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ExamineCd);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
        }
    }
}
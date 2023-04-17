using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.TrapperMod;
using System.Collections.Generic;

namespace TownOfUs.CrewmateRoles.ImitatorMod
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            if (__instance == null)
            {
                return;
            }
            if (StartImitate.ImitatingPlayer != null && !StartImitate.ImitatingPlayer.Is(RoleEnum.Traitor))
            {
                PlayerControl lastExaminedPlayer = null;
                List<RoleEnum> trappedPlayers = null;

                if (PlayerControl.LocalPlayer == StartImitate.ImitatingPlayer)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(Role.GetRole<Investigator>(PlayerControl.LocalPlayer));

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                    {
                        var engineerRole = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
                        Object.Destroy(engineerRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                    {
                        var trackerRole = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                        trackerRole.TrackerArrows.Values.DestroyAll();
                        trackerRole.TrackerArrows.Clear();
                        Object.Destroy(trackerRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                    {
                        var mysticRole = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);
                        mysticRole.BodyArrows.Values.DestroyAll();
                        mysticRole.BodyArrows.Clear();
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                    {
                        var transporterRole = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                        Object.Destroy(transporterRole.UsesText);
                        if (transporterRole.TransportList != null)
                        {
                            transporterRole.TransportList.Toggle();
                            transporterRole.TransportList.SetVisible(false);
                            transporterRole.TransportList = null;
                            transporterRole.PressedButton = false;
                            transporterRole.TransportPlayer1 = null;
                        }
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                    {
                        var veteranRole = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                        Object.Destroy(veteranRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
                    {
                        var medRole = Role.GetRole<Medium>(PlayerControl.LocalPlayer);
                        medRole.MediatedPlayers.Values.DestroyAll();
                        medRole.MediatedPlayers.Clear();
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
                    {
                        var trapperRole = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
                        Object.Destroy(trapperRole.UsesText);
                        trapperRole.traps.ClearTraps();
                        trappedPlayers = trapperRole.trappedPlayers;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
                    {
                        var detecRole = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                        lastExaminedPlayer = detecRole.LastExaminedPlayer;
                    }

                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator) && !PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)
                        && !PlayerControl.LocalPlayer.Is(RoleEnum.Spy)) DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                }
                var role = Role.GetRole(StartImitate.ImitatingPlayer);
                var killsList = (role.Kills, role.CorrectKills, role.IncorrectKills, role.CorrectAssassinKills, role.IncorrectAssassinKills);
                Role.RoleDictionary.Remove(StartImitate.ImitatingPlayer.PlayerId);
                var imitator = new Imitator(StartImitate.ImitatingPlayer);
                imitator.trappedPlayers = trappedPlayers;
                imitator.LastExaminedPlayer = lastExaminedPlayer;
                var newRole = Role.GetRole(StartImitate.ImitatingPlayer);
                newRole.RemoveFromRoleHistory(newRole.RoleType);
                newRole.Kills = killsList.Kills;
                newRole.CorrectKills = killsList.CorrectKills;
                newRole.IncorrectKills = killsList.IncorrectKills;
                newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
                newRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                Role.GetRole<Imitator>(StartImitate.ImitatingPlayer).ImitatePlayer = null;
                StartImitate.ImitatingPlayer = null;
            }
            return;
        }
    }
}

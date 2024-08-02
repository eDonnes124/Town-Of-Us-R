using HarmonyLib;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static List<RoleEnum> RoleList = new List<RoleEnum>();
        public static List<RoleEnum> OnCrewRoles = new List<RoleEnum>();
        public static List<RoleEnum> OnNeutRoles = new List<RoleEnum>();
        public static List<RoleEnum> OnImpRoles = new List<RoleEnum>();
        public static List<RoleEnum> FakeRole = new List<RoleEnum>();
        public static bool SetRole {get; set;} = false;

        public static RoleEnum LastRole;
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return;
            var role = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);
            if (role.InvestigatedPlayer != null)
            {
                var playerResults = PlayerReportFeedback(role);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
            }
        }

        public static string PlayerReportFeedback(Investigator investigator)
        {
            if (LastRole != Role.GetRole(investigator.InvestigatedPlayer).RoleType && SetRole)
            {
                investigator.ShouldRemove = false;
                investigator.StartRemoving = false;
            }
            if (investigator.PreviouslyInvestigated.Contains(investigator.InvestigatedPlayer.PlayerId) && Role.GetRole(investigator.InvestigatedPlayer).PossibleRoles.Count() > 0)
            {
                string basestring = $"You decided to investigate {investigator.InvestigatedPlayer.name} again.\n";
                Role.GetRole(investigator.InvestigatedPlayer).PossibleRoles.Clear();
                SetRole = true;
                LastRole = Role.GetRole(investigator.InvestigatedPlayer).RoleType;
                return basestring + RoleList.Join();
            }
            else if (!investigator.StartRemoving && investigator.InvestigatedPlayer != null)
            {
                investigator.StartRemoving = true;
                string basestring = $"You investigated {investigator.InvestigatedPlayer.name} and found out they are either:\n";

                if (LastRole != Role.GetRole(investigator.InvestigatedPlayer).RoleType && SetRole) 
                basestring = $"You noticed {investigator.InvestigatedPlayer.name}'s acting differently, they must have changed their role!\n";

                if (CustomGameOptions.ArsonistOn > 0) OnNeutRoles.Add(RoleEnum.Arsonist);
                if (CustomGameOptions.BlackmailerOn > 0) OnImpRoles.Add(RoleEnum.Blackmailer);
                if (CustomGameOptions.BomberOn > 0) OnImpRoles.Add(RoleEnum.Bomber);
                if (CustomGameOptions.DetectiveOn > 0) OnCrewRoles.Add(RoleEnum.Detective);
                if (CustomGameOptions.DoomsayerOn > 0 && !CustomGameOptions.DoomsayerCantObserve) OnNeutRoles.Add(RoleEnum.Doomsayer);
                if (CustomGameOptions.EscapistOn > 0) OnImpRoles.Add(RoleEnum.Escapist);
                if (CustomGameOptions.GlitchOn > 0) OnNeutRoles.Add(RoleEnum.Glitch);
                if (CustomGameOptions.GrenadierOn > 0) OnImpRoles.Add(RoleEnum.Grenadier);
                if (CustomGameOptions.HunterOn > 0) OnCrewRoles.Add(RoleEnum.Hunter);
                if (CustomGameOptions.JanitorOn > 0) OnImpRoles.Add(RoleEnum.Janitor);
                if (CustomGameOptions.HiddenRoles || CustomGameOptions.GameMode == GameMode.AllAny) OnNeutRoles.Add(RoleEnum.Juggernaut);
                if (CustomGameOptions.MedicOn > 0) OnCrewRoles.Add(RoleEnum.Medic);
                if (CustomGameOptions.MinerOn > 0) OnImpRoles.Add(RoleEnum.Miner);
                if (CustomGameOptions.MorphlingOn > 0) OnImpRoles.Add(RoleEnum.Morphling);
                if (CustomGameOptions.OracleOn > 0) OnCrewRoles.Add(RoleEnum.Oracle);
                if (CustomGameOptions.PlaguebearerOn > 0) OnNeutRoles.Add(RoleEnum.Pestilence);
                if (CustomGameOptions.PlaguebearerOn > 0) OnNeutRoles.Add(RoleEnum.Plaguebearer);
                if (CustomGameOptions.SeerOn > 0) OnCrewRoles.Add(RoleEnum.Seer);
                if (CustomGameOptions.SheriffOn > 0) OnCrewRoles.Add(RoleEnum.Sheriff);
                if (CustomGameOptions.SwooperOn > 0) OnImpRoles.Add(RoleEnum.Swooper);
                if (CustomGameOptions.TrackerOn > 0) OnCrewRoles.Add(RoleEnum.Tracker);
                if (CustomGameOptions.TraitorOn > 0) OnImpRoles.Add(RoleEnum.Traitor);
                if (CustomGameOptions.UndertakerOn > 0) OnImpRoles.Add(RoleEnum.Undertaker);
                if (CustomGameOptions.VampireOn > 0) OnNeutRoles.Add(RoleEnum.Vampire);
                if (CustomGameOptions.VampireHunterOn > 0) OnCrewRoles.Add(RoleEnum.VampireHunter);
                if (CustomGameOptions.VenererOn > 0) OnImpRoles.Add(RoleEnum.Venerer);
                if (CustomGameOptions.WarlockOn > 0) OnImpRoles.Add(RoleEnum.Warlock);
                if (CustomGameOptions.WerewolfOn > 0) OnNeutRoles.Add(RoleEnum.Werewolf);
                if (CustomGameOptions.ImitatorOn > 0) OnCrewRoles.Add(RoleEnum.Imitator);
                OnImpRoles.Add(RoleEnum.Impostor);

                if (investigator.InvestigatedPlayer.Is(Faction.Crewmates)) OnCrewRoles.Remove(Role.GetRole(investigator.InvestigatedPlayer).RoleType);
                else if (investigator.InvestigatedPlayer.Is(Faction.NeutralBenign)) OnNeutRoles.Remove(Role.GetRole(investigator.InvestigatedPlayer).RoleType);
                else if (investigator.InvestigatedPlayer.Is(Faction.NeutralEvil)) OnNeutRoles.Remove(Role.GetRole(investigator.InvestigatedPlayer).RoleType);
                else if (investigator.InvestigatedPlayer.Is(Faction.NeutralKilling)) OnNeutRoles.Remove(Role.GetRole(investigator.InvestigatedPlayer).RoleType);
                else if (investigator.InvestigatedPlayer.Is(Faction.Impostors)) OnNeutRoles.Remove(Role.GetRole(investigator.InvestigatedPlayer).RoleType);
                float shouldaddcrew = 0;
                float shouldaddneut = 0;
                float shouldaddimp = 0;

                if (investigator.RoleCount == 3)
                {
                    shouldaddcrew = 1;
                    shouldaddneut = 1;
                    shouldaddimp = 1;
                }
                else if (investigator.RoleCount == 4)
                {
                    shouldaddcrew = 1;
                    shouldaddneut = 2;
                    shouldaddimp = 1;
                }
                else if (investigator.RoleCount == 5)
                {
                    shouldaddcrew = 2;
                    shouldaddneut = 2;
                    shouldaddimp = 1;
                }
                else if (investigator.RoleCount == 6)
                {
                    shouldaddcrew = 2;
                    shouldaddneut = 2;
                    shouldaddimp = 2;
                }
                else if (investigator.RoleCount == 7)
                {
                    shouldaddcrew = 2;
                    shouldaddneut = 3;
                    shouldaddimp = 2;
                }
                else if (investigator.RoleCount == 8)
                {
                    shouldaddcrew = 3;
                    shouldaddneut = 3;
                    shouldaddimp = 2;
                }
                if (investigator.InvestigatedPlayer.Is(Faction.Crewmates)) shouldaddcrew--;
                else if (investigator.InvestigatedPlayer.Is(Faction.NeutralBenign)) shouldaddneut--;
                else if (investigator.InvestigatedPlayer.Is(Faction.NeutralEvil)) shouldaddneut--;
                else if (investigator.InvestigatedPlayer.Is(Faction.NeutralKilling)) shouldaddneut--;
                else if (investigator.InvestigatedPlayer.Is(Faction.Impostors)) shouldaddimp--;

                OnCrewRoles.Shuffle();
                while (shouldaddcrew > 0)
                {
                    RoleList.Add(OnCrewRoles[UnityEngine.Random.RandomRangeInt(0, OnCrewRoles.Count)]);
                    shouldaddcrew--;
                }
                OnNeutRoles.Shuffle();
                while (shouldaddneut > 0)
                {
                    RoleList.Add(OnNeutRoles[UnityEngine.Random.RandomRangeInt(0, OnNeutRoles.Count)]);
                    shouldaddneut--;
                }
                OnImpRoles.Shuffle();
                while (shouldaddimp > 0)
                {
                    RoleList.Add(OnImpRoles[UnityEngine.Random.RandomRangeInt(0, OnImpRoles.Count)]);
                    shouldaddimp--;
                }
                RoleList.Add(Role.GetRole(investigator.InvestigatedPlayer).RoleType);
                RoleList.Shuffle();

                SetRole = true;
                LastRole = Role.GetRole(investigator.InvestigatedPlayer).RoleType;
                return basestring + RoleList.Join();
            }
            else if (investigator.ShouldRemove && investigator.InvestigatedPlayer != null)
            {
                investigator.ShouldRemove = false;
                string basestring = $"You were able to narrow down {investigator.InvestigatedPlayer.name}'s role!\n";
                string cantnarrow = $"You weren't able to narrow down {investigator.InvestigatedPlayer.name}'s role more.\n";
                int RemoveRoles = RoleList.Count - 2;

                if (investigator.NarrowCount > 0)
                {
                    foreach (RoleEnum role in RoleList) FakeRole.Add(role);
                    FakeRole.Remove(Role.GetRole(investigator.InvestigatedPlayer).RoleType);
                    while (RemoveRoles > 0)
                    {
                        FakeRole.Remove(FakeRole[UnityEngine.Random.RandomRangeInt(0, FakeRole.Count)]);
                        RemoveRoles--;
                    }
                    foreach (RoleEnum role in FakeRole) RoleList.Remove(role);
                    FakeRole.Clear();
                }

                LastRole = Role.GetRole(investigator.InvestigatedPlayer).RoleType;
                SetRole = true;
                if (investigator.NarrowCount > 0) return basestring + RoleList.Join();
                else return cantnarrow + RoleList.Join();
            }
            else return "You weren't able to gather information";
        }
    }
}
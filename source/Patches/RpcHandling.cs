using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using Reactor.Networking.Extensions;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.CrewmateRoles.VigilanteMod;
using TownOfUs.CultistRoles.NecromancerMod;
using TownOfUs.CustomOption;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using TownOfUs.ImpostorRoles.MinerMod;
using TownOfUs.CrewmateRoles.HaunterMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using Coroutine = TownOfUs.ImpostorRoles.JanitorMod.Coroutine;
using Object = UnityEngine.Object;
using PerformKillButton = TownOfUs.NeutralRoles.AmnesiacMod.PerformKillButton;
using Random = UnityEngine.Random; //using Il2CppSystem;
using TownOfUs.Patches;
using TownOfUs.CrewmateRoles.ImitatorMod;
using AmongUs.GameOptions;

namespace TownOfUs
{
    public static class RpcHandling
    {
        private static readonly List<(Type, int, int, bool)> CrewmateRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> NeutralNonKillingRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> NeutralKillingRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> ImpostorRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int)> CrewmateModifiers = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> GlobalModifiers = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> ImpostorModifiers = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> ButtonModifiers = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> AssassinModifiers = new List<(Type, int, int)>();
        private static readonly List<(Type, CustomRPC, int)> AssassinAbility = new List<(Type, CustomRPC, int)>();

        internal static bool Check(int probability)
        {
            if (probability == 0) return false;
            if (probability == 100) return true;
            var num = Random.RandomRangeInt(1, 101);
            return num <= probability;
        }
        internal static bool CheckJugg()
        {
            var num = Random.RandomRangeInt(1, 101);
            return num <= 10 * NeutralKillingRoles.Count;
        }

        private static void SortRoles(List<(Type, int, int, bool)> roles, int max, int min)
        {
            roles.Shuffle();
            if (roles.Count < max) max = roles.Count;
            if (min > max) min = max;
            var amount = Random.RandomRangeInt(min, max + 1);
            roles.Sort((a, b) =>
            {
                var a_ = a.Item3 == 100 ? 0 : 100;
                var b_ = b.Item3 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });
            var certainRoles = 0;
            var odds = 0;
            foreach (var role in roles)
                if (role.Item3 == 100) certainRoles += 1;
                else odds += role.Item3;
            while (certainRoles < amount)
            {
                var num = certainRoles;
                var random = Random.RandomRangeInt(0, odds);
                var rolePicked = false;
                while (num < roles.Count && rolePicked == false)
                {
                    random -= roles[num].Item3;
                    if (random < 0)
                    {
                        odds -= roles[num].Item3;
                        var role = roles[num];
                        roles.Remove(role);
                        roles.Insert(0, role);
                        certainRoles += 1;
                        rolePicked = true;
                    }
                    num += 1;
                }
            }
            while (roles.Count > amount) roles.RemoveAt(roles.Count - 1);
        }

        private static void SortModifiers(List<(Type, int, int)> roles, int max)
        {
            roles.Shuffle();
            roles.Sort((a, b) =>
            {
                var a_ = a.Item3 == 100 ? 0 : 100;
                var b_ = b.Item3 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });
            while (roles.Count > max) roles.RemoveAt(roles.Count - 1);
        }

        private static void GenEachRole(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();
            impostors.Shuffle();

            if (CustomGameOptions.GameMode == GameMode.Classic)
            {
                if (crewmates.Count > CustomGameOptions.MaxNeutralNonKillingRoles)
                    SortRoles(NeutralNonKillingRoles, CustomGameOptions.MaxNeutralNonKillingRoles, CustomGameOptions.MinNeutralNonKillingRoles);
                else SortRoles(NeutralNonKillingRoles, crewmates.Count - 1, CustomGameOptions.MinNeutralNonKillingRoles);
                if (crewmates.Count - NeutralNonKillingRoles.Count > CustomGameOptions.MaxNeutralKillingRoles)
                    SortRoles(NeutralKillingRoles, CustomGameOptions.MaxNeutralKillingRoles, CustomGameOptions.MinNeutralKillingRoles);
                else SortRoles(NeutralKillingRoles, crewmates.Count - NeutralNonKillingRoles.Count - 1, CustomGameOptions.MinNeutralKillingRoles);

                if (CheckJugg() && NeutralKillingRoles.Count > 0 && CustomGameOptions.HiddenRoles)
                {
                    NeutralKillingRoles.RemoveAt(NeutralKillingRoles.Count - 1);
                    NeutralKillingRoles.Add((typeof(Juggernaut), 18, 100, true));
                    NeutralKillingRoles.Shuffle();
                }

                SortRoles(CrewmateRoles, crewmates.Count - NeutralNonKillingRoles.Count - NeutralKillingRoles.Count,
                    crewmates.Count - NeutralNonKillingRoles.Count - NeutralKillingRoles.Count);
                SortRoles(ImpostorRoles, impostors.Count, impostors.Count);
            }

            var crewAndNeutralRoles = new List<(Type, int, int, bool)>();
            if (CustomGameOptions.GameMode == GameMode.Classic) crewAndNeutralRoles.AddRange(CrewmateRoles);
            crewAndNeutralRoles.AddRange(NeutralNonKillingRoles);
            crewAndNeutralRoles.AddRange(NeutralKillingRoles);

            var crewRoles = new List<(Type, int, int, bool)>();
            var impRoles = new List<(Type, int, int, bool)>();

            if (CustomGameOptions.GameMode == GameMode.AllAny)
            {
                crewAndNeutralRoles.Shuffle();
                if (crewAndNeutralRoles.Count > 0)
                {
                    crewRoles.Add(crewAndNeutralRoles[0]);
                    if (crewAndNeutralRoles[0].Item4 == true) crewAndNeutralRoles.Remove(crewAndNeutralRoles[0]);
                }
                if (CrewmateRoles.Count > 0)
                {
                    CrewmateRoles.Shuffle();
                    crewRoles.Add(CrewmateRoles[0]);
                    if (CrewmateRoles[0].Item4 == true) CrewmateRoles.Remove(CrewmateRoles[0]);
                }
                else
                {
                    crewRoles.Add((typeof(Crewmate), 38, 100, false));
                }
                crewAndNeutralRoles.AddRange(CrewmateRoles);
                while (crewRoles.Count < crewmates.Count && crewAndNeutralRoles.Count > 0)
                {
                    crewAndNeutralRoles.Shuffle();
                    crewRoles.Add(crewAndNeutralRoles[0]);
                    if (crewAndNeutralRoles[0].Item4 == true)
                    {
                        if (CrewmateRoles.Contains(crewAndNeutralRoles[0])) CrewmateRoles.Remove(crewAndNeutralRoles[0]);
                        crewAndNeutralRoles.Remove(crewAndNeutralRoles[0]);
                    }
                }
                while (impRoles.Count < impostors.Count && ImpostorRoles.Count > 0)
                {
                    ImpostorRoles.Shuffle();
                    impRoles.Add(ImpostorRoles[0]);
                    if (ImpostorRoles[0].Item4 == true) ImpostorRoles.Remove(ImpostorRoles[0]);
                }
            }
            crewRoles.Shuffle();
            impRoles.Shuffle();

            SortModifiers(CrewmateModifiers, crewmates.Count);
            SortModifiers(GlobalModifiers, crewmates.Count + impostors.Count);
            SortModifiers(ImpostorModifiers, impostors.Count);
            SortModifiers(ButtonModifiers, crewmates.Count + impostors.Count);

            if (CustomGameOptions.GameMode == GameMode.AllAny)
            {
                foreach (var (type, id, _, unique) in crewRoles)
                {
                    Role.GenRole<Role>(type, crewmates, id);
                }
                foreach (var (type, id, _, unique) in impRoles)
                {
                    Role.GenRole<Role>(type, impostors, id);
                }
            }
            else
            {
                foreach (var (type, id, _, unique) in crewAndNeutralRoles)
                {
                    Role.GenRole<Role>(type, crewmates, id);
                }
                foreach (var (type, id, _, unique) in ImpostorRoles)
                {
                    Role.GenRole<Role>(type, impostors, id);
                }
            }

            foreach (var crewmate in crewmates)
                Role.GenRole<Role>(typeof(Crewmate), crewmate, 38);

            foreach (var impostor in impostors)
                Role.GenRole<Role>(typeof(Impostor), impostor, 39);

            var canHaveModifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveImpModifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            canHaveImpModifier.RemoveAll(player => !player.Is(Faction.Impostors));
            var canHaveAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility2 = PlayerControl.AllPlayerControls.ToArray().ToList();
            canHaveModifier.Shuffle();
            canHaveAbility.RemoveAll(player => !player.Is(Faction.Impostors));
            canHaveAbility.Shuffle();
            canHaveAbility2.RemoveAll(player => !player.Is(Faction.NeutralKilling));
            canHaveAbility2.Shuffle();
            var impAssassins = CustomGameOptions.NumberOfImpostorAssassins;
            var neutAssassins = CustomGameOptions.NumberOfNeutralAssassins;

            while (canHaveAbility.Count > 0 && impAssassins > 0)
            {
                var (type, rpc, _) = AssassinAbility.Ability();
                Role.Gen<Ability>(type, canHaveAbility.TakeFirst(), rpc);
                impAssassins -= 1;
            }

            while (canHaveAbility2.Count > 0 && neutAssassins > 0)
            {
                var (type, rpc, _) = AssassinAbility.Ability();
                Role.Gen<Ability>(type, canHaveAbility2.TakeFirst(), rpc);
                neutAssassins -= 1;
            }

            var canHaveAssassinModifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            canHaveAssassinModifier.RemoveAll(player => !player.Is(Faction.Impostors) || !player.Is(AbilityEnum.Assassin));

            foreach (var (type, id, _) in AssassinModifiers)
            {
                if (canHaveAssassinModifier.Count == 0) break;
                Role.GenModifier<Modifier>(type, canHaveAssassinModifier, id);
            }

            canHaveImpModifier.RemoveAll(player => player.Is(ModifierEnum.DoubleShot));

            foreach (var (type, id, _) in ImpostorModifiers)
            {
                if (canHaveImpModifier.Count == 0) break;
                Role.GenModifier<Modifier>(type, canHaveImpModifier, id);
            }

            canHaveModifier.RemoveAll(player => player.Is(ModifierEnum.Disperser) || player.Is(ModifierEnum.DoubleShot) || player.Is(ModifierEnum.Underdog));

            foreach (var (type, id, _) in GlobalModifiers)
            {
                if (canHaveModifier.Count == 0) break;
                if(id == 0)
                {
                    if (canHaveModifier.Count == 1) continue;
                        Lover.Gen(canHaveModifier);
                }
                else
                {
                    Role.GenModifier<Modifier>(type, canHaveModifier, id);
                }
            }

            canHaveModifier.RemoveAll(player => player.Is(RoleEnum.Glitch));

            foreach (var (type, id, _) in ButtonModifiers)
            {
                if (canHaveModifier.Count == 0) break;
                Role.GenModifier<Modifier>(type, canHaveModifier, id);
            }

            canHaveModifier.RemoveAll(player => player.Is(Faction.NeutralKilling) || player.Is(Faction.Impostors));
            canHaveModifier.Shuffle();

            while (canHaveModifier.Count > 0 && CrewmateModifiers.Count > 0)
            {
                var (type, id, _) = CrewmateModifiers.TakeFirst();
                Role.GenModifier<Modifier>(type, canHaveModifier.TakeFirst(), id);
            }

            var exeTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ModifierEnum.Lover) && !x.Is(RoleEnum.Mayor) && !x.Is(RoleEnum.Swapper) && !x.Is(RoleEnum.Vigilante)).ToList();
            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                if (exeTargets.Count > 0)
                {
                    exe.target = exeTargets[Random.RandomRangeInt(0, exeTargets.Count)];
                    exeTargets.Remove(exe.target);

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetTarget, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(exe.target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            var gaTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.NeutralOther) && !x.Is(Faction.NeutralKilling) && !x.Is(ModifierEnum.Lover)).ToList();
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                if (gaTargets.Count > 0)
                {
                    ga.target = gaTargets[Random.RandomRangeInt(0, gaTargets.Count)];
                    gaTargets.Remove(ga.target);

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetGATarget, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(ga.target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
        private static void GenEachRoleKilling(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();
            impostors.Shuffle();

            ImpostorRoles.Add((typeof(Undertaker), 33, 10, true));
            ImpostorRoles.Add((typeof(Morphling), 31, 10, false));
            ImpostorRoles.Add((typeof(Escapist), 37, 10, false));
            ImpostorRoles.Add((typeof(Miner), 29, 10, true));
            ImpostorRoles.Add((typeof(Swooper), 30, 10, false));
            ImpostorRoles.Add((typeof(Grenadier), 34, 10, true));

            SortRoles(ImpostorRoles, impostors.Count, impostors.Count);

            NeutralKillingRoles.Add((typeof(Glitch), 11, 10, true));
            NeutralKillingRoles.Add((typeof(Werewolf), 27, 10, true));
            if (CustomGameOptions.HiddenRoles)
                NeutralKillingRoles.Add((typeof(Juggernaut), 18, 10, true));
            if (CustomGameOptions.AddArsonist)
                NeutralKillingRoles.Add((typeof(Arsonist), 13, 10, true));
            if (CustomGameOptions.AddPlaguebearer)
                NeutralKillingRoles.Add((typeof(Plaguebearer), 26, 10, true));

            var neutrals = 0;
            if (NeutralKillingRoles.Count < CustomGameOptions.NeutralRoles) neutrals = NeutralKillingRoles.Count;
            else neutrals = CustomGameOptions.NeutralRoles;
            var spareCrew = crewmates.Count - neutrals;
            if (spareCrew > 2) SortRoles(NeutralKillingRoles, neutrals, neutrals);
            else SortRoles(NeutralKillingRoles, crewmates.Count - 3, crewmates.Count - 3);

            var veterans = CustomGameOptions.VeteranCount;
            while (veterans > 0)
            {
                CrewmateRoles.Add((typeof(Veteran), 16, 10, false));
                veterans -= 1;
            }
            var vigilantes = CustomGameOptions.VigilanteCount;
            while (vigilantes > 0)
            {
                CrewmateRoles.Add((typeof(Vigilante), 15, 10, false));
                vigilantes -= 1;
            }
            if (CrewmateRoles.Count + NeutralKillingRoles.Count > crewmates.Count)
            {
                SortRoles(CrewmateRoles, crewmates.Count - NeutralKillingRoles.Count, crewmates.Count - NeutralKillingRoles.Count);
            }
            else if (CrewmateRoles.Count + NeutralKillingRoles.Count < crewmates.Count)
            {
                var sheriffs = crewmates.Count - NeutralKillingRoles.Count - CrewmateRoles.Count;
                while (sheriffs > 0)
                {
                    CrewmateRoles.Add((typeof(Sheriff), 0, 10, false));
                    sheriffs -= 1;
                }
            }

            var crewAndNeutralRoles = new List<(Type, int, int, bool)>();
            crewAndNeutralRoles.AddRange(CrewmateRoles);
            crewAndNeutralRoles.AddRange(NeutralKillingRoles);
            crewAndNeutralRoles.Shuffle();
            ImpostorRoles.Shuffle();

            foreach (var (type, id, _, unique) in crewAndNeutralRoles)
            {
                Role.GenRole<Role>(type, crewmates, id);
            }
            foreach (var (type, id, _, unique) in ImpostorRoles)
            {
                Role.GenRole<Role>(type, impostors, id);
            }
        }
        private static void GenEachRoleCultist(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();
            impostors.Shuffle();

            var specialRoles = new List<(Type, int, int, bool)>();
            var crewRoles = new List<(Type, int, int, bool)>();
            var impRole = new List<(Type, int, int, bool)>();
            if (CustomGameOptions.MayorCultistOn > 0) specialRoles.Add((typeof(Mayor), 3, CustomGameOptions.MayorCultistOn, true));
            if (CustomGameOptions.SeerCultistOn > 0) specialRoles.Add((typeof(CultistSeer), 102, CustomGameOptions.SeerCultistOn, true));
            if (CustomGameOptions.SheriffCultistOn > 0) specialRoles.Add((typeof(Sheriff), 0, CustomGameOptions.SheriffCultistOn, true));
            if (CustomGameOptions.SurvivorCultistOn > 0) specialRoles.Add((typeof(Survivor), 23, CustomGameOptions.SurvivorCultistOn, true));
            if (specialRoles.Count > CustomGameOptions.SpecialRoleCount) SortRoles(specialRoles, CustomGameOptions.SpecialRoleCount, CustomGameOptions.SpecialRoleCount);
            if (specialRoles.Count > crewmates.Count) SortRoles(specialRoles, crewmates.Count, crewmates.Count);
            if (specialRoles.Count < crewmates.Count)
            {
                var chameleons = CustomGameOptions.MaxChameleons;
                var engineers = CustomGameOptions.MaxEngineers;
                var investigators = CustomGameOptions.MaxInvestigators;
                var mystics = CustomGameOptions.MaxMystics;
                var spies = CustomGameOptions.MaxSpies;
                var transporters = CustomGameOptions.MaxTransporters;
                var vigilantes = CustomGameOptions.MaxVigilantes;
                while (chameleons > 0)
                {
                    crewRoles.Add((typeof(Chameleon), 104, 10, false));
                    chameleons--;
                }
                while (engineers > 0)
                {
                    crewRoles.Add((typeof(Engineer), 2, 10, false));
                    engineers--;
                }
                while (investigators > 0)
                {
                    crewRoles.Add((typeof(Investigator), 5, 10, false));
                    investigators--;
                }
                while (mystics > 0)
                {
                    crewRoles.Add((typeof(CultistMystic), 103, 10, false));
                    mystics--;
                }
                while (spies > 0)
                {
                    crewRoles.Add((typeof(Spy), 10, 10, false));
                    spies--;
                }
                while (transporters > 0)
                {
                    crewRoles.Add((typeof(Transporter), 20, 10, false));
                    transporters--;
                }
                while (vigilantes > 0)
                {
                    crewRoles.Add((typeof(Vigilante), 15, 10, false));
                    vigilantes--;
                }
                SortRoles(crewRoles, crewmates.Count - specialRoles.Count, crewmates.Count - specialRoles.Count);
            }
            impRole.Add((typeof(Necromancer), 100, 10, true));
            impRole.Add((typeof(Whisperer), 101, 10, true));
            SortRoles(impRole, 1, 1);

            foreach (var (type, id, _, unique) in specialRoles)
            {
                Role.GenRole<Role>(type, crewmates, id);
            }
            foreach (var (type, id, _, unique) in crewRoles)
            {
                Role.GenRole<Role>(type, crewmates, id);
            }
            foreach (var (type, id, _, unique) in impRole)
            {
                Role.GenRole<Role>(type, impostors, id);
            }

            foreach (var crewmate in crewmates)
                Role.GenRole<Role>(typeof(Crewmate), crewmate, 38);
        }


        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpc
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                byte readByte, readByte1, readByte2;
                sbyte readSByte, readSByte2;
                switch ((CustomRPC) callId)
                {
                    case CustomRPC.SetRole:
                        var player = Utils.PlayerById(reader.ReadByte());
                        var roleID = reader.ReadInt32();
                        switch (roleID)
                        {
                            case 0:
                                new Sheriff(player);
                                break;
                            case 1:
                                new Jester(player);
                                break;
                            case 2:
                                new Engineer(player);
                                break;
                            case 3:
                                new Mayor(player);
                                break;
                            case 4:
                                new Swapper(player);
                                break;
                            case 5:
                                new Investigator(player);
                                break;
                            case 7:
                                new Medic(player);
                                break;
                            case 8:
                                new Seer(player);
                                break;
                            case 9:
                                new Executioner(player);
                                break;
                            case 10:
                                new Spy(player);
                                break;
                            case 11:
                                new Glitch(player);
                                break;
                            case 12:
                                new Snitch(player);
                                break;
                            case 13:
                                new Arsonist(player);
                                break;
                            case 14:
                                new Altruist(player);
                                break;
                            case 15:
                                new Vigilante(player);
                                break;
                            case 16:
                                new Veteran(player);
                                break;
                            case 17:
                                new Amnesiac(player);
                                break;
                            case 18:
                                new Juggernaut(player);
                                break;
                            case 19:
                                new Tracker(player);
                                break;
                            case 20:
                                new Transporter(player);
                                break;
                            case 21:
                                new Medium(player);
                                break;
                            case 22:
                                new Trapper(player);
                                break;
                            case 23:
                                new Survivor(player);
                                break;
                            case 24:
                                new GuardianAngel(player);
                                break;
                            case 25:
                                new Mystic(player);
                                break;
                            case 26:
                                new Plaguebearer(player);
                                break;
                            case 27:
                                new Werewolf(player);
                                break;
                            case 28:
                                new Detective(player);
                                break;
                            case 29:
                                new Miner(player);
                                break;
                            case 30:
                                new Swooper(player);
                                break;
                            case 31:
                                new Morphling(player);
                                break;
                            case 32:
                                new Janitor(player);
                                break;
                            case 33:
                                new Undertaker(player);
                                break;
                            case 34:
                                new Grenadier(player);
                                break;
                            case 36:
                                new Blackmailer(player);
                                break;
                            case 37:
                                new Escapist(player);
                                break;
                            case 38:
                                new Crewmate(player);
                                break;
                            case 39:
                                new Impostor(player);
                                break;
                            case 40:
                                new Imitator(player);
                                break;
                            case 41:
                                new Bomber(player);
                                break;
                            case 100:
                                new Necromancer(player);
                                break;
                            case 101:
                                new Whisperer(player);
                                break;
                            case 102:
                                new CultistSeer(player);
                                break;
                            case 103:
                                new CultistMystic(player);
                                break;
                            case 104:
                                new Chameleon(player);
                                break;
                        }
                        break;
                    case CustomRPC.SetModifier:
                        var player2 = Utils.PlayerById(reader.ReadByte());
                        var modifierID = reader.ReadInt32();
                        switch (modifierID)
                        {
                            case 1:
                                new Torch(player2);
                                break;
                            case 2:
                                new Diseased(player2);
                                break;
                            case 3:
                                new Flash(player2);
                                break;
                            case 4:
                                new Tiebreaker(player2);
                                break;
                            case 5:
                                new Giant(player2);
                                break;
                            case 6:
                                new ButtonBarry(player2);
                                break;
                            case 7:
                                new Bait(player2);
                                break;
                            case 8:
                                new Sleuth(player2);
                                break;
                            case 9:
                                new Blind(player2);
                                break;
                            case 10:
                                new Radar(player2);
                                break;
                            case 11:
                                new Disperser(player2);
                                break;
                            case 12:
                                new Multitasker(player2);
                                break;
                            case 13:
                                new DoubleShot(player2);
                                break;
                            case 14:
                                new Underdog(player2);
                                break;
                        }
                        break;

                    case CustomRPC.LoveWin:
                        var winnerlover = Utils.PlayerById(reader.ReadByte());
                        Modifier.GetModifier<Lover>(winnerlover).Win();
                        break;

                    case CustomRPC.JesterLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Jester)
                                ((Jester) role).Loses();
                        break;

                    case CustomRPC.PhantomLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Phantom)
                                ((Phantom) role).Loses();
                        break;


                    case CustomRPC.GlitchLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Glitch)
                                ((Glitch) role).Loses();
                        break;


                    case CustomRPC.JuggernautLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Juggernaut)
                                ((Juggernaut)role).Loses();
                        break;

                    case CustomRPC.AmnesiacLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Amnesiac)
                                ((Amnesiac)role).Loses();
                        break;

                    case CustomRPC.ExecutionerLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Executioner)
                                ((Executioner) role).Loses();
                        break;

                    case CustomRPC.NobodyWins:
                        Role.NobodyWinsFunc();
                        break;

                    case CustomRPC.SurvivorOnlyWin:
                        Role.SurvOnlyWin();
                        break;

                    case CustomRPC.SetCouple:
                        var id = reader.ReadByte();
                        var id2 = reader.ReadByte();
                        var lover1 = Utils.PlayerById(id);
                        var lover2 = Utils.PlayerById(id2);

                        var modifierLover1 = new Lover(lover1);
                        var modifierLover2 = new Lover(lover2);

                        modifierLover1.OtherLover = modifierLover2;
                        modifierLover2.OtherLover = modifierLover1;

                        break;

                    case CustomRPC.Start:
                        Utils.ShowDeadBodies = false;
                        Murder.KilledPlayers.Clear();
                        Role.NobodyWins = false;
                        Role.SurvOnlyWins = false;
                        ExileControllerPatch.lastExiled = null;
                        PatchKillTimer.GameStarted = false;
                        StartImitate.ImitatingPlayer = null;
                        KillButtonTarget.DontRevive = byte.MaxValue;
                        ReviveHudManagerUpdate.DontRevive = byte.MaxValue;
                        ExilePatch.AssassinatedPlayers.Clear();
                        break;

                    case CustomRPC.JanitorClean:
                        readByte1 = reader.ReadByte();
                        var janitorPlayer = Utils.PlayerById(readByte1);
                        var janitorRole = Role.GetRole<Janitor>(janitorPlayer);
                        readByte = reader.ReadByte();
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in deadBodies)
                            if (body.ParentId == readByte)
                                Coroutines.Start(Coroutine.CleanCoroutine(body, janitorRole));

                        break;
                    case CustomRPC.EngineerFix:
                        var engineer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Engineer>(engineer).UsesLeft -= 1;
                        break;

                    case CustomRPC.FixLights:
                        var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        break;

                    case CustomRPC.SetExtraVotes:

                        var mayor = Utils.PlayerById(reader.ReadByte());
                        var mayorRole = Role.GetRole<Mayor>(mayor);
                        mayorRole.ExtraVotes = reader.ReadBytesAndSize().ToList();
                        if (!mayor.Is(RoleEnum.Mayor)) mayorRole.VoteBank -= mayorRole.ExtraVotes.Count;

                        break;

                    case CustomRPC.SetSwaps:
                        readSByte = reader.ReadSByte();
                        SwapVotes.Swap1 =
                            MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte);
                        readSByte2 = reader.ReadSByte();
                        SwapVotes.Swap2 =
                            MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte2);
                        PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Bytes received - " + readSByte + " - " +
                                                                          readSByte2);
                        break;

                    case CustomRPC.Imitate:
                        var imitator = Utils.PlayerById(reader.ReadByte());
                        var imitatorRole = Role.GetRole<Imitator>(imitator);
                        var imitateTarget = Utils.PlayerById(reader.ReadByte());
                        imitatorRole.ImitatePlayer = imitateTarget;
                        break;
                    case CustomRPC.StartImitate:
                        var imitator2 = Utils.PlayerById(reader.ReadByte());
                        if (imitator2.Is(RoleEnum.Traitor)) break;
                        var imitatorRole2 = Role.GetRole<Imitator>(imitator2);
                        StartImitate.Imitate(imitatorRole2);
                        break;
                    case CustomRPC.Remember:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var amnesiac = Utils.PlayerById(readByte1);
                        var other = Utils.PlayerById(readByte2);
                        PerformKillButton.Remember(Role.GetRole<Amnesiac>(amnesiac), other);
                        break;
                    case CustomRPC.Protect:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();

                        var medic = Utils.PlayerById(readByte1);
                        var shield = Utils.PlayerById(readByte2);
                        Role.GetRole<Medic>(medic).ShieldedPlayer = shield;
                        Role.GetRole<Medic>(medic).UsedAbility = true;
                        break;
                    case CustomRPC.AttemptSound:
                        var medicId = reader.ReadByte();
                        readByte = reader.ReadByte();
                        StopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
                        break;
                    case CustomRPC.BypassKill:
                        var killer = Utils.PlayerById(reader.ReadByte());
                        var target = Utils.PlayerById(reader.ReadByte());

                        Utils.MurderPlayer(killer, target, true);
                        break;
                    case CustomRPC.BypassMultiKill:
                        var killer2 = Utils.PlayerById(reader.ReadByte());
                        var target2 = Utils.PlayerById(reader.ReadByte());

                        Utils.MurderPlayer(killer2, target2, false);
                        break;
                    case CustomRPC.AssassinKill:
                        var toDie = Utils.PlayerById(reader.ReadByte());
                        var assassin = Utils.PlayerById(reader.ReadByte());
                        AssassinKill.MurderPlayer(toDie);
                        AssassinKill.AssassinKillCount(toDie, assassin);
                        break;
                    case CustomRPC.VigilanteKill:
                        var toDie2 = Utils.PlayerById(reader.ReadByte());
                        var vigi = Utils.PlayerById(reader.ReadByte());
                        VigilanteKill.MurderPlayer(toDie2);
                        VigilanteKill.VigiKillCount(toDie2, vigi);
                        break;
                    case CustomRPC.SetMimic:
                        var glitchPlayer = Utils.PlayerById(reader.ReadByte());
                        var mimicPlayer = Utils.PlayerById(reader.ReadByte());
                        var glitchRole = Role.GetRole<Glitch>(glitchPlayer);
                        glitchRole.MimicTarget = mimicPlayer;
                        glitchRole.IsUsingMimic = true;
                        Utils.Morph(glitchPlayer, mimicPlayer);
                        break;
                    case CustomRPC.RpcResetAnim:
                        var animPlayer = Utils.PlayerById(reader.ReadByte());
                        var theGlitchRole = Role.GetRole<Glitch>(animPlayer);
                        theGlitchRole.MimicTarget = null;
                        theGlitchRole.IsUsingMimic = false;
                        Utils.Unmorph(theGlitchRole.Player);
                        break;
                    case CustomRPC.GlitchWin:
                        var theGlitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                        ((Glitch) theGlitch)?.Wins();
                        break;
                    case CustomRPC.JuggernautWin:
                        var juggernaut = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Juggernaut);
                        ((Juggernaut)juggernaut)?.Wins();
                        break;
                    case CustomRPC.SetHacked:
                        var hackPlayer = Utils.PlayerById(reader.ReadByte());
                        if (hackPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            var glitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                            ((Glitch) glitch)?.SetHacked(hackPlayer);
                        }

                        break;
                    case CustomRPC.Morph:
                        var morphling = Utils.PlayerById(reader.ReadByte());
                        var morphTarget = Utils.PlayerById(reader.ReadByte());
                        var morphRole = Role.GetRole<Morphling>(morphling);
                        morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                        morphRole.MorphedPlayer = morphTarget;
                        break;
                    case CustomRPC.SetTarget:
                        var exe = Utils.PlayerById(reader.ReadByte());
                        var exeTarget = Utils.PlayerById(reader.ReadByte());
                        var exeRole = Role.GetRole<Executioner>(exe);
                        exeRole.target = exeTarget;
                        break;
                    case CustomRPC.SetGATarget:
                        var ga = Utils.PlayerById(reader.ReadByte());
                        var gaTarget = Utils.PlayerById(reader.ReadByte());
                        var gaRole = Role.GetRole<GuardianAngel>(ga);
                        gaRole.target = gaTarget;
                        break;
                    case CustomRPC.Blackmail:
                        var blackmailer = Role.GetRole<Blackmailer>(Utils.PlayerById(reader.ReadByte()));
                        blackmailer.Blackmailed = Utils.PlayerById(reader.ReadByte());
                        break;
                    case CustomRPC.ExecutionerToJester:
                        TargetColor.ExeToJes(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.GAToSurv:
                        GATargetColor.GAToSurv(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Mine:
                        var ventId = reader.ReadInt32();
                        var miner = Utils.PlayerById(reader.ReadByte());
                        var minerRole = Role.GetRole<Miner>(miner);
                        var pos = reader.ReadVector2();
                        var zAxis = reader.ReadSingle();
                        PerformKill.SpawnVent(ventId, minerRole, pos, zAxis);
                        break;
                    case CustomRPC.Swoop:
                        var swooper = Utils.PlayerById(reader.ReadByte());
                        var swooperRole = Role.GetRole<Swooper>(swooper);
                        swooperRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                        swooperRole.Swoop();
                        break;
                    case CustomRPC.ChameleonSwoop:
                        var chameleon = Utils.PlayerById(reader.ReadByte());
                        var chameleonRole = Role.GetRole<Chameleon>(chameleon);
                        chameleonRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                        chameleonRole.Swoop();
                        break;
                    case CustomRPC.Alert:
                        var veteran = Utils.PlayerById(reader.ReadByte());
                        var veteranRole = Role.GetRole<Veteran>(veteran);
                        veteranRole.TimeRemaining = CustomGameOptions.AlertDuration;
                        veteranRole.Alert();
                        break;
                    case CustomRPC.Vest:
                        var surv = Utils.PlayerById(reader.ReadByte());
                        var survRole = Role.GetRole<Survivor>(surv);
                        survRole.TimeRemaining = CustomGameOptions.VestDuration;
                        survRole.Vest();
                        break;
                    case CustomRPC.GAProtect:
                        var ga2 = Utils.PlayerById(reader.ReadByte());
                        var ga2Role = Role.GetRole<GuardianAngel>(ga2);
                        ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                        ga2Role.Protect();
                        break;
                    case CustomRPC.Transport:
                        Coroutines.Start(Transporter.TransportPlayers(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean()));
                        break;
                    case CustomRPC.SetDelayRoles:
                        ExilePatch.PhantomOn = reader.ReadBoolean();
                        ExilePatch.HaunterOn = reader.ReadBoolean();
                        ExilePatch.TraitorOn = reader.ReadBoolean();
                        ExilePatch.WillBePhantom = null;
                        ExilePatch.WillBeHaunter = null;
                        ExilePatch.WillBeTraitor = null;
                        ExilePatch.TraitorCanSpawn = false;
                        break;
                    case CustomRPC.SetUntransportable:
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                        {
                            Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                        }
                        break;
                    case CustomRPC.Mediate:
                        var mediatedPlayer = Utils.PlayerById(reader.ReadByte());
                        var medium = Role.GetRole<Medium>(Utils.PlayerById(reader.ReadByte()));
                        if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer.PlayerId) break;
                        medium.AddMediatePlayer(mediatedPlayer.PlayerId);
                        break;
                    case CustomRPC.FlashGrenade:
                        var grenadier = Utils.PlayerById(reader.ReadByte());
                        var grenadierRole = Role.GetRole<Grenadier>(grenadier);
                        grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                        grenadierRole.Flash();
                        break;
                    case CustomRPC.ArsonistWin:
                        var theArsonistTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Arsonist);
                        ((Arsonist) theArsonistTheRole)?.Wins();
                        break;
                    case CustomRPC.ArsonistLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Arsonist)
                                ((Arsonist) role).Loses();
                        break;
                    case CustomRPC.WerewolfWin:
                        var theWerewolfTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Werewolf);
                        ((Werewolf)theWerewolfTheRole)?.Wins();
                        break;
                    case CustomRPC.WerewolfLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Werewolf)
                                ((Werewolf)role).Loses();
                        break;
                    case CustomRPC.SurvivorImpWin:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Survivor && !role.Player.Data.IsDead && !role.Player.Data.Disconnected)
                            {
                                ((Survivor)role).AliveImpWin();
                            }
                        break;
                    case CustomRPC.SurvivorCrewWin:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Survivor && (role.Player.Data.IsDead || role.Player.Data.Disconnected))
                            {
                                ((Survivor)role).DeadCrewWin();
                            }
                        break;
                    case CustomRPC.GAImpWin:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.GuardianAngel && ((GuardianAngel)role).target.Is(Faction.Impostors))
                            {
                                ((GuardianAngel)role).ImpTargetWin();
                            }
                        break;
                    case CustomRPC.GAImpLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.GuardianAngel && ((GuardianAngel)role).target.Is(Faction.Impostors))
                            {
                                ((GuardianAngel)role).ImpTargetLose();
                            }
                        break;
                    case CustomRPC.PlaguebearerWin:
                        var thePlaguebearerTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Plaguebearer);
                        ((Plaguebearer)thePlaguebearerTheRole)?.Wins();
                        break;
                    case CustomRPC.PlaguebearerLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Plaguebearer)
                                ((Plaguebearer)role).Loses();
                        break;
                    case CustomRPC.Infect:
                        Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).InfectedPlayers.Add(reader.ReadByte());
                        break;
                    case CustomRPC.TurnPestilence:
                        Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).TurnPestilence();
                        break;
                    case CustomRPC.PestilenceWin:
                        var thePestilenceTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Pestilence);
                        ((Pestilence)thePestilenceTheRole)?.Wins();
                        break;
                    case CustomRPC.PestilenceLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Pestilence)
                                ((Pestilence)role).Loses();
                        break;
                    case CustomRPC.SyncCustomSettings:
                        Rpc.ReceiveRpc(reader);
                        break;
                    case CustomRPC.AltruistRevive:
                        readByte1 = reader.ReadByte();
                        var altruistPlayer = Utils.PlayerById(readByte1);
                        var altruistRole = Role.GetRole<Altruist>(altruistPlayer);
                        readByte = reader.ReadByte();
                        var theDeadBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in theDeadBodies)
                            if (body.ParentId == readByte)
                            {
                                if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                    Coroutines.Start(Utils.FlashCoroutine(altruistRole.Color,
                                        CustomGameOptions.ReviveDuration, 0.5f));

                                Coroutines.Start(
                                    global::TownOfUs.CrewmateRoles.AltruistMod.Coroutine.AltruistRevive(body,
                                        altruistRole));
                            }

                        break;
                    case CustomRPC.FixAnimation:
                        var player3 = Utils.PlayerById(reader.ReadByte());
                        player3.MyPhysics.ResetMoveState();
                        player3.Collider.enabled = true;
                        player3.moveable = true;
                        player3.NetTransform.enabled = true;
                        break;
                    case CustomRPC.BarryButton:
                        var buttonBarry = Utils.PlayerById(reader.ReadByte());

                        if (AmongUsClient.Instance.AmHost)
                        {
                            MeetingRoomManager.Instance.reporter = buttonBarry;
                            MeetingRoomManager.Instance.target = null;
                            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance
                                .Cast<IDisconnectHandler>());
                            if (GameManager.Instance.CheckTaskCompletion()) return;

                            DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(buttonBarry);
                            buttonBarry.RpcStartMeeting(null);
                        }
                        break;
                    case CustomRPC.Disperse:
                        byte teleports = reader.ReadByte();
                        Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>();
                        for (int i = 0; i < teleports; i++)
                        {
                            byte playerId = reader.ReadByte();
                            Vector2 location = reader.ReadVector2();
                            coordinates.Add(playerId, location);
                        }
                        Disperser.DispersePlayersToCoordinates(coordinates);
                        break;
                    case CustomRPC.BaitReport:
                        var baitKiller = Utils.PlayerById(reader.ReadByte());
                        var bait = Utils.PlayerById(reader.ReadByte());
                        baitKiller.ReportDeadBody(bait.Data);
                        break;
                    case CustomRPC.CheckMurder:
                        var murderKiller = Utils.PlayerById(reader.ReadByte());
                        var murderTarget = Utils.PlayerById(reader.ReadByte());
                        murderKiller.CheckMurder(murderTarget);
                        break;
                    case CustomRPC.Drag:
                        readByte1 = reader.ReadByte();
                        var dienerPlayer = Utils.PlayerById(readByte1);
                        var dienerRole = Role.GetRole<Undertaker>(dienerPlayer);
                        readByte = reader.ReadByte();
                        var dienerBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in dienerBodies)
                            if (body.ParentId == readByte)
                                dienerRole.CurrentlyDragging = body;

                        break;
                    case CustomRPC.Drop:
                        readByte1 = reader.ReadByte();
                        var v2 = reader.ReadVector2();
                        var v2z = reader.ReadSingle();
                        var dienerPlayer2 = Utils.PlayerById(readByte1);
                        var dienerRole2 = Role.GetRole<Undertaker>(dienerPlayer2);
                        var body2 = dienerRole2.CurrentlyDragging;
                        dienerRole2.CurrentlyDragging = null;

                        body2.transform.position = new Vector3(v2.x, v2.y, v2z);

                        break;
                    case CustomRPC.SetAssassin:
                        new Assassin(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetPhantom:
                        readByte = reader.ReadByte();
                        ExilePatch.WillBePhantom = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.CatchPhantom:
                        var phantomPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Phantom>(phantomPlayer).Caught = true;
                        if (PlayerControl.LocalPlayer == phantomPlayer) HudManager.Instance.AbilityButton.gameObject.SetActive(true);
                        break;
                    case CustomRPC.PhantomWin:
                        Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte())).CompletedTasks = true;
                        break;
                    case CustomRPC.SetHaunter:
                        readByte = reader.ReadByte();
                        ExilePatch.WillBeHaunter = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.CatchHaunter:
                        var haunterPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Haunter>(haunterPlayer).Caught = true;
                        if (PlayerControl.LocalPlayer == haunterPlayer) HudManager.Instance.AbilityButton.gameObject.SetActive(true);
                        break;
                    case CustomRPC.HaunterFinished:
                        HighlightImpostors.UpdateMeeting(MeetingHud.Instance);
                        break;
                    case CustomRPC.SetTraitor:
                        readByte = reader.ReadByte();
                        ExilePatch.WillBeTraitor = Utils.PlayerById(readByte);
                        var traitor = ExilePatch.WillBeTraitor;
                        if (traitor == StartImitate.ImitatingPlayer) StartImitate.ImitatingPlayer = null;
                        var oldRole = Role.GetRole(traitor);
                        var killsList3 = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                        Role.RoleDictionary.Remove(traitor.PlayerId);
                        var traitorRole = new Traitor(traitor);
                        traitorRole.formerRole = oldRole.RoleType;
                        traitorRole.CorrectKills = killsList3.CorrectKills;
                        traitorRole.IncorrectKills = killsList3.IncorrectKills;
                        traitorRole.CorrectAssassinKills = killsList3.CorrectAssassinKills;
                        traitorRole.IncorrectAssassinKills = killsList3.IncorrectAssassinKills;
                        traitorRole.RegenTask();
                        ExilePatch.SpawnTraitor(traitor);
                        break;
                    case CustomRPC.Escape:
                        var escapist = Utils.PlayerById(reader.ReadByte());
                        var escapistRole = Role.GetRole<Escapist>(escapist);
                        var escapePos = reader.ReadVector2();
                        escapistRole.EscapePoint = escapePos;
                        Escapist.Escape(escapist);
                        break;
                    case CustomRPC.Revive:
                        var necromancer = Utils.PlayerById(reader.ReadByte());
                        var necromancerRole = Role.GetRole<Necromancer>(necromancer);
                        var revived = reader.ReadByte();
                        var theDeadBodies2 = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in theDeadBodies2)
                            if (body.ParentId == revived)
                            {
                                PerformRevive.Revive(body, necromancerRole);
                            }
                        break;
                    case CustomRPC.Convert:
                        var convertedPlayer = Utils.PlayerById(reader.ReadByte());
                        Utils.Convert(convertedPlayer);
                        break;
                    case CustomRPC.AddMayorVoteBank:
                        Role.GetRole<Mayor>(Utils.PlayerById(reader.ReadByte())).VoteBank += reader.ReadInt32();
                        break;
                    case CustomRPC.RemoveAllBodies:
                        var buggedBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in buggedBodies)
                            body.gameObject.Destroy();
                        break;
                    case CustomRPC.SubmergedFixOxygen:
                        Patches.SubmergedCompatibility.RepairOxygen();
                        break;
                    case CustomRPC.SetPos:
                        var setplayer = Utils.PlayerById(reader.ReadByte());
                        setplayer.transform.position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        break;
                    case CustomRPC.SetSettings:
                        readByte = reader.ReadByte();
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId = readByte == byte.MaxValue ? (byte)0 : readByte;
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                        if (CustomGameOptions.AutoAdjustSettings) RandomMap.AdjustSettings(readByte);
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        public static class RpcSetRole
        {
            public static void Postfix()
            {
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage("RPC SET ROLE");
                var infected = GameData.Instance.AllPlayers.ToArray().Where(o => o.IsImpostor());

                Utils.ShowDeadBodies = false;
                Role.NobodyWins = false;
                Role.SurvOnlyWins = false;
                ExileControllerPatch.lastExiled = null;
                PatchKillTimer.GameStarted = false;
                StartImitate.ImitatingPlayer = null;
                ExilePatch.AssassinatedPlayers.Clear();
                CrewmateRoles.Clear();
                NeutralNonKillingRoles.Clear();
                NeutralKillingRoles.Clear();
                ImpostorRoles.Clear();
                CrewmateModifiers.Clear();
                GlobalModifiers.Clear();
                ImpostorModifiers.Clear();
                ButtonModifiers.Clear();
                AssassinModifiers.Clear();
                AssassinAbility.Clear();

                Murder.KilledPlayers.Clear();
                KillButtonTarget.DontRevive = byte.MaxValue;
                ReviveHudManagerUpdate.DontRevive = byte.MaxValue;

                var startWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Start, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(startWriter);

                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;

                if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny)
                {
                    ExilePatch.PhantomOn = Check(CustomGameOptions.PhantomOn);
                    ExilePatch.HaunterOn = Check(CustomGameOptions.HaunterOn);
                    ExilePatch.TraitorOn = Check(CustomGameOptions.TraitorOn);

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetDelayRoles, SendOption.Reliable, -1);
                    writer.Write(ExilePatch.PhantomOn);
                    writer.Write(ExilePatch.HaunterOn);
                    writer.Write(ExilePatch.TraitorOn);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else
                {
                    ExilePatch.PhantomOn = false;
                    ExilePatch.HaunterOn = false;
                    ExilePatch.TraitorOn = false;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetDelayRoles, SendOption.Reliable, -1);
                    writer.Write(false);
                    writer.Write(false);
                    writer.Write(false);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                ExilePatch.WillBePhantom = null;
                ExilePatch.WillBeHaunter = null;
                ExilePatch.WillBeTraitor = null;
                ExilePatch.TraitorCanSpawn = false;

                if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny)
                {
                    #region Crewmate Roles
                    if (CustomGameOptions.MayorOn > 0)
                        CrewmateRoles.Add((typeof(Mayor), 3, CustomGameOptions.MayorOn, true));

                    if (CustomGameOptions.SheriffOn > 0)
                        CrewmateRoles.Add((typeof(Sheriff), 0, CustomGameOptions.SheriffOn, false));

                    if (CustomGameOptions.EngineerOn > 0)
                        CrewmateRoles.Add((typeof(Engineer), 2, CustomGameOptions.EngineerOn, false));

                    if (CustomGameOptions.SwapperOn > 0)
                        CrewmateRoles.Add((typeof(Swapper), 4, CustomGameOptions.SwapperOn, true));

                    if (CustomGameOptions.InvestigatorOn > 0)
                        CrewmateRoles.Add((typeof(Investigator), 5, CustomGameOptions.InvestigatorOn, false));

                    if (CustomGameOptions.MedicOn > 0)
                        CrewmateRoles.Add((typeof(Medic), 7, CustomGameOptions.MedicOn, true));

                    if (CustomGameOptions.SeerOn > 0)
                        CrewmateRoles.Add((typeof(Seer), 8, CustomGameOptions.SeerOn, false));

                    if (CustomGameOptions.SpyOn > 0)
                        CrewmateRoles.Add((typeof(Spy), 10, CustomGameOptions.SpyOn, false));

                    if (CustomGameOptions.SnitchOn > 0)
                        CrewmateRoles.Add((typeof(Snitch), 12, CustomGameOptions.SnitchOn, true));

                    if (CustomGameOptions.AltruistOn > 0)
                        CrewmateRoles.Add((typeof(Altruist), 14, CustomGameOptions.AltruistOn, true));

                    if (CustomGameOptions.VigilanteOn > 0)
                        CrewmateRoles.Add((typeof(Vigilante), 15, CustomGameOptions.VigilanteOn, false));

                    if (CustomGameOptions.VeteranOn > 0)
                        CrewmateRoles.Add((typeof(Veteran), 16, CustomGameOptions.VeteranOn, false));

                    if (CustomGameOptions.TrackerOn > 0)
                        CrewmateRoles.Add((typeof(Tracker), 19, CustomGameOptions.TrackerOn, false));

                    if (CustomGameOptions.TransporterOn > 0)
                        CrewmateRoles.Add((typeof(Transporter), 20, CustomGameOptions.TransporterOn, false));

                    if (CustomGameOptions.MediumOn > 0)
                        CrewmateRoles.Add((typeof(Medium), 21, CustomGameOptions.MediumOn, false));

                    if (CustomGameOptions.MysticOn > 0)
                        CrewmateRoles.Add((typeof(Mystic), 25, CustomGameOptions.MysticOn, false));

                    if (CustomGameOptions.TrapperOn > 0)
                        CrewmateRoles.Add((typeof(Trapper), 22, CustomGameOptions.TrapperOn, false));

                    if (CustomGameOptions.DetectiveOn > 0)
                        CrewmateRoles.Add((typeof(Detective), 28, CustomGameOptions.DetectiveOn, false));

                    if (CustomGameOptions.ImitatorOn > 0)
                        CrewmateRoles.Add((typeof(Imitator), 40, CustomGameOptions.ImitatorOn, true));
                    #endregion
                    #region Neutral Roles
                    if (CustomGameOptions.JesterOn > 0)
                        NeutralNonKillingRoles.Add((typeof(Jester), 1, CustomGameOptions.JesterOn, false));

                    if (CustomGameOptions.AmnesiacOn > 0)
                        NeutralNonKillingRoles.Add((typeof(Amnesiac), 17, CustomGameOptions.AmnesiacOn, false));

                    if (CustomGameOptions.ExecutionerOn > 0)
                        NeutralNonKillingRoles.Add((typeof(Executioner), 9, CustomGameOptions.ExecutionerOn, false));

                    if (CustomGameOptions.SurvivorOn > 0)
                        NeutralNonKillingRoles.Add((typeof(Survivor), 23, CustomGameOptions.SurvivorOn, false));

                    if (CustomGameOptions.GuardianAngelOn > 0)
                        NeutralNonKillingRoles.Add((typeof(GuardianAngel), 24, CustomGameOptions.GuardianAngelOn, false));

                    if (CustomGameOptions.GlitchOn > 0)
                        NeutralKillingRoles.Add((typeof(Glitch), 11, CustomGameOptions.GlitchOn, true));

                    if (CustomGameOptions.ArsonistOn > 0)
                        NeutralKillingRoles.Add((typeof(Arsonist), 13, CustomGameOptions.ArsonistOn, true));

                    if (CustomGameOptions.PlaguebearerOn > 0)
                        NeutralKillingRoles.Add((typeof(Plaguebearer), 26, CustomGameOptions.PlaguebearerOn, true));

                    if (CustomGameOptions.WerewolfOn > 0)
                        NeutralKillingRoles.Add((typeof(Werewolf), 27, CustomGameOptions.WerewolfOn, true));

                    if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.HiddenRoles)
                        NeutralKillingRoles.Add((typeof(Juggernaut), 18, 10, true));
                    #endregion
                    #region Impostor Roles
                    if (CustomGameOptions.UndertakerOn > 0)
                        ImpostorRoles.Add((typeof(Undertaker), 33, CustomGameOptions.UndertakerOn, true));

                    if (CustomGameOptions.MorphlingOn > 0)
                        ImpostorRoles.Add((typeof(Morphling), 31, CustomGameOptions.MorphlingOn, false));

                    if (CustomGameOptions.BlackmailerOn > 0)
                        ImpostorRoles.Add((typeof(Blackmailer), 36, CustomGameOptions.BlackmailerOn, true));

                    if (CustomGameOptions.MinerOn > 0)
                        ImpostorRoles.Add((typeof(Miner), 29, CustomGameOptions.MinerOn, true));

                    if (CustomGameOptions.SwooperOn > 0)
                        ImpostorRoles.Add((typeof(Swooper), 30, CustomGameOptions.SwooperOn, false));

                    if (CustomGameOptions.JanitorOn > 0)
                        ImpostorRoles.Add((typeof(Janitor), 32, CustomGameOptions.JanitorOn, false));

                    if (CustomGameOptions.GrenadierOn > 0)
                        ImpostorRoles.Add((typeof(Grenadier), 34, CustomGameOptions.GrenadierOn, true));

                    if (CustomGameOptions.EscapistOn > 0)
                        ImpostorRoles.Add((typeof(Escapist), 37, CustomGameOptions.EscapistOn, false));

                    if (CustomGameOptions.BomberOn > 0)
                        ImpostorRoles.Add((typeof(Bomber), 41, CustomGameOptions.BomberOn, true));
                    #endregion
                    #region Crewmate Modifiers
                    if (Check(CustomGameOptions.TorchOn))
                        CrewmateModifiers.Add((typeof(Torch), 1, CustomGameOptions.TorchOn));

                    if (Check(CustomGameOptions.DiseasedOn))
                        CrewmateModifiers.Add((typeof(Diseased), 2, CustomGameOptions.DiseasedOn));

                    if (Check(CustomGameOptions.BaitOn))
                        CrewmateModifiers.Add((typeof(Bait), 7, CustomGameOptions.BaitOn));

                    if (Check(CustomGameOptions.BlindOn))
                        CrewmateModifiers.Add((typeof(Blind), 9, CustomGameOptions.BlindOn));

                    if (Check(CustomGameOptions.MultitaskerOn))
                        CrewmateModifiers.Add((typeof(Multitasker), 12, CustomGameOptions.MultitaskerOn));
                    #endregion
                    #region Global Modifiers
                    if (Check(CustomGameOptions.TiebreakerOn))
                        GlobalModifiers.Add((typeof(Tiebreaker), 4, CustomGameOptions.TiebreakerOn));

                    if (Check(CustomGameOptions.FlashOn))
                        GlobalModifiers.Add((typeof(Flash), 3, CustomGameOptions.FlashOn));

                    if (Check(CustomGameOptions.GiantOn))
                        GlobalModifiers.Add((typeof(Giant), 5, CustomGameOptions.GiantOn));

                    if (Check(CustomGameOptions.ButtonBarryOn))
                        ButtonModifiers.Add((typeof(ButtonBarry), 6, CustomGameOptions.ButtonBarryOn));

                    if (Check(CustomGameOptions.LoversOn))
                        GlobalModifiers.Add((typeof(Lover), 0, CustomGameOptions.LoversOn));

                    if (Check(CustomGameOptions.SleuthOn))
                        GlobalModifiers.Add((typeof(Sleuth), 8, CustomGameOptions.SleuthOn));

                    if (Check(CustomGameOptions.RadarOn))
                        GlobalModifiers.Add((typeof(Radar), 10, CustomGameOptions.RadarOn));
                    #endregion
                    #region Impostor Modifiers
                    if (Check(CustomGameOptions.DisperserOn) && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 4 && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 5)
                        ImpostorModifiers.Add((typeof(Disperser), 11, CustomGameOptions.DisperserOn));

                    if (Check(CustomGameOptions.DoubleShotOn))
                        AssassinModifiers.Add((typeof(DoubleShot), 13, CustomGameOptions.DoubleShotOn));

                    if (CustomGameOptions.UnderdogOn > 0)
                        ImpostorModifiers.Add((typeof(Underdog), 14, CustomGameOptions.UnderdogOn));
                    #endregion
                    #region Assassin Ability
                    AssassinAbility.Add((typeof(Assassin), CustomRPC.SetAssassin, 100));
                    #endregion
                }

                if (CustomGameOptions.GameMode == GameMode.KillingOnly) GenEachRoleKilling(infected.ToList());
                else if (CustomGameOptions.GameMode == GameMode.Cultist) GenEachRoleCultist(infected.ToList());
                else GenEachRole(infected.ToList());
            }
        }
    }
}

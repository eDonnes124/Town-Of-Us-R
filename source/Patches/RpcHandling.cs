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
using TownOfUs.NeutralRoles.DoomsayerMod;
using TownOfUs.CultistRoles.NecromancerMod;
using TownOfUs.CustomOption;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using TownOfUs.ImpostorRoles.MinerMod;
using TownOfUs.CrewmateRoles.HaunterMod;
using TownOfUs.NeutralRoles.PhantomMod;
using TownOfUs.ImpostorRoles.TraitorMod;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using Coroutine = TownOfUs.ImpostorRoles.JanitorMod.Coroutine;
using Object = UnityEngine.Object;
using PerformKillButton = TownOfUs.NeutralRoles.AmnesiacMod.PerformKillButton;
using Random = UnityEngine.Random; //using Il2CppSystem;
using TownOfUs.Patches;
using AmongUs.GameOptions;
using TownOfUs.NeutralRoles.VampireMod;
using TownOfUs.CrewmateRoles.MayorMod;
using System.Reflection;

namespace TownOfUs
{
    public static class RpcHandling
    {
        private static readonly List<(Type, int, bool)> CrewmateRoles = new();
        private static readonly List<(Type, int, bool)> NeutralBenignRoles = new();
        private static readonly List<(Type, int, bool)> NeutralEvilRoles = new();
        private static readonly List<(Type, int, bool)> NeutralKillingRoles = new();
        private static readonly List<(Type, int, bool)> ImpostorRoles = new();
        private static readonly List<(Type, int)> CrewmateModifiers = new();
        private static readonly List<(Type, int)> GlobalModifiers = new();
        private static readonly List<(Type, int)> ImpostorModifiers = new();
        private static readonly List<(Type, int)> ButtonModifiers = new();
        private static readonly List<(Type, int)> AssassinModifiers = new();
        private static readonly List<(Type, CustomRPC, int)> AssassinAbility = new();
        private static bool PhantomOn;
        private static bool HaunterOn;
        private static bool TraitorOn;

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
            return num <= 10 * CustomGameOptions.MaxNeutralKillingRoles;
        }
        private static void PickRoleCount(int roleCount, int min, int max)
        {
            if (min > max) min = max;
            roleCount = Random.RandomRangeInt(min, max + 1);
        }

        private static void SortRoles(List<(Type, int, bool)> roles, int numRoles)
        {
            roles.Shuffle();
            if (roles.Count < numRoles) numRoles = roles.Count;
            roles.Sort((a, b) =>
            {
                var a_ = a.Item2 == 100 ? 0 : 100;
                var b_ = b.Item2 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });
            var certainRoles = 0;
            var odds = 0;
            foreach (var role in roles)
                if (role.Item2 == 100) certainRoles += 1;
                else odds += role.Item2;
            while (certainRoles < numRoles)
            {
                var num = certainRoles;
                var random = Random.RandomRangeInt(0, odds);
                var rolePicked = false;
                while (num < roles.Count && rolePicked == false)
                {
                    random -= roles[num].Item2;
                    if (random < 0)
                    {
                        odds -= roles[num].Item2;
                        var role = roles[num];
                        roles.Remove(role);
                        roles.Insert(0, role);
                        certainRoles += 1;
                        rolePicked = true;
                    }
                    num += 1;
                }
            }
            while (roles.Count > numRoles) roles.RemoveAt(roles.Count - 1);
        }

        private static void SortModifiers(List<(Type, int)> roles, int max)
        {
            roles.Shuffle();
            roles.Sort((a, b) =>
            {
                var a_ = a.Item2 == 100 ? 0 : 100;
                var b_ = b.Item2 == 100 ? 0 : 100;
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
                var benign = CustomGameOptions.MaxNeutralBenignRoles;
                if (NeutralBenignRoles.Count < benign) benign = NeutralBenignRoles.Count;
                PickRoleCount(benign, CustomGameOptions.MinNeutralBenignRoles, benign);
                var evil = CustomGameOptions.MaxNeutralEvilRoles;
                if (NeutralEvilRoles.Count < evil) evil = NeutralEvilRoles.Count;
                PickRoleCount(evil, CustomGameOptions.MinNeutralEvilRoles, evil);
                var killing = CustomGameOptions.MaxNeutralKillingRoles;
                if (NeutralKillingRoles.Count < killing) killing = NeutralKillingRoles.Count;
                PickRoleCount(killing, CustomGameOptions.MinNeutralKillingRoles, killing);

                var canSubtractBenign = benign > CustomGameOptions.MinNeutralBenignRoles;
                var canSubtractEvil = evil > CustomGameOptions.MinNeutralEvilRoles;
                var canSubtractKilling = killing > CustomGameOptions.MinNeutralKillingRoles;

                while (crewmates.Count <= benign + evil + killing)
                {
                    if ((canSubtractBenign && canSubtractEvil && canSubtractKilling) ||
                        (!canSubtractBenign && !canSubtractEvil && !canSubtractKilling))
                    {
                        var num = Random.RandomRangeInt(0, 3);
                        if (num == 0 && benign > 0)
                        {
                            benign -= 1;
                            canSubtractBenign = benign > CustomGameOptions.MinNeutralBenignRoles;
                        }
                        else if (num <= 1 && evil > 0)
                        {
                            evil -= 1;
                            canSubtractEvil = evil > CustomGameOptions.MinNeutralEvilRoles;
                        }
                        else if (killing > 0)
                        {
                            killing -= 1;
                            canSubtractKilling = killing > CustomGameOptions.MinNeutralKillingRoles;
                        }
                        else if (benign > 0)
                        {
                            benign -= 1;
                            canSubtractBenign = benign > CustomGameOptions.MinNeutralBenignRoles;
                        }
                        else
                        {
                            evil -= 1;
                            canSubtractEvil = evil > CustomGameOptions.MinNeutralEvilRoles;
                        }
                    }
                    else if (canSubtractBenign && !canSubtractEvil && !canSubtractKilling)
                    {
                        benign -= 1;
                        canSubtractBenign = benign > CustomGameOptions.MinNeutralBenignRoles;
                    }
                    else if (!canSubtractBenign && canSubtractEvil && !canSubtractKilling)
                    {
                        evil -= 1;
                        canSubtractEvil = evil > CustomGameOptions.MinNeutralEvilRoles;
                    }
                    else if (!canSubtractBenign && !canSubtractEvil && canSubtractKilling)
                    {
                        killing -= 1;
                        canSubtractKilling = killing > CustomGameOptions.MinNeutralKillingRoles;
                    }
                    else if (canSubtractBenign && canSubtractEvil && !canSubtractKilling)
                    {
                        var num = Random.RandomRangeInt(0, 2);
                        if (num == 0)
                        {
                            benign -= 1;
                            canSubtractBenign = benign > CustomGameOptions.MinNeutralBenignRoles;
                        }
                        else
                        {
                            evil -= 1;
                            canSubtractEvil = evil > CustomGameOptions.MinNeutralEvilRoles;
                        }
                    }
                    else if (canSubtractBenign && !canSubtractEvil && canSubtractKilling)
                    {
                        var num = Random.RandomRangeInt(0, 2);
                        if (num == 0)
                        {
                            benign -= 1;
                            canSubtractBenign = benign > CustomGameOptions.MinNeutralBenignRoles;
                        }
                        else
                        {
                            killing -= 1;
                            canSubtractKilling = killing > CustomGameOptions.MinNeutralKillingRoles;
                        }
                    }
                    else if (!canSubtractBenign && canSubtractEvil && canSubtractKilling)
                    {
                        var num = Random.RandomRangeInt(0, 2);
                        if (num == 0)
                        {
                            evil -= 1;
                            canSubtractEvil = evil > CustomGameOptions.MinNeutralEvilRoles;
                        }
                        else
                        {
                            killing -= 1;
                            canSubtractKilling = killing > CustomGameOptions.MinNeutralKillingRoles;
                        }
                    }
                }

                SortRoles(NeutralBenignRoles, benign);
                SortRoles(NeutralEvilRoles, evil);
                SortRoles(NeutralKillingRoles, killing);

                if (NeutralKillingRoles.Contains((typeof(Vampire), CustomGameOptions.VampireOn, true)) && CustomGameOptions.VampireHunterOn > 0)
                {
                    CrewmateRoles.Add((typeof(VampireHunter), CustomGameOptions.VampireHunterOn, true));
                }

                SortRoles(CrewmateRoles, crewmates.Count - NeutralBenignRoles.Count - NeutralEvilRoles.Count - NeutralKillingRoles.Count);
                SortRoles(ImpostorRoles, impostors.Count);
            }

            var crewAndNeutralRoles = new List<(Type, int, bool)>();
            if (CustomGameOptions.GameMode == GameMode.Classic) crewAndNeutralRoles.AddRange(CrewmateRoles);
            crewAndNeutralRoles.AddRange(NeutralBenignRoles);
            crewAndNeutralRoles.AddRange(NeutralEvilRoles);
            crewAndNeutralRoles.AddRange(NeutralKillingRoles);

            var crewRoles = new List<(Type, int, bool)>();
            var impRoles = new List<(Type, int, bool)>();

            if (CustomGameOptions.GameMode == GameMode.AllAny)
            {
                crewAndNeutralRoles.Shuffle();
                if (crewAndNeutralRoles.Count > 0)
                {
                    crewRoles.Add(crewAndNeutralRoles[0]);
                    if (crewAndNeutralRoles[0].Item3 == true) crewAndNeutralRoles.Remove(crewAndNeutralRoles[0]);
                }
                if (CrewmateRoles.Count > 0)
                {
                    CrewmateRoles.Shuffle();
                    crewRoles.Add(CrewmateRoles[0]);
                    if (CrewmateRoles[0].Item3 == true) CrewmateRoles.Remove(CrewmateRoles[0]);
                }
                else
                {
                    crewRoles.Add((typeof(Crewmate), 100, false));
                }
                crewAndNeutralRoles.AddRange(CrewmateRoles);
                while (crewRoles.Count < crewmates.Count && crewAndNeutralRoles.Count > 0)
                {
                    crewAndNeutralRoles.Shuffle();
                    crewRoles.Add(crewAndNeutralRoles[0]);
                    if (crewAndNeutralRoles[0].Item3 == true)
                    {
                        if (CrewmateRoles.Contains(crewAndNeutralRoles[0])) CrewmateRoles.Remove(crewAndNeutralRoles[0]);
                        crewAndNeutralRoles.Remove(crewAndNeutralRoles[0]);
                    }
                }
                while (impRoles.Count < impostors.Count && ImpostorRoles.Count > 0)
                {
                    ImpostorRoles.Shuffle();
                    impRoles.Add(ImpostorRoles[0]);
                    if (ImpostorRoles[0].Item3 == true) ImpostorRoles.Remove(ImpostorRoles[0]);
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
                foreach (var (type, _, unique) in crewRoles)
                {
                    Role.GenRole<Role>(type, crewmates);
                }
                foreach (var (type, _, unique) in impRoles)
                {
                    Role.GenRole<Role>(type, impostors);
                }
            }
            else
            {
                foreach (var (type, _, unique) in crewAndNeutralRoles)
                {
                    Role.GenRole<Role>(type, crewmates);
                }
                foreach (var (type, _, unique) in ImpostorRoles)
                {
                    Role.GenRole<Role>(type, impostors);
                }
            }

            foreach (var crewmate in crewmates)
                Role.GenRole<Role>(typeof(Crewmate), crewmate);

            foreach (var impostor in impostors)
                Role.GenRole<Role>(typeof(Impostor), impostor);

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

            foreach (var (type, _) in AssassinModifiers)
            {
                if (canHaveAssassinModifier.Count == 0) break;
                Role.GenModifier<Modifier>(type, canHaveAssassinModifier);
            }

            canHaveImpModifier.RemoveAll(player => player.Is(ModifierEnum.DoubleShot));

            foreach (var (type, _) in ImpostorModifiers)
            {
                if (canHaveImpModifier.Count == 0) break;
                Role.GenModifier<Modifier>(type, canHaveImpModifier);
            }

            canHaveModifier.RemoveAll(player => player.Is(ModifierEnum.Disperser) || player.Is(ModifierEnum.DoubleShot) || player.Is(ModifierEnum.Underdog));

            foreach (var (type, id) in GlobalModifiers)
            {
                if (canHaveModifier.Count == 0) break;
                if (type.FullName.Contains("Lover"))
                {
                    if (canHaveModifier.Count == 1) continue;
                        Lover.Gen(canHaveModifier);
                }
                else
                {
                    Role.GenModifier<Modifier>(type, canHaveModifier);
                }
            }

            canHaveModifier.RemoveAll(player => player.Is(RoleEnum.Glitch));

            foreach (var (type, id) in ButtonModifiers)
            {
                if (canHaveModifier.Count == 0) break;
                Role.GenModifier<Modifier>(type, canHaveModifier);
            }

            canHaveModifier.RemoveAll(player => !player.Is(Faction.Crewmates));
            canHaveModifier.Shuffle();

            while (canHaveModifier.Count > 0 && CrewmateModifiers.Count > 0)
            {
                var (type, _) = CrewmateModifiers.TakeFirst();
                Role.GenModifier<Modifier>(type, canHaveModifier.TakeFirst());
            }


            var toChooseFromCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor) && !x.Is(ModifierEnum.Lover)).ToList();
            if (TraitorOn && toChooseFromCrew.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromCrew.Count);
                var pc = toChooseFromCrew[rand];

                SetTraitor.WillBeTraitor = pc;

                Utils.Rpc(CustomRPC.SetTraitor, pc.PlayerId);
            }
            else
            {
                Utils.Rpc(CustomRPC.SetTraitor, byte.MaxValue);
            }

            if (HaunterOn && toChooseFromCrew.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromCrew.Count);
                var pc = toChooseFromCrew[rand];

                SetHaunter.WillBeHaunter = pc;

                Utils.Rpc(CustomRPC.SetHaunter, pc.PlayerId);
            }
            else
            {
                Utils.Rpc(CustomRPC.SetHaunter, byte.MaxValue);
            }

            var toChooseFromNeut = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.NeutralBenign) || x.Is(Faction.NeutralEvil) || x.Is(Faction.NeutralKilling)) && !x.Is(ModifierEnum.Lover)).ToList();
            if (PhantomOn && toChooseFromNeut.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromNeut.Count);
                var pc = toChooseFromNeut[rand];

                SetPhantom.WillBePhantom = pc;

                Utils.Rpc(CustomRPC.SetPhantom, pc.PlayerId);
            }
            else
            {
                Utils.Rpc(CustomRPC.SetPhantom, byte.MaxValue);
            }

            var exeTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ModifierEnum.Lover) && !x.Is(RoleEnum.Mayor) && !x.Is(RoleEnum.Swapper) && !x.Is(RoleEnum.Vigilante) && x != SetTraitor.WillBeTraitor).ToList();
            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                if (exeTargets.Count > 0)
                {
                    exe.target = exeTargets[Random.RandomRangeInt(0, exeTargets.Count)];
                    exeTargets.Remove(exe.target);

                    Utils.Rpc(CustomRPC.SetTarget, role.Player.PlayerId, exe.target.PlayerId);
                }
            }

            var goodGATargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ModifierEnum.Lover)).ToList();
            var evilGATargets = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.Impostors) || x.Is(Faction.NeutralKilling)) && !x.Is(ModifierEnum.Lover)).ToList();
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                if (!(goodGATargets.Count == 0 && CustomGameOptions.EvilTargetPercent == 0) ||
                    (evilGATargets.Count == 0 && CustomGameOptions.EvilTargetPercent == 100) ||
                    goodGATargets.Count == 0 && evilGATargets.Count == 0)
                {
                    if (goodGATargets.Count == 0)
                    {
                        ga.target = evilGATargets[Random.RandomRangeInt(0, evilGATargets.Count)];
                        evilGATargets.Remove(ga.target);
                    }
                    else if (evilGATargets.Count == 0 || !Check(CustomGameOptions.EvilTargetPercent))
                    {
                        ga.target = goodGATargets[Random.RandomRangeInt(0, goodGATargets.Count)];
                        goodGATargets.Remove(ga.target);
                    }
                    else
                    {
                        ga.target = evilGATargets[Random.RandomRangeInt(0, evilGATargets.Count)];
                        evilGATargets.Remove(ga.target);
                    }

                    Utils.Rpc(CustomRPC.SetGATarget, role.Player.PlayerId, ga.target.PlayerId);
                }
            }
        }
        private static void GenEachRoleKilling(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();
            impostors.Shuffle();

            ImpostorRoles.Add((typeof(Undertaker), 10, true));
            ImpostorRoles.Add((typeof(Morphling), 10, false));
            ImpostorRoles.Add((typeof(Escapist), 10, false));
            ImpostorRoles.Add((typeof(Miner), 10, true));
            ImpostorRoles.Add((typeof(Swooper), 10, false));
            ImpostorRoles.Add((typeof(Grenadier), 10, true));

            SortRoles(ImpostorRoles, impostors.Count);

            NeutralKillingRoles.Add((typeof(Glitch), 10, true));
            NeutralKillingRoles.Add((typeof(Werewolf), 10, true));
            if (CustomGameOptions.HiddenRoles)
                NeutralKillingRoles.Add((typeof(Juggernaut), 10, true));
            if (CustomGameOptions.AddArsonist)
                NeutralKillingRoles.Add((typeof(Arsonist), 10, true));
            if (CustomGameOptions.AddPlaguebearer)
                NeutralKillingRoles.Add((typeof(Plaguebearer), 10, true));

            var neutrals = 0;
            if (NeutralKillingRoles.Count < CustomGameOptions.NeutralRoles) neutrals = NeutralKillingRoles.Count;
            else neutrals = CustomGameOptions.NeutralRoles;
            var spareCrew = crewmates.Count - neutrals;
            if (spareCrew > 2) SortRoles(NeutralKillingRoles, neutrals);
            else SortRoles(NeutralKillingRoles, crewmates.Count - 3);

            var veterans = CustomGameOptions.VeteranCount;
            while (veterans > 0)
            {
                CrewmateRoles.Add((typeof(Veteran), 10, false));
                veterans -= 1;
            }
            var vigilantes = CustomGameOptions.VigilanteCount;
            while (vigilantes > 0)
            {
                CrewmateRoles.Add((typeof(Vigilante), 10, false));
                vigilantes -= 1;
            }
            if (CrewmateRoles.Count + NeutralKillingRoles.Count > crewmates.Count)
            {
                SortRoles(CrewmateRoles, crewmates.Count - NeutralKillingRoles.Count);
            }
            else if (CrewmateRoles.Count + NeutralKillingRoles.Count < crewmates.Count)
            {
                var sheriffs = crewmates.Count - NeutralKillingRoles.Count - CrewmateRoles.Count;
                while (sheriffs > 0)
                {
                    CrewmateRoles.Add((typeof(Sheriff), 10, false));
                    sheriffs -= 1;
                }
            }

            var crewAndNeutralRoles = new List<(Type, int, bool)>();
            crewAndNeutralRoles.AddRange(CrewmateRoles);
            crewAndNeutralRoles.AddRange(NeutralKillingRoles);
            crewAndNeutralRoles.Shuffle();
            ImpostorRoles.Shuffle();

            foreach (var (type, _, unique) in crewAndNeutralRoles)
            {
                Role.GenRole<Role>(type, crewmates);
            }
            foreach (var (type, _, unique) in ImpostorRoles)
            {
                Role.GenRole<Role>(type, impostors);
            }
        }
        private static void GenEachRoleCultist(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();
            impostors.Shuffle();

            var specialRoles = new List<(Type, int, bool)>();
            var crewRoles = new List<(Type, int, bool)>();
            var impRole = new List<(Type, int, bool)>();
            if (CustomGameOptions.MayorCultistOn > 0) specialRoles.Add((typeof(Mayor), CustomGameOptions.MayorCultistOn, true));
            if (CustomGameOptions.SeerCultistOn > 0) specialRoles.Add((typeof(CultistSeer), CustomGameOptions.SeerCultistOn, true));
            if (CustomGameOptions.SheriffCultistOn > 0) specialRoles.Add((typeof(Sheriff), CustomGameOptions.SheriffCultistOn, true));
            if (CustomGameOptions.SurvivorCultistOn > 0) specialRoles.Add((typeof(Survivor), CustomGameOptions.SurvivorCultistOn, true));
            if (specialRoles.Count > CustomGameOptions.SpecialRoleCount) SortRoles(specialRoles, CustomGameOptions.SpecialRoleCount);
            if (specialRoles.Count > crewmates.Count) SortRoles(specialRoles, crewmates.Count);
            if (specialRoles.Count < crewmates.Count)
            {
                var chameleons = CustomGameOptions.MaxChameleons;
                var engineers = CustomGameOptions.MaxEngineers;
                var investigators = CustomGameOptions.MaxInvestigators;
                var mystics = CustomGameOptions.MaxMystics;
                var snitches = CustomGameOptions.MaxSnitches;
                var spies = CustomGameOptions.MaxSpies;
                var transporters = CustomGameOptions.MaxTransporters;
                var vigilantes = CustomGameOptions.MaxVigilantes;
                while (chameleons > 0)
                {
                    crewRoles.Add((typeof(Chameleon), 10, false));
                    chameleons--;
                }
                while (engineers > 0)
                {
                    crewRoles.Add((typeof(Engineer), 10, false));
                    engineers--;
                }
                while (investigators > 0)
                {
                    crewRoles.Add((typeof(Investigator), 10, false));
                    investigators--;
                }
                while (mystics > 0)
                {
                    crewRoles.Add((typeof(CultistMystic), 10, false));
                    mystics--;
                }
                while (snitches > 0)
                {
                    crewRoles.Add((typeof(CultistSnitch), 10, false));
                    snitches--;
                }
                while (spies > 0)
                {
                    crewRoles.Add((typeof(Spy), 10, false));
                    spies--;
                }
                while (transporters > 0)
                {
                    crewRoles.Add((typeof(Transporter), 10, false));
                    transporters--;
                }
                while (vigilantes > 0)
                {
                    crewRoles.Add((typeof(Vigilante), 10, false));
                    vigilantes--;
                }
                SortRoles(crewRoles, crewmates.Count - specialRoles.Count);
            }
            impRole.Add((typeof(Necromancer), 10, true));
            impRole.Add((typeof(Whisperer), 10, true));
            SortRoles(impRole, 1);

            foreach (var (type, _, unique) in specialRoles)
            {
                Role.GenRole<Role>(type, crewmates);
            }
            foreach (var (type, _, unique) in crewRoles)
            {
                Role.GenRole<Role>(type, crewmates);
            }
            foreach (var (type, _, unique) in impRole)
            {
                Role.GenRole<Role>(type, impostors);
            }

            foreach (var crewmate in crewmates)
                Role.GenRole<Role>(typeof(Crewmate), crewmate);
        }


        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpc
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                Assembly asm = typeof(Role).Assembly;

                byte readByte, readByte1, readByte2;
                sbyte readSByte, readSByte2;
                switch ((CustomRPC) callId)
                {
                    case CustomRPC.SetRole:
                        var player = Utils.PlayerById(reader.ReadByte());
                        var rstring = reader.ReadString();
                        Activator.CreateInstance(asm.GetType(rstring), new object[] { player });
                        break;
                    case CustomRPC.SetModifier:
                        var player2 = Utils.PlayerById(reader.ReadByte());
                        var mstring = reader.ReadString();
                        Activator.CreateInstance(asm.GetType(mstring), new object[] { player2 });
                        break;

                    case CustomRPC.LoveWin:
                        var winnerlover = Utils.PlayerById(reader.ReadByte());
                        Modifier.GetModifier<Lover>(winnerlover).Win();
                        break;

                    case CustomRPC.NobodyWins:
                        Role.NobodyWinsFunc();
                        break;

                    case CustomRPC.SurvivorOnlyWin:
                        Role.SurvOnlyWin();
                        break;

                    case CustomRPC.VampireWin:
                        Role.VampWin();
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
                        readByte = reader.ReadByte();
                        Utils.ShowDeadBodies = false;
                        ShowRoundOneShield.FirstRoundShielded = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        ShowRoundOneShield.DiedFirst = "";
                        Murder.KilledPlayers.Clear();
                        Role.NobodyWins = false;
                        Role.SurvOnlyWins = false;
                        Role.VampireWins = false;
                        ExileControllerPatch.lastExiled = null;
                        PatchKillTimer.GameStarted = false;
                        StartImitate.ImitatingPlayer = null;
                        KillButtonTarget.DontRevive = byte.MaxValue;
                        ReviveHudManagerUpdate.DontRevive = byte.MaxValue;
                        AddHauntPatch.AssassinatedPlayers.Clear();
                        HudUpdate.Zooming = false;
                        HudUpdate.ZoomStart();
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

                    case CustomRPC.Reveal:
                        var mayor = Utils.PlayerById(reader.ReadByte());
                        var mayorRole = Role.GetRole<Mayor>(mayor);
                        mayorRole.Revealed = true;
                        AddRevealButton.RemoveAssassin(mayorRole);
                        break;

                    case CustomRPC.Prosecute:
                        var host = reader.ReadBoolean();
                        if (host && AmongUsClient.Instance.AmHost)
                        {
                            var prosecutor = Utils.PlayerById(reader.ReadByte());
                            var prosRole = Role.GetRole<Prosecutor>(prosecutor);
                            prosRole.ProsecuteThisMeeting = true;
                        }
                        else if (!host && !AmongUsClient.Instance.AmHost)
                        {
                            var prosecutor = Utils.PlayerById(reader.ReadByte());
                            var prosRole = Role.GetRole<Prosecutor>(prosecutor);
                            prosRole.ProsecuteThisMeeting = true;
                        }
                        break;

                    case CustomRPC.Bite:
                        var newVamp = Utils.PlayerById(reader.ReadByte());
                        Bite.Convert(newVamp);
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
                    case CustomRPC.DoomsayerKill:
                        var toDie3 = Utils.PlayerById(reader.ReadByte());
                        var doom = Utils.PlayerById(reader.ReadByte());
                        DoomsayerKill.DoomKillCount(toDie3, doom);
                        DoomsayerKill.MurderPlayer(toDie3);
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
                    case CustomRPC.SnitchCultistReveal:
                        var snitch = Role.GetRole<CultistSnitch>(Utils.PlayerById(reader.ReadByte()));
                        snitch.CompletedTasks = true;
                        snitch.RevealedPlayer = Utils.PlayerById(reader.ReadByte());
                        break;
                    case CustomRPC.Confess:
                        var oracle = Role.GetRole<Oracle>(Utils.PlayerById(reader.ReadByte()));
                        oracle.Confessor = Utils.PlayerById(reader.ReadByte());
                        var faction = reader.ReadInt32();
                        if (faction == 0) oracle.RevealedFaction = Faction.Crewmates;
                        else if (faction == 1) oracle.RevealedFaction = Faction.NeutralEvil;
                        else oracle.RevealedFaction = Faction.Impostors;
                        break;
                    case CustomRPC.Bless:
                        var oracle2 = Role.GetRole<Oracle>(Utils.PlayerById(reader.ReadByte()));
                        oracle2.SavedConfessor = true;
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
                        PlaceVent.SpawnVent(ventId, minerRole, pos, zAxis);
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
                    case CustomRPC.Camouflage:
                        var venerer = Utils.PlayerById(reader.ReadByte());
                        var venererRole = Role.GetRole<Venerer>(venerer);
                        venererRole.TimeRemaining = CustomGameOptions.AbilityDuration;
                        venererRole.KillsAtStartAbility = reader.ReadInt32();
                        venererRole.Ability();
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
                    case CustomRPC.WerewolfWin:
                        var theWerewolfTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Werewolf);
                        ((Werewolf)theWerewolfTheRole)?.Wins();
                        break;
                    case CustomRPC.PlaguebearerWin:
                        var thePlaguebearerTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Plaguebearer);
                        ((Plaguebearer)thePlaguebearerTheRole)?.Wins();
                        break;
                    case CustomRPC.Infect:
                        var pb = Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte()));
                        pb.SpreadInfection(Utils.PlayerById(reader.ReadByte()), Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.TurnPestilence:
                        Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).TurnPestilence();
                        break;
                    case CustomRPC.PestilenceWin:
                        var thePestilenceTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Pestilence);
                        ((Pestilence)thePestilenceTheRole)?.Wins();
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
                        SetPhantom.WillBePhantom = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.CatchPhantom:
                        var phantomPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Phantom>(phantomPlayer).Caught = true;
                        if (PlayerControl.LocalPlayer == phantomPlayer) HudManager.Instance.AbilityButton.gameObject.SetActive(true);
                        phantomPlayer.Exiled();
                        break;
                    case CustomRPC.PhantomWin:
                        Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte())).CompletedTasks = true;
                        break;
                    case CustomRPC.SetHaunter:
                        readByte = reader.ReadByte();
                        SetHaunter.WillBeHaunter = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.CatchHaunter:
                        var haunterPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Haunter>(haunterPlayer).Caught = true;
                        if (PlayerControl.LocalPlayer == haunterPlayer) HudManager.Instance.AbilityButton.gameObject.SetActive(true);
                        haunterPlayer.Exiled();
                        break;
                    case CustomRPC.SetTraitor:
                        readByte = reader.ReadByte();
                        SetTraitor.WillBeTraitor = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.TraitorSpawn:
                        var traitor = SetTraitor.WillBeTraitor;
                        if (traitor == StartImitate.ImitatingPlayer) StartImitate.ImitatingPlayer = null;
                        var oldRole = Role.GetRole(traitor);
                        var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                        Role.RoleDictionary.Remove(traitor.PlayerId);
                        var traitorRole = new Traitor(traitor);
                        traitorRole.formerRole = oldRole.RoleType;
                        traitorRole.CorrectKills = killsList.CorrectKills;
                        traitorRole.IncorrectKills = killsList.IncorrectKills;
                        traitorRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
                        traitorRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                        traitorRole.RegenTask();
                        SetTraitor.TurnImp(traitor);
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
                if (ShowRoundOneShield.DiedFirst != null && CustomGameOptions.FirstDeathShield)
                {
                    var shielded = false;
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.name == ShowRoundOneShield.DiedFirst)
                        {
                            ShowRoundOneShield.FirstRoundShielded = player;
                            shielded = true;
                        }
                    }
                    if (!shielded) ShowRoundOneShield.FirstRoundShielded = null;
                }
                else ShowRoundOneShield.FirstRoundShielded = null;
                ShowRoundOneShield.DiedFirst = "";
                Role.NobodyWins = false;
                Role.SurvOnlyWins = false;
                Role.VampireWins = false;
                ExileControllerPatch.lastExiled = null;
                PatchKillTimer.GameStarted = false;
                StartImitate.ImitatingPlayer = null;
                AddHauntPatch.AssassinatedPlayers.Clear();
                CrewmateRoles.Clear();
                NeutralBenignRoles.Clear();
                NeutralEvilRoles.Clear();
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
                HudUpdate.Zooming = false;
                HudUpdate.ZoomStart();

                if (ShowRoundOneShield.FirstRoundShielded != null)
                {
                    Utils.Rpc(CustomRPC.Start, ShowRoundOneShield.FirstRoundShielded.PlayerId);
                }
                else
                {
                    Utils.Rpc(CustomRPC.Start, byte.MaxValue);
                }

                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;

                if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny)
                {
                    PhantomOn = Check(CustomGameOptions.PhantomOn);
                    HaunterOn = Check(CustomGameOptions.HaunterOn);
                    TraitorOn = Check(CustomGameOptions.TraitorOn);
                }
                else
                {
                    PhantomOn = false;
                    HaunterOn = false;
                    TraitorOn = false;
                }

                if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny)
                {
                    #region Crewmate Roles
                    if (CustomGameOptions.MayorOn > 0)
                        CrewmateRoles.Add((typeof(Mayor), CustomGameOptions.MayorOn, true));

                    if (CustomGameOptions.SheriffOn > 0)
                        CrewmateRoles.Add((typeof(Sheriff), CustomGameOptions.SheriffOn, false));

                    if (CustomGameOptions.EngineerOn > 0)
                        CrewmateRoles.Add((typeof(Engineer), CustomGameOptions.EngineerOn, false));

                    if (CustomGameOptions.SwapperOn > 0)
                        CrewmateRoles.Add((typeof(Swapper), CustomGameOptions.SwapperOn, true));

                    if (CustomGameOptions.InvestigatorOn > 0)
                        CrewmateRoles.Add((typeof(Investigator), CustomGameOptions.InvestigatorOn, false));

                    if (CustomGameOptions.MedicOn > 0)
                        CrewmateRoles.Add((typeof(Medic), CustomGameOptions.MedicOn, true));

                    if (CustomGameOptions.SeerOn > 0)
                        CrewmateRoles.Add((typeof(Seer), CustomGameOptions.SeerOn, false));

                    if (CustomGameOptions.SpyOn > 0)
                        CrewmateRoles.Add((typeof(Spy), CustomGameOptions.SpyOn, false));

                    if (CustomGameOptions.SnitchOn > 0)
                        CrewmateRoles.Add((typeof(Snitch), CustomGameOptions.SnitchOn, true));

                    if (CustomGameOptions.AltruistOn > 0)
                        CrewmateRoles.Add((typeof(Altruist), CustomGameOptions.AltruistOn, true));

                    if (CustomGameOptions.VigilanteOn > 0)
                        CrewmateRoles.Add((typeof(Vigilante), CustomGameOptions.VigilanteOn, false));

                    if (CustomGameOptions.VeteranOn > 0)
                        CrewmateRoles.Add((typeof(Veteran), CustomGameOptions.VeteranOn, false));

                    if (CustomGameOptions.TrackerOn > 0)
                        CrewmateRoles.Add((typeof(Tracker), CustomGameOptions.TrackerOn, false));

                    if (CustomGameOptions.TransporterOn > 0)
                        CrewmateRoles.Add((typeof(Transporter), CustomGameOptions.TransporterOn, false));

                    if (CustomGameOptions.MediumOn > 0)
                        CrewmateRoles.Add((typeof(Medium), CustomGameOptions.MediumOn, false));

                    if (CustomGameOptions.MysticOn > 0)
                        CrewmateRoles.Add((typeof(Mystic), CustomGameOptions.MysticOn, false));

                    if (CustomGameOptions.TrapperOn > 0)
                        CrewmateRoles.Add((typeof(Trapper), CustomGameOptions.TrapperOn, false));

                    if (CustomGameOptions.DetectiveOn > 0)
                        CrewmateRoles.Add((typeof(Detective), CustomGameOptions.DetectiveOn, false));

                    if (CustomGameOptions.ImitatorOn > 0)
                        CrewmateRoles.Add((typeof(Imitator), CustomGameOptions.ImitatorOn, true));

                    if (CustomGameOptions.ProsecutorOn > 0)
                        CrewmateRoles.Add((typeof(Prosecutor), CustomGameOptions.ProsecutorOn, true));

                    if (CustomGameOptions.OracleOn > 0)
                        CrewmateRoles.Add((typeof(Oracle), CustomGameOptions.OracleOn, true));

                    if (CustomGameOptions.AurialOn > 0)
                        CrewmateRoles.Add((typeof(Aurial), CustomGameOptions.AurialOn, false));
                    #endregion
                    #region Neutral Roles
                    if (CustomGameOptions.JesterOn > 0)
                        NeutralEvilRoles.Add((typeof(Jester), CustomGameOptions.JesterOn, false));

                    if (CustomGameOptions.AmnesiacOn > 0)
                        NeutralBenignRoles.Add((typeof(Amnesiac), CustomGameOptions.AmnesiacOn, false));

                    if (CustomGameOptions.ExecutionerOn > 0)
                        NeutralEvilRoles.Add((typeof(Executioner), CustomGameOptions.ExecutionerOn, false));

                    if (CustomGameOptions.DoomsayerOn > 0)
                        NeutralEvilRoles.Add((typeof(Doomsayer), CustomGameOptions.DoomsayerOn, false));

                    if (CustomGameOptions.SurvivorOn > 0)
                        NeutralBenignRoles.Add((typeof(Survivor), CustomGameOptions.SurvivorOn, false));

                    if (CustomGameOptions.GuardianAngelOn > 0)
                        NeutralBenignRoles.Add((typeof(GuardianAngel), CustomGameOptions.GuardianAngelOn, false));

                    if (CustomGameOptions.GlitchOn > 0)
                        NeutralKillingRoles.Add((typeof(Glitch), CustomGameOptions.GlitchOn, true));

                    if (CustomGameOptions.ArsonistOn > 0)
                        NeutralKillingRoles.Add((typeof(Arsonist), CustomGameOptions.ArsonistOn, true));

                    if (CustomGameOptions.PlaguebearerOn > 0)
                        NeutralKillingRoles.Add((typeof(Plaguebearer), CustomGameOptions.PlaguebearerOn, true));

                    if (CustomGameOptions.WerewolfOn > 0)
                        NeutralKillingRoles.Add((typeof(Werewolf), CustomGameOptions.WerewolfOn, true));

                    if (CustomGameOptions.GameMode == GameMode.Classic && CustomGameOptions.VampireOn > 0)
                        NeutralKillingRoles.Add((typeof(Vampire), CustomGameOptions.VampireOn, true));

                    if ((CheckJugg() || CustomGameOptions.GameMode == GameMode.AllAny) && CustomGameOptions.HiddenRoles)
                        NeutralKillingRoles.Add((typeof(Juggernaut), 100, true));
                    #endregion
                    #region Impostor Roles
                    if (CustomGameOptions.UndertakerOn > 0)
                        ImpostorRoles.Add((typeof(Undertaker), CustomGameOptions.UndertakerOn, true));

                    if (CustomGameOptions.MorphlingOn > 0)
                        ImpostorRoles.Add((typeof(Morphling), CustomGameOptions.MorphlingOn, false));

                    if (CustomGameOptions.BlackmailerOn > 0)
                        ImpostorRoles.Add((typeof(Blackmailer), CustomGameOptions.BlackmailerOn, true));

                    if (CustomGameOptions.MinerOn > 0)
                        ImpostorRoles.Add((typeof(Miner), CustomGameOptions.MinerOn, true));

                    if (CustomGameOptions.SwooperOn > 0)
                        ImpostorRoles.Add((typeof(Swooper), CustomGameOptions.SwooperOn, false));

                    if (CustomGameOptions.JanitorOn > 0)
                        ImpostorRoles.Add((typeof(Janitor), CustomGameOptions.JanitorOn, false));

                    if (CustomGameOptions.GrenadierOn > 0)
                        ImpostorRoles.Add((typeof(Grenadier), CustomGameOptions.GrenadierOn, true));

                    if (CustomGameOptions.EscapistOn > 0)
                        ImpostorRoles.Add((typeof(Escapist), CustomGameOptions.EscapistOn, false));

                    if (CustomGameOptions.BomberOn > 0)
                        ImpostorRoles.Add((typeof(Bomber), CustomGameOptions.BomberOn, true));

                    if (CustomGameOptions.WarlockOn > 0)
                        ImpostorRoles.Add((typeof(Warlock), CustomGameOptions.WarlockOn, false));

                    if (CustomGameOptions.VenererOn > 0)
                        ImpostorRoles.Add((typeof(Venerer), CustomGameOptions.VenererOn, true));
                    #endregion
                    #region Crewmate Modifiers
                    if (Check(CustomGameOptions.TorchOn))
                        CrewmateModifiers.Add((typeof(Torch), CustomGameOptions.TorchOn));

                    if (Check(CustomGameOptions.DiseasedOn))
                        CrewmateModifiers.Add((typeof(Diseased), CustomGameOptions.DiseasedOn));

                    if (Check(CustomGameOptions.BaitOn))
                        CrewmateModifiers.Add((typeof(Bait), CustomGameOptions.BaitOn));

                    if (Check(CustomGameOptions.AftermathOn))
                        CrewmateModifiers.Add((typeof(Aftermath), CustomGameOptions.AftermathOn));

                    if (Check(CustomGameOptions.MultitaskerOn))
                        CrewmateModifiers.Add((typeof(Multitasker), CustomGameOptions.MultitaskerOn));

                    if (Check(CustomGameOptions.FrostyOn))
                        CrewmateModifiers.Add((typeof(Frosty), CustomGameOptions.FrostyOn));
                    #endregion
                    #region Global Modifiers
                    if (Check(CustomGameOptions.TiebreakerOn))
                        GlobalModifiers.Add((typeof(Tiebreaker), CustomGameOptions.TiebreakerOn));

                    if (Check(CustomGameOptions.FlashOn))
                        GlobalModifiers.Add((typeof(Flash), CustomGameOptions.FlashOn));

                    if (Check(CustomGameOptions.GiantOn))
                        GlobalModifiers.Add((typeof(Giant), CustomGameOptions.GiantOn));

                    if (Check(CustomGameOptions.ButtonBarryOn))
                        ButtonModifiers.Add((typeof(ButtonBarry), CustomGameOptions.ButtonBarryOn));

                    if (Check(CustomGameOptions.LoversOn))
                        GlobalModifiers.Add((typeof(Lover), CustomGameOptions.LoversOn));

                    if (Check(CustomGameOptions.SleuthOn))
                        GlobalModifiers.Add((typeof(Sleuth), CustomGameOptions.SleuthOn));

                    if (Check(CustomGameOptions.RadarOn))
                        GlobalModifiers.Add((typeof(Radar), CustomGameOptions.RadarOn));
                    #endregion
                    #region Impostor Modifiers
                    if (Check(CustomGameOptions.DisperserOn) && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 4 && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 5)
                        ImpostorModifiers.Add((typeof(Disperser), CustomGameOptions.DisperserOn));

                    if (Check(CustomGameOptions.DoubleShotOn))
                        AssassinModifiers.Add((typeof(DoubleShot), CustomGameOptions.DoubleShotOn));

                    if (CustomGameOptions.UnderdogOn > 0)
                        ImpostorModifiers.Add((typeof(Underdog), CustomGameOptions.UnderdogOn));
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

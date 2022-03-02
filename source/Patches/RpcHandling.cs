using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor;
using Reactor.Extensions;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.CrewmateRoles.TimeLordMod;
using TownOfUs.CrewmateRoles.VigilanteMod;
using TownOfUs.CustomOption;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.ImpostorRoles.MinerMod;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.PhantomMod;
using TownOfUs.CrewmateRoles.HaunterMod;
using TownOfUs.ImpostorRoles.TraitorMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using Coroutine = TownOfUs.ImpostorRoles.JanitorMod.Coroutine;
using Object = UnityEngine.Object;
using PerformKillButton = TownOfUs.NeutralRoles.AmnesiacMod.PerformKillButton;
using Random = UnityEngine.Random; //using Il2CppSystem;

namespace TownOfUs
{
    public static class RpcHandling
    {
        private static readonly List<(Type, CustomRPC, int)> CrewmateRoles = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NeutralRoles = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> ImpostorRoles = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> CrewmateModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> GlobalModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> ButtonModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> AssassinModifier = new List<(Type, CustomRPC, int)>();
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
            return num <= 90;
        }

        /*
        private static void GenExe(List<GameData.PlayerInfo> infected, List<PlayerControl> crewmates)
        {
            PlayerControl pc;
            var targets = Utils.getCrewmates(infected).Where(x =>
            {
                var role = Role.GetRole(x);
                if (role == null) return true;
                return role.Faction == Faction.Crewmates;
            }).ToList();
            if (targets.Count > 1)
            {
                var rand = Random.RandomRangeInt(0, targets.Count);
                pc = targets[rand];
                var role = Role.Gen(typeof(Executioner), crewmates.Where(x => x.PlayerId != pc.PlayerId).ToList(),
                    CustomRPC.SetExecutioner);
                if (role != null)
                {
                    crewmates.Remove(role.Player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.SetTarget, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(pc.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    ((Executioner) role).target = pc;
                }
            }
        }*/

        private static void SortRoles(List<(Type, CustomRPC, int)> roles, int max = int.MaxValue)
        {
            roles.Shuffle();
            roles.Sort((a, b) =>
            {
                var a_ = a.Item3 == 100 ? 0 : 100;
                var b_ = b.Item3 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });
            if (roles.Count > max)
                while (roles.Count > max)
                    roles.RemoveAt(roles.Count - 1);
        }

        private static void GenEachRole(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();
            impostors.Shuffle();

            while(CrewmateRoles.Count + NeutralRoles.Count < crewmates.Count)
            {
                CrewmateRoles.Add((typeof(Crewmate), CustomRPC.SetCrewmate, 1));
            }
            while(ImpostorRoles.Count < impostors.Count)
            {
                ImpostorRoles.Add((typeof(Impostor), CustomRPC.SetImpostor, 1));
            }

            SortRoles(NeutralRoles, CustomGameOptions.MaxNeutralRoles);
            SortRoles(CrewmateRoles, crewmates.Count - NeutralRoles.Count);
            SortRoles(ImpostorRoles, impostors.Count);
            SortRoles(CrewmateModifiers, crewmates.Count);
            SortRoles(GlobalModifiers, crewmates.Count + impostors.Count);
            SortRoles(ButtonModifiers, crewmates.Count + impostors.Count);

            var crewAndNeutralRoles = new List<(Type, CustomRPC, int)>();
            crewAndNeutralRoles.AddRange(NeutralRoles);
            crewAndNeutralRoles.AddRange(CrewmateRoles);
            SortRoles(crewAndNeutralRoles, crewmates.Count);

            if (Check(CustomGameOptions.VanillaGame))
            {
                CrewmateRoles.Clear();
                NeutralRoles.Clear();
                CrewmateModifiers.Clear();
                GlobalModifiers.Clear();
                ButtonModifiers.Clear();
                AssassinModifier.Clear();
                ImpostorRoles.Clear();
                PhantomOn = false;
                HaunterOn = false;
                TraitorOn = false;
            }

            List<PlayerControl> executionerList = new List<PlayerControl>();

            foreach (var (type, rpc, _) in crewAndNeutralRoles)
            {
                if (rpc == CustomRPC.SetExecutioner)
                {
                    var executioner = crewmates[Random.RandomRangeInt(0, crewmates.Count)];
                    executionerList.Add(executioner);
                    crewmates.Remove(executioner);
                    continue;
                }
                    
                Role.Gen<Role>(type, crewmates, rpc);
            }

            while (impostors.Count > 0)
            {
                var (type, rpc, _) = ImpostorRoles.TakeFirst();
                if (type == null) break;
                Role.Gen<Role>(type, impostors.TakeFirst(), rpc);
            }

            foreach (var crewmate in crewmates)
                Role.Gen<Role>(typeof(Crewmate), crewmate, CustomRPC.SetCrewmate);

            foreach (var impostor in impostors)
                Role.Gen<Role>(typeof(Impostor), impostor, CustomRPC.SetImpostor);

            var canHaveModifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            canHaveModifier.Shuffle();
            canHaveAbility.RemoveAll(player => !player.Is(Faction.Impostors));
            canHaveAbility.Shuffle();
            var assassins = CustomGameOptions.NumberOfAssassins;

            foreach (var (type, rpc, _) in GlobalModifiers)
            {
                if (canHaveModifier.Count == 0) break;
                if(rpc == CustomRPC.SetCouple)
                {
                    if (canHaveModifier.Count == 1) continue;
                        Lover.Gen(canHaveModifier);
                }
                else
                {
                    Role.Gen<Modifier>(type, canHaveModifier, rpc);
                }
            }

            canHaveModifier.RemoveAll(player => player.Is(RoleEnum.Glitch));

            foreach (var (type, rpc, _) in ButtonModifiers)
            {
                if (canHaveModifier.Count == 0) break;
                Role.Gen<Modifier>(type, canHaveModifier, rpc);
            }

            canHaveModifier.RemoveAll(player => player.Is(RoleEnum.Juggernaut) || player.Is(Faction.Impostors));
            canHaveModifier.Shuffle();

            while (canHaveModifier.Count > 0 && CrewmateModifiers.Count > 0)
            {
                var (type, rpc, _) = CrewmateModifiers.TakeFirst();
                Role.Gen<Modifier>(type, canHaveModifier.TakeFirst(), rpc);
            }

            while (canHaveAbility.Count > 0 && assassins > 0)
            {
                var (type, rpc, _) = AssassinModifier.Ability();
                Role.Gen<Ability>(type, canHaveAbility.TakeFirst(), rpc);
                assassins = assassins - 1;
            }

            var becomeRoles = 0;
            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ModifierEnum.Lover)).ToList();
            toChooseFrom.Shuffle();
            var phantomSpawn = false;
            var haunterSpawn = false;
            var traitorSpawn = false;
            if (PhantomOn) becomeRoles = becomeRoles + 1;
            if (HaunterOn) becomeRoles = becomeRoles + 1;
            if (TraitorOn) becomeRoles = becomeRoles + 1;
            while (becomeRoles > toChooseFrom.Count)
            {
                var subtractRole = Random.RandomRangeInt(0, 3);
                if (subtractRole == 0)
                {
                    if (PhantomOn == true)
                    {
                        PhantomOn = false;
                        becomeRoles--;
                    }
                }
                else if (subtractRole == 1)
                {
                    if (HaunterOn == true)
                    {
                        HaunterOn = false;
                        becomeRoles--;
                    }
                }
                else
                {
                    if (TraitorOn == true)
                    {
                        TraitorOn = false;
                        becomeRoles--;
                    }
                }
            }
            var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
            var rand2 = Random.RandomRangeInt(0, toChooseFrom.Count);
            var rand3 = Random.RandomRangeInt(0, toChooseFrom.Count);
            if (becomeRoles == 3)
            {
                phantomSpawn = true;
                haunterSpawn = true;
                traitorSpawn = true;
                while (rand == rand2)
                {
                    rand2 = Random.RandomRangeInt(0, toChooseFrom.Count);
                }
                while (rand == rand3 || rand2 == rand3)
                {
                    rand3 = Random.RandomRangeInt(0, toChooseFrom.Count);
                }
            }
            else if (becomeRoles == 2)
            {
                if (PhantomOn) phantomSpawn = true;
                if (HaunterOn) haunterSpawn = true;
                if (TraitorOn) traitorSpawn = true;
                if (phantomSpawn == true)
                {
                    while (rand == rand2)
                    {
                        rand2 = Random.RandomRangeInt(0, toChooseFrom.Count);
                    }
                    while (rand == rand3)
                    {
                        rand3 = Random.RandomRangeInt(0, toChooseFrom.Count);
                    }
                }
                else
                {
                    while (rand2 == rand3)
                    {
                        rand3 = Random.RandomRangeInt(0, toChooseFrom.Count);
                    }
                }
            }
            else if (becomeRoles == 1)
            {
                if (PhantomOn) phantomSpawn = true;
                if (HaunterOn) haunterSpawn = true;
                if (TraitorOn) traitorSpawn = true;
            }
            var pc = toChooseFrom[rand];
            var pc2 = toChooseFrom[rand2];
            var pc3 = toChooseFrom[rand3];
            if (phantomSpawn == true)
            {
                SetPhantom.WillBePhantom = pc;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                writer.Write(pc.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            else
            {
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                writer2.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
            }
            if (haunterSpawn == true)
            {
                SetHaunter.WillBeHaunter = pc2;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetHaunter, SendOption.Reliable, -1);
                writer.Write(pc2.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            else
            {
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetHaunter, SendOption.Reliable, -1);
                writer2.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
            }
            if (traitorSpawn == true)
            {
                SetTraitor.WillBeTraitor = pc3;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetTraitor, SendOption.Reliable, -1);
                writer.Write(pc3.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            else
            {
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetTraitor, SendOption.Reliable, -1);
                writer2.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
            }

            if (executionerList.Count > 0)
            {
                var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ModifierEnum.Lover) && !x.Is(RoleEnum.Mayor) && !x.Is(RoleEnum.Swapper) && !x.Is(RoleEnum.Vigilante) && x != SetTraitor.WillBeTraitor).ToList();
                foreach (var executioner in executionerList)
                {
                    if (targets.Count > 0)
                    {
                        var exec = Role.Gen<Executioner>(
                            typeof(Executioner),
                            executioner,
                            CustomRPC.SetExecutioner
                        );
                        var target = exec.target = targets[Random.RandomRangeInt(0, targets.Count)];

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.SetTarget, SendOption.Reliable, -1);
                        writer.Write(executioner.PlayerId);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        targets.Remove(target);
                    }
                    else
                        Role.Gen<Role>(typeof(Crewmate), executioner, CustomRPC.SetExecutioner);
                }
            }
        }


        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpc
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                //if (callId >= 43) //System.Console.WriteLine("Received " + callId);
                byte readByte, readByte1, readByte2;
                sbyte readSByte, readSByte2;
                switch ((CustomRPC) callId)
                {
                    case CustomRPC.SetMayor:
                        readByte = reader.ReadByte();
                        new Mayor(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetJester:
                        readByte = reader.ReadByte();
                        new Jester(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetSheriff:
                        readByte = reader.ReadByte();
                        new Sheriff(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetEngineer:
                        readByte = reader.ReadByte();
                        new Engineer(Utils.PlayerById(readByte));
                        break;


                    case CustomRPC.SetJanitor:
                        new Janitor(Utils.PlayerById(reader.ReadByte()));

                        break;

                    case CustomRPC.SetSwapper:
                        readByte = reader.ReadByte();
                        new Swapper(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetAmnesiac:
                        readByte = reader.ReadByte();
                        new Amnesiac(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetInvestigator:
                        readByte = reader.ReadByte();
                        new Investigator(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetTimeLord:
                        readByte = reader.ReadByte();
                        new TimeLord(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetTorch:
                        readByte = reader.ReadByte();
                        new Torch(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetDiseased:
                        readByte = reader.ReadByte();
                        new Diseased(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetBait:
                        readByte = reader.ReadByte();
                        new Bait(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetFlash:
                        readByte = reader.ReadByte();
                        new Flash(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetMedic:
                        readByte = reader.ReadByte();
                        new Medic(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetMorphling:
                        readByte = reader.ReadByte();
                        new Morphling(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetPoisoner:
                        readByte = reader.ReadByte();
                        new Poisoner(Utils.PlayerById(readByte));
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

                    /*case CustomRPC.ShifterLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Shifter)
                                ((Shifter) role).Loses();

                        break;*/

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
                        /*
                        EngineerMod.PerformKill.UsedThisRound = false;
                        EngineerMod.PerformKill.SabotageTime = DateTime.UtcNow.AddSeconds(-100);
                        */
                        Utils.ShowDeadBodies = false;
                        Murder.KilledPlayers.Clear();
                        Role.NobodyWins = false;
                        RecordRewind.points.Clear();
                        KillButtonTarget.DontRevive = byte.MaxValue;
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
                        Role.GetRole<Engineer>(engineer).UsedThisRound = true;
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
                    case CustomRPC.Remember:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var amnesiac = Utils.PlayerById(readByte1);
                        var other = Utils.PlayerById(readByte2);
                        PerformKillButton.Remember(Role.GetRole<Amnesiac>(amnesiac), other);
                        break;
                    case CustomRPC.Rewind:
                        readByte = reader.ReadByte();
                        var TimeLordPlayer = Utils.PlayerById(readByte);
                        var TimeLordRole = Role.GetRole<TimeLord>(TimeLordPlayer);
                        StartStop.StartRewind(TimeLordRole);
                        break;
                    case CustomRPC.Protect:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();

                        var medic = Utils.PlayerById(readByte1);
                        var shield = Utils.PlayerById(readByte2);
                        Role.GetRole<Medic>(medic).ShieldedPlayer = shield;
                        Role.GetRole<Medic>(medic).UsedAbility = true;
                        break;
                    case CustomRPC.RewindRevive:
                        readByte = reader.ReadByte();
                        RecordRewind.ReviveBody(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.AttemptSound:
                        var medicId = reader.ReadByte();
                        readByte = reader.ReadByte();
                        StopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
                        break;
                    case CustomRPC.SetGlitch:
                        var GlitchId = reader.ReadByte();
                        var GlitchPlayer = Utils.PlayerById(GlitchId);
                        new Glitch(GlitchPlayer);
                        break;
                    case CustomRPC.SetJuggernaut:
                        var JuggernautId = reader.ReadByte();
                        var JuggernautPlayer = Utils.PlayerById(JuggernautId);
                        new Juggernaut(JuggernautPlayer);
                        break;
                    case CustomRPC.BypassKill:
                        var killer = Utils.PlayerById(reader.ReadByte());
                        var target = Utils.PlayerById(reader.ReadByte());

                        Utils.MurderPlayer(killer, target);
                        break;
                    case CustomRPC.AssassinKill:
                        var toDie = Utils.PlayerById(reader.ReadByte());
                        AssassinKill.MurderPlayer(toDie);
                        break;
                    case CustomRPC.VigilanteKill:
                        var toDie2 = Utils.PlayerById(reader.ReadByte());
                        VigilanteKill.MurderPlayer(toDie2);
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
                    case CustomRPC.Investigate:
                        var seer = Utils.PlayerById(reader.ReadByte());
                        var otherPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Seer>(seer).Investigated.Add(otherPlayer.PlayerId);
                        Role.GetRole<Seer>(seer).LastInvestigated = DateTime.UtcNow;
                        break;
                    case CustomRPC.SetSeer:
                        new Seer(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Morph:
                        var morphling = Utils.PlayerById(reader.ReadByte());
                        var morphTarget = Utils.PlayerById(reader.ReadByte());
                        var morphRole = Role.GetRole<Morphling>(morphling);
                        morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                        morphRole.MorphedPlayer = morphTarget;
                        break;
                    case CustomRPC.SetExecutioner:
                        new Executioner(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetTarget:
                        var executioner = Utils.PlayerById(reader.ReadByte());
                        var exeTarget = Utils.PlayerById(reader.ReadByte());
                        var exeRole = Role.GetRole<Executioner>(executioner);
                        exeRole.target = exeTarget;
                        break;
                    case CustomRPC.SetCamouflager:
                        new Camouflager(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Camouflage:
                        var camouflager = Utils.PlayerById(reader.ReadByte());
                        var camouflagerRole = Role.GetRole<Camouflager>(camouflager);
                        camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                        Utils.Camouflage();
                        break;
                    case CustomRPC.SetSpy:
                        new Spy(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.ExecutionerToJester:
                        TargetColor.ExeToJes(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetSnitch:
                        new Snitch(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetTracker:
                        new Tracker(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetMiner:
                        new Miner(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Mine:
                        var ventId = reader.ReadInt32();
                        var miner = Utils.PlayerById(reader.ReadByte());
                        var minerRole = Role.GetRole<Miner>(miner);
                        var pos = reader.ReadVector2();
                        var zAxis = reader.ReadSingle();
                        PerformKill.SpawnVent(ventId, minerRole, pos, zAxis);
                        break;
                    case CustomRPC.SetSwooper:
                        new Swooper(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Swoop:
                        var swooper = Utils.PlayerById(reader.ReadByte());
                        var swooperRole = Role.GetRole<Swooper>(swooper);
                        swooperRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                        swooperRole.Swoop();
                        break;
                    case CustomRPC.Alert:
                        var veteran = Utils.PlayerById(reader.ReadByte());
                        var veteranRole = Role.GetRole<Veteran>(veteran);
                        veteranRole.TimeRemaining = CustomGameOptions.AlertDuration;
                        veteranRole.Alert();
                        break;
                    case CustomRPC.Transport:
                        Transporter.TransportPlayers(reader.ReadByte(), reader.ReadByte());
                        break;
                    case CustomRPC.SetGrenadier:
                        new Grenadier(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.FlashGrenade:
                        var grenadier = Utils.PlayerById(reader.ReadByte());
                        var grenadierRole = Role.GetRole<Grenadier>(grenadier);
                        grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                        grenadierRole.Flash();
                        break;
                    case CustomRPC.SetTiebreaker:
                        new Tiebreaker(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetDrunk:
                        new Drunk(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetSleuth:
                        new Sleuth(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetArsonist:
                        new Arsonist(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Douse:
                        var arsonist = Utils.PlayerById(reader.ReadByte());
                        var douseTarget = Utils.PlayerById(reader.ReadByte());
                        var arsonistRole = Role.GetRole<Arsonist>(arsonist);
                        arsonistRole.DousedPlayers.Add(douseTarget.PlayerId);
                        arsonistRole.LastDoused = DateTime.UtcNow;

                        break;
                    case CustomRPC.Ignite:
                        var theArsonist = Utils.PlayerById(reader.ReadByte());
                        var theArsonistRole = Role.GetRole<Arsonist>(theArsonist);
                        global::TownOfUs.NeutralRoles.ArsonistMod.PerformKill.Ignite(theArsonistRole);
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
                    case CustomRPC.SetImpostor:
                        new Impostor(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetCrewmate:
                        new Crewmate(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SyncCustomSettings:
                        Rpc.ReceiveRpc(reader);
                        break;
                    case CustomRPC.SetAltruist:
                        new Altruist(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetGiant:
                        new Giant(Utils.PlayerById(reader.ReadByte()));
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
                        var player = Utils.PlayerById(reader.ReadByte());
                        player.MyPhysics.ResetMoveState();
                        player.Collider.enabled = true;
                        player.moveable = true;
                        player.NetTransform.enabled = true;
                        break;
                    case CustomRPC.SetButtonBarry:
                        new ButtonBarry(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.BarryButton:
                        var buttonBarry = Utils.PlayerById(reader.ReadByte());
                        buttonBarry.ReportDeadBody(null);
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
                    case CustomRPC.SetUndertaker:
                        new Undertaker(Utils.PlayerById(reader.ReadByte()));
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
                    case CustomRPC.SetVigilante:
                        new Vigilante(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetVeteran:
                        new Veteran(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetTransporter:
                        new Transporter(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetUnderdog:
                        new Underdog(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetPhantom:
                        readByte = reader.ReadByte();
                        SetPhantom.WillBePhantom = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.PhantomDied:
                        var phantom = SetPhantom.WillBePhantom;
                        Role.RoleDictionary.Remove(phantom.PlayerId);
                        var phantomRole = new Phantom(phantom);
                        phantomRole.RegenTask();
                        phantom.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetPhantom.RemoveTasks(phantom);
                        SetPhantom.AddCollider(phantomRole);
                        if (!PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                        {
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        }
                        System.Console.WriteLine("Become Phantom - Users");
                        break;
                    case CustomRPC.CatchPhantom:
                        var phantomPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Phantom>(phantomPlayer).Caught = true;
                        break;
                    case CustomRPC.PhantomWin:
                        Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte())).CompletedTasks = true;
                        break;
                    case CustomRPC.SetHaunter:
                        readByte = reader.ReadByte();
                        SetHaunter.WillBeHaunter = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.HaunterDied:
                        var haunter = SetHaunter.WillBeHaunter;
                        Role.RoleDictionary.Remove(haunter.PlayerId);
                        var haunterRole = new Haunter(haunter);
                        haunterRole.RegenTask();
                        haunter.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetHaunter.RemoveTasks(haunter);
                        SetHaunter.AddCollider(haunterRole);
                        if (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                        {
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        }
                        System.Console.WriteLine("Become Haunter - Users");
                        break;
                    case CustomRPC.CatchHaunter:
                        var haunterPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Haunter>(haunterPlayer).Caught = true;
                        break;
                    case CustomRPC.HaunterFinished:
                        HighlightImpostors.UpdateMeeting(MeetingHud.Instance);
                        break;
                    case CustomRPC.SetTraitor:
                        readByte = reader.ReadByte();
                        SetTraitor.WillBeTraitor = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.TraitorSpawn:
                        var traitor = SetTraitor.WillBeTraitor;
                        Role.RoleDictionary.Remove(traitor.PlayerId);
                        var traitorRole = new Traitor(traitor);
                        traitorRole.RegenTask();
                        SetTraitor.TurnImp(traitor);
                        break;
                    case CustomRPC.AddMayorVoteBank:
                        Role.GetRole<Mayor>(Utils.PlayerById(reader.ReadByte())).VoteBank += reader.ReadInt32();
                        break;
                    case CustomRPC.RemoveAllBodies:
                        var buggedBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in buggedBodies)
                            body.gameObject.Destroy();
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
                CrewmateRoles.Clear();
                NeutralRoles.Clear();
                ImpostorRoles.Clear();
                CrewmateModifiers.Clear();
                GlobalModifiers.Clear();
                ButtonModifiers.Clear();
                AssassinModifier.Clear();

                RecordRewind.points.Clear();
                Murder.KilledPlayers.Clear();
                KillButtonTarget.DontRevive = byte.MaxValue;

                var startWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Start, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(startWriter);


                PhantomOn = Check(CustomGameOptions.PhantomOn);
                HaunterOn = Check(CustomGameOptions.HaunterOn);
                TraitorOn = Check(CustomGameOptions.TraitorOn);

                #region Crewmate Roles
                if (Check(CustomGameOptions.MayorOn))
                    CrewmateRoles.Add((typeof(Mayor), CustomRPC.SetMayor, CustomGameOptions.MayorOn));

                if (Check(CustomGameOptions.SheriffOn))
                    CrewmateRoles.Add((typeof(Sheriff), CustomRPC.SetSheriff, CustomGameOptions.SheriffOn));

                if (Check(CustomGameOptions.EngineerOn))
                    CrewmateRoles.Add((typeof(Engineer), CustomRPC.SetEngineer, CustomGameOptions.EngineerOn));

                if (Check(CustomGameOptions.SwapperOn))
                    CrewmateRoles.Add((typeof(Swapper), CustomRPC.SetSwapper, CustomGameOptions.SwapperOn));

                if (Check(CustomGameOptions.InvestigatorOn))
                    CrewmateRoles.Add((typeof(Investigator), CustomRPC.SetInvestigator, CustomGameOptions.InvestigatorOn));

                if (Check(CustomGameOptions.TimeLordOn))
                    CrewmateRoles.Add((typeof(TimeLord), CustomRPC.SetTimeLord, CustomGameOptions.TimeLordOn));

                if (Check(CustomGameOptions.MedicOn))
                    CrewmateRoles.Add((typeof(Medic), CustomRPC.SetMedic, CustomGameOptions.MedicOn));

                if (Check(CustomGameOptions.SeerOn))
                    CrewmateRoles.Add((typeof(Seer), CustomRPC.SetSeer, CustomGameOptions.SeerOn));

                if (Check(CustomGameOptions.SpyOn))
                    CrewmateRoles.Add((typeof(Spy), CustomRPC.SetSpy, CustomGameOptions.SpyOn));

                if (Check(CustomGameOptions.SnitchOn))
                    CrewmateRoles.Add((typeof(Snitch), CustomRPC.SetSnitch, CustomGameOptions.SnitchOn));

                if (Check(CustomGameOptions.AltruistOn))
                    CrewmateRoles.Add((typeof(Altruist), CustomRPC.SetAltruist, CustomGameOptions.AltruistOn));

                if (Check(CustomGameOptions.VigilanteOn))
                    CrewmateRoles.Add((typeof(Vigilante), CustomRPC.SetVigilante, CustomGameOptions.VigilanteOn));

                if (Check(CustomGameOptions.VeteranOn))
                    CrewmateRoles.Add((typeof(Veteran), CustomRPC.SetVeteran, CustomGameOptions.VeteranOn));

                if (Check(CustomGameOptions.TrackerOn))
                    CrewmateRoles.Add((typeof(Tracker), CustomRPC.SetTracker, CustomGameOptions.TrackerOn));

                if (Check(CustomGameOptions.TransporterOn))
                    CrewmateRoles.Add((typeof(Transporter), CustomRPC.SetTransporter, CustomGameOptions.TransporterOn));
                #endregion
                #region Neutral Roles
                if (Check(CustomGameOptions.JesterOn))
                    NeutralRoles.Add((typeof(Jester), CustomRPC.SetJester, CustomGameOptions.JesterOn));

                if (Check(CustomGameOptions.AmnesiacOn))
                    NeutralRoles.Add((typeof(Amnesiac), CustomRPC.SetAmnesiac, CustomGameOptions.AmnesiacOn));

                if (Check(CustomGameOptions.GlitchOn))
                    if (CheckJugg())
                        NeutralRoles.Add((typeof(Glitch), CustomRPC.SetGlitch, CustomGameOptions.GlitchOn));
                    else
                        NeutralRoles.Add((typeof(Juggernaut), CustomRPC.SetJuggernaut, CustomGameOptions.GlitchOn));

                if (Check(CustomGameOptions.ArsonistOn))
                    NeutralRoles.Add((typeof(Arsonist), CustomRPC.SetArsonist, CustomGameOptions.ArsonistOn));

                if (Check(CustomGameOptions.ExecutionerOn))
                    NeutralRoles.Add((typeof(Executioner), CustomRPC.SetExecutioner, CustomGameOptions.ExecutionerOn));
                #endregion
                #region Impostor Roles
                if (Check(CustomGameOptions.UndertakerOn))
                    ImpostorRoles.Add((typeof(Undertaker), CustomRPC.SetUndertaker, CustomGameOptions.UndertakerOn));

                if (Check(CustomGameOptions.UnderdogOn))
                    ImpostorRoles.Add((typeof(Underdog), CustomRPC.SetUnderdog, CustomGameOptions.UnderdogOn));

                if (Check(CustomGameOptions.MorphlingOn))
                    ImpostorRoles.Add((typeof(Morphling), CustomRPC.SetMorphling, CustomGameOptions.MorphlingOn));

                if (Check(CustomGameOptions.CamouflagerOn))
                    ImpostorRoles.Add((typeof(Camouflager), CustomRPC.SetCamouflager, CustomGameOptions.CamouflagerOn));

                if (Check(CustomGameOptions.MinerOn))
                    ImpostorRoles.Add((typeof(Miner), CustomRPC.SetMiner, CustomGameOptions.MinerOn));

                if (Check(CustomGameOptions.SwooperOn))
                    ImpostorRoles.Add((typeof(Swooper), CustomRPC.SetSwooper, CustomGameOptions.SwooperOn));

                if (Check(CustomGameOptions.JanitorOn))
                    ImpostorRoles.Add((typeof(Janitor), CustomRPC.SetJanitor, CustomGameOptions.JanitorOn));

                if (Check(CustomGameOptions.GrenadierOn))
                    ImpostorRoles.Add((typeof(Grenadier), CustomRPC.SetGrenadier, CustomGameOptions.GrenadierOn));

                if (Check(CustomGameOptions.PoisonerOn))
                    ImpostorRoles.Add((typeof(Poisoner), CustomRPC.SetPoisoner, CustomGameOptions.PoisonerOn));
                #endregion
                #region Crewmate Modifiers
                if (Check(CustomGameOptions.TorchOn))
                    CrewmateModifiers.Add((typeof(Torch), CustomRPC.SetTorch, CustomGameOptions.TorchOn));

                if (Check(CustomGameOptions.DiseasedOn))
                    CrewmateModifiers.Add((typeof(Diseased), CustomRPC.SetDiseased, CustomGameOptions.DiseasedOn));

                if (Check(CustomGameOptions.BaitOn))
                    CrewmateModifiers.Add((typeof(Bait), CustomRPC.SetBait, CustomGameOptions.BaitOn));
                #endregion
                #region Global Modifiers
                if (Check(CustomGameOptions.TiebreakerOn))
                    GlobalModifiers.Add((typeof(Tiebreaker), CustomRPC.SetTiebreaker, CustomGameOptions.TiebreakerOn));

                if (Check(CustomGameOptions.FlashOn))
                    GlobalModifiers.Add((typeof(Flash), CustomRPC.SetFlash, CustomGameOptions.FlashOn));

                if (Check(CustomGameOptions.DrunkOn))
                    GlobalModifiers.Add((typeof(Drunk), CustomRPC.SetDrunk, CustomGameOptions.DrunkOn));

                if (Check(CustomGameOptions.GiantOn))
                    GlobalModifiers.Add((typeof(Giant), CustomRPC.SetGiant, CustomGameOptions.GiantOn));

                if (Check(CustomGameOptions.ButtonBarryOn))
                    ButtonModifiers.Add((typeof(ButtonBarry), CustomRPC.SetButtonBarry, CustomGameOptions.ButtonBarryOn));

                if (Check(CustomGameOptions.LoversOn))
                    GlobalModifiers.Add((typeof(Lover), CustomRPC.SetCouple, CustomGameOptions.LoversOn));
                    
                if (Check(CustomGameOptions.SleuthOn))
                    GlobalModifiers.Add((typeof(Sleuth), CustomRPC.SetSleuth, CustomGameOptions.SleuthOn));
                #endregion
                #region Assassin Modifier
                AssassinModifier.Add((typeof(Assassin), CustomRPC.SetAssassin, 100));
                #endregion
                GenEachRole(infected.ToList());
            }
        }
    }
}

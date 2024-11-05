using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Extensions;

namespace TownOfUs
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class AmongUsClientGameEnd
    {
        public static void Postfix()
        {
            List<int> losers = new List<int>();
            foreach (var role in Role.GetRoles(RoleEnum.Amnesiac))
            {
                var amne = (Amnesiac)role;
                losers.Add(amne.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                losers.Add(ga.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Survivor))
            {
                var surv = (Survivor)role;
                losers.Add(surv.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Doomsayer))
            {
                var doom = (Doomsayer)role;
                losers.Add(doom.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                losers.Add(exe.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Jester))
            {
                var jest = (Jester)role;
                losers.Add(jest.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Phantom))
            {
                var phan = (Phantom)role;
                losers.Add(phan.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.SoulCollector))
            {
                var sc = (SoulCollector)role;
                losers.Add(sc.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Arsonist))
            {
                var arso = (Arsonist)role;
                losers.Add(arso.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Juggernaut))
            {
                var jugg = (Juggernaut)role;
                losers.Add(jugg.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Pestilence))
            {
                var pest = (Pestilence)role;
                losers.Add(pest.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Plaguebearer))
            {
                var pb = (Plaguebearer)role;
                losers.Add(pb.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch)role;
                losers.Add(glitch.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Vampire))
            {
                var vamp = (Vampire)role;
                losers.Add(vamp.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Werewolf))
            {
                var ww = (Werewolf)role;
                losers.Add(ww.Player.GetDefaultOutfit().ColorId);
            }

            var toRemoveWinners = EndGameResult.CachedWinners.ToArray().Where(o => losers.Contains(o.ColorId)).ToArray();
            for (int i = 0; i < toRemoveWinners.Count(); i++) EndGameResult.CachedWinners.Remove(toRemoveWinners[i]);

            if (Role.NobodyWins)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                return;
            }
            if (Role.SurvOnlyWins)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                foreach (var role in Role.GetRoles(RoleEnum.Survivor))
                {
                    var surv = (Survivor)role;
                    if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                    {
                        var survData = new CachedPlayerData(surv.Player.Data);
                        if (PlayerControl.LocalPlayer != surv.Player) survData.IsYou = false;
                        EndGameResult.CachedWinners.Add(survData);
                    }
                }

                return;
            }

            if (CustomGameOptions.JesterEndsGame)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Jester))
                {
                    var jester = (Jester)role;
                    if (jester.VotedOut)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var jestData = new CachedPlayerData(jester.Player.Data);
                        jestData.IsDead = false;
                        if (PlayerControl.LocalPlayer != jester.Player) jestData.IsYou = false;
                        EndGameResult.CachedWinners.Add(jestData);
                        return;
                    }

                }
            }

            if (CustomGameOptions.ExecutionerEndsGame)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Executioner))
                {
                    var executioner = (Executioner)role;
                    if (executioner.TargetVotedOut)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var exeData = new CachedPlayerData(executioner.Player.Data);
                        if (PlayerControl.LocalPlayer != executioner.Player) exeData.IsYou = false;
                        EndGameResult.CachedWinners.Add(exeData);
                        return;
                    }
                }
            }

            if (CustomGameOptions.DoomsayerEndsGame)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Executioner))
                {
                    var doom = (Doomsayer)role;
                    if (doom.WonByGuessing)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var doomData = new CachedPlayerData(doom.Player.Data);
                        if (PlayerControl.LocalPlayer != doom.Player) doomData.IsYou = false;
                        EndGameResult.CachedWinners.Add(doomData);
                        return;
                    }
                }
            }

            if (CustomGameOptions.DoomsayerEndsGame)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Executioner))
                {
                    var doom = (Doomsayer)role;
                    if (doom.WonByGuessing)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var doomData = new CachedPlayerData(doom.Player.Data);
                        if (PlayerControl.LocalPlayer != doom.Player) doomData.IsYou = false;
                        EndGameResult.CachedWinners.Add(doomData);
                        return;
                    }
                }
            }

            if (CustomGameOptions.SoulCollectorEndsGame)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Executioner))
                {
                    var sc = (SoulCollector)role;
                    if (sc.CollectedSouls)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var scData = new CachedPlayerData(sc.Player.Data);
                        if (PlayerControl.LocalPlayer != sc.Player) scData.IsYou = false;
                        EndGameResult.CachedWinners.Add(scData);
                        return;
                    }
                }
            }

            if (CustomGameOptions.PhantomEndsGame)
            {
                foreach (var role in Role.GetRoles(RoleEnum.Executioner))
                {
                    var phantom = (Phantom)role;
                    if (phantom.CompletedTasks)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var phantomData = new CachedPlayerData(phantom.Player.Data);
                        if (PlayerControl.LocalPlayer != phantom.Player) phantomData.IsYou = false;
                        EndGameResult.CachedWinners.Add(phantomData);
                        return;
                    }
                }
            }

            foreach (var modifier in Modifier.AllModifiers)
            {
                var type = modifier.ModifierType;

                if (type == ModifierEnum.Lover)
                {
                    var lover = (Lover)modifier;
                    if (lover.LoveCoupleWins)
                    {
                        var otherLover = lover.OtherLover;
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var loverOneData = new CachedPlayerData(lover.Player.Data);
                        var loverTwoData = new CachedPlayerData(otherLover.Player.Data);
                        if (PlayerControl.LocalPlayer != lover.Player) loverOneData.IsYou = false;
                        if (PlayerControl.LocalPlayer != otherLover.Player) loverTwoData.IsYou = false;
                        EndGameResult.CachedWinners.Add(loverOneData);
                        EndGameResult.CachedWinners.Add(loverTwoData);
                        return;
                    }
                }
            }

            if (Role.VampireWins)
            {
                EndGameResult.CachedWinners = new List<CachedPlayerData>();
                foreach (var role in Role.GetRoles(RoleEnum.Vampire))
                {
                    var vamp = (Vampire)role;
                    var vampData = new CachedPlayerData(vamp.Player.Data);
                    if (PlayerControl.LocalPlayer != vamp.Player) vampData.IsYou = false;
                    EndGameResult.CachedWinners.Add(vampData);
                }
            }

            foreach (var role in Role.AllRoles)
            {
                var type = role.RoleType;

                if (type == RoleEnum.Glitch)
                {
                    var glitch = (Glitch)role;
                    if (glitch.GlitchWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var glitchData = new CachedPlayerData(glitch.Player.Data);
                        if (PlayerControl.LocalPlayer != glitch.Player) glitchData.IsYou = false;
                        EndGameResult.CachedWinners.Add(glitchData);
                    }
                }
                else if (type == RoleEnum.Juggernaut)
                {
                    var juggernaut = (Juggernaut)role;
                    if (juggernaut.JuggernautWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var juggData = new CachedPlayerData(juggernaut.Player.Data);
                        if (PlayerControl.LocalPlayer != juggernaut.Player) juggData.IsYou = false;
                        EndGameResult.CachedWinners.Add(juggData);
                    }
                }
                else if (type == RoleEnum.Arsonist)
                {
                    var arsonist = (Arsonist)role;
                    if (arsonist.ArsonistWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var arsonistData = new CachedPlayerData(arsonist.Player.Data);
                        if (PlayerControl.LocalPlayer != arsonist.Player) arsonistData.IsYou = false;
                        EndGameResult.CachedWinners.Add(arsonistData);
                    }
                }
                else if (type == RoleEnum.Plaguebearer)
                {
                    var plaguebearer = (Plaguebearer)role;
                    if (plaguebearer.PlaguebearerWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var pbData = new CachedPlayerData(plaguebearer.Player.Data);
                        if (PlayerControl.LocalPlayer != plaguebearer.Player) pbData.IsYou = false;
                        EndGameResult.CachedWinners.Add(pbData);
                    }
                }
                else if (type == RoleEnum.Pestilence)
                {
                    var pestilence = (Pestilence)role;
                    if (pestilence.PestilenceWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var pestilenceData = new CachedPlayerData(pestilence.Player.Data);
                        if (PlayerControl.LocalPlayer != pestilence.Player) pestilenceData.IsYou = false;
                        EndGameResult.CachedWinners.Add(pestilenceData);
                    }
                }
                else if (type == RoleEnum.Werewolf)
                {
                    var werewolf = (Werewolf)role;
                    if (werewolf.WerewolfWins)
                    {
                        EndGameResult.CachedWinners = new List<CachedPlayerData>();
                        var werewolfData = new CachedPlayerData(werewolf.Player.Data);
                        if (PlayerControl.LocalPlayer != werewolf.Player) werewolfData.IsYou = false;
                        EndGameResult.CachedWinners.Add(werewolfData);
                    }
                }
            }

            foreach (var role in Role.GetRoles(RoleEnum.Survivor))
            {
                var surv = (Survivor)role;
                if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                {
                    var isImp = EndGameResult.CachedWinners.Count != 0 && EndGameResult.CachedWinners[0].IsImpostor;
                    var survWinData = new CachedPlayerData(surv.Player.Data);
                    if (isImp) survWinData.IsImpostor = true;
                    if (PlayerControl.LocalPlayer != surv.Player) survWinData.IsYou = false;
                    EndGameResult.CachedWinners.Add(survWinData);
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                var gaTargetData = new CachedPlayerData(ga.target.Data);
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.ToArray())
                {
                    if (gaTargetData.ColorId == winner.ColorId)
                    {
                        var isImp = EndGameResult.CachedWinners[0].IsImpostor;
                        var gaWinData = new CachedPlayerData(ga.Player.Data);
                        if (isImp) gaWinData.IsImpostor = true;
                        if (PlayerControl.LocalPlayer != ga.Player) gaWinData.IsYou = false;
                        EndGameResult.CachedWinners.Add(gaWinData);
                    }
                }
            }
        }
    }
}

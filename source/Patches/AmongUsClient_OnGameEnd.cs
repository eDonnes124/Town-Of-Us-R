using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Extensions;

namespace TownOfUs
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class EndGameManager_SetEverythingUp
    {
        public static void Prefix()
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
            foreach (var role in Role.GetRoles(RoleEnum.Lawyer))
            {
                var lwyr = (Lawyer)role;
                losers.Add(lwyr.Player.GetDefaultOutfit().ColorId);
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

            var toRemoveWinners = TempData.winners.ToArray().Where(o => losers.Contains(o.ColorId)).ToArray();
            for (int i = 0; i < toRemoveWinners.Count(); i++) TempData.winners.Remove(toRemoveWinners[i]);

            if (Role.NobodyWins)
            {
                TempData.winners = new List<WinningPlayerData>();
                return;
            }
            if (Role.SurvOnlyWins)
            {
                TempData.winners = new List<WinningPlayerData>();
                foreach (var role in Role.GetRoles(RoleEnum.Survivor))
                {
                    var surv = (Survivor)role;
                    if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                    {
                        var survData = new WinningPlayerData(surv.Player.Data);
                        if (PlayerControl.LocalPlayer != surv.Player) survData.IsYou = false;
                        TempData.winners.Add(new WinningPlayerData(surv.Player.Data));
                    }
                }

                return;
            }

            if (CustomGameOptions.NeutralEvilWinEndsGame)
            {
                foreach (var role in Role.AllRoles)
                {
                    var type = role.RoleType;

                    if (type == RoleEnum.Jester)
                    {
                        var jester = (Jester)role;
                        if (jester.VotedOut)
                        {
                            TempData.winners = new List<WinningPlayerData>();
                            var jestData = new WinningPlayerData(jester.Player.Data);
                            jestData.IsDead = false;
                            if (PlayerControl.LocalPlayer != jester.Player) jestData.IsYou = false;
                            TempData.winners.Add(jestData);
                            return;
                        }
                    }
                    else if (type == RoleEnum.Executioner)
                    {
                        var executioner = (Executioner)role;
                        if (executioner.TargetVotedOut)
                        {
                            TempData.winners = new List<WinningPlayerData>();
                            var exeData = new WinningPlayerData(executioner.Player.Data);
                            if (PlayerControl.LocalPlayer != executioner.Player) exeData.IsYou = false;
                            TempData.winners.Add(exeData);
                            return;
                        }
                    }
                    else if (type == RoleEnum.Doomsayer)
                    {
                        var doom = (Doomsayer)role;
                        if (doom.WonByGuessing)
                        {
                            TempData.winners = new List<WinningPlayerData>();
                            var doomData = new WinningPlayerData(doom.Player.Data);
                            if (PlayerControl.LocalPlayer != doom.Player) doomData.IsYou = false;
                            TempData.winners.Add(doomData);
                            return;
                        }
                    }
                    else if (type == RoleEnum.Phantom)
                    {
                        var phantom = (Phantom)role;
                        if (phantom.CompletedTasks)
                        {
                            TempData.winners = new List<WinningPlayerData>();
                            var phantomData = new WinningPlayerData(phantom.Player.Data);
                            if (PlayerControl.LocalPlayer != phantom.Player) phantomData.IsYou = false;
                            TempData.winners.Add(phantomData);
                            return;
                        }
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
                        TempData.winners = new List<WinningPlayerData>();
                        var loverOneData = new WinningPlayerData(lover.Player.Data);
                        var loverTwoData = new WinningPlayerData(otherLover.Player.Data);
                        if (PlayerControl.LocalPlayer != lover.Player) loverOneData.IsYou = false;
                        if (PlayerControl.LocalPlayer != otherLover.Player) loverTwoData.IsYou = false;
                        TempData.winners.Add(loverOneData);
                        TempData.winners.Add(loverTwoData);
                        return;
                    }
                }
            }

            if (Role.VampireWins)
            {
                TempData.winners = new List<WinningPlayerData>();
                foreach (var role in Role.GetRoles(RoleEnum.Vampire))
                {
                    var vamp = (Vampire)role;
                    var vampData = new WinningPlayerData(vamp.Player.Data);
                    if (PlayerControl.LocalPlayer != vamp.Player) vampData.IsYou = false;
                    TempData.winners.Add(vampData);
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
                        TempData.winners = new List<WinningPlayerData>();
                        var glitchData = new WinningPlayerData(glitch.Player.Data);
                        if (PlayerControl.LocalPlayer != glitch.Player) glitchData.IsYou = false;
                        TempData.winners.Add(glitchData);
                    }
                }
                else if (type == RoleEnum.Juggernaut)
                {
                    var juggernaut = (Juggernaut)role;
                    if (juggernaut.JuggernautWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        var juggData = new WinningPlayerData(juggernaut.Player.Data);
                        if (PlayerControl.LocalPlayer != juggernaut.Player) juggData.IsYou = false;
                        TempData.winners.Add(juggData);
                    }
                }
                else if (type == RoleEnum.Arsonist)
                {
                    var arsonist = (Arsonist)role;
                    if (arsonist.ArsonistWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        var arsonistData = new WinningPlayerData(arsonist.Player.Data);
                        if (PlayerControl.LocalPlayer != arsonist.Player) arsonistData.IsYou = false;
                        TempData.winners.Add(arsonistData);
                    }
                }
                else if (type == RoleEnum.Plaguebearer)
                {
                    var plaguebearer = (Plaguebearer)role;
                    if (plaguebearer.PlaguebearerWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        var pbData = new WinningPlayerData(plaguebearer.Player.Data);
                        if (PlayerControl.LocalPlayer != plaguebearer.Player) pbData.IsYou = false;
                        TempData.winners.Add(pbData);
                    }
                }
                else if (type == RoleEnum.Pestilence)
                {
                    var pestilence = (Pestilence)role;
                    if (pestilence.PestilenceWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        var pestilenceData = new WinningPlayerData(pestilence.Player.Data);
                        if (PlayerControl.LocalPlayer != pestilence.Player) pestilenceData.IsYou = false;
                        TempData.winners.Add(pestilenceData);
                    }
                }
                else if (type == RoleEnum.Werewolf)
                {
                    var werewolf = (Werewolf)role;
                    if (werewolf.WerewolfWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        var werewolfData = new WinningPlayerData(werewolf.Player.Data);
                        if (PlayerControl.LocalPlayer != werewolf.Player) werewolfData.IsYou = false;
                        TempData.winners.Add(werewolfData);
                    }
                }
            }

            foreach (var role in Role.GetRoles(RoleEnum.Survivor))
            {
                var surv = (Survivor)role;
                if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                {
                    var isImp = TempData.winners.Count != 0 && TempData.winners[0].IsImpostor;
                    var survWinData = new WinningPlayerData(surv.Player.Data);
                    if (isImp) survWinData.IsImpostor = true;
                    if (PlayerControl.LocalPlayer != surv.Player) survWinData.IsYou = false;
                    TempData.winners.Add(survWinData);
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.Lawyer))
            {
                var lwyr = (Lawyer)role;
                if (!lwyr.TargetVotedOut && !lwyr.Player.Data.IsDead)
                {
                    var isImp = TempData.winners[0].IsImpostor;
                    var lwyrWinData = new WinningPlayerData(lwyr.Player.Data);
                    if (isImp) lwyrWinData.IsImpostor = true;
                    if (PlayerControl.LocalPlayer != lwyr.Player) lwyrWinData.IsYou = false;
                    TempData.winners.Add(lwyrWinData);
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                var gaTargetData = new WinningPlayerData(ga.target.Data);
                foreach (WinningPlayerData winner in TempData.winners.ToArray())
                {
                    if (gaTargetData.ColorId == winner.ColorId)
                    {
                        var isImp = TempData.winners[0].IsImpostor;
                        var gaWinData = new WinningPlayerData(ga.Player.Data);
                        if (isImp) gaWinData.IsImpostor = true;
                        if (PlayerControl.LocalPlayer != ga.Player) gaWinData.IsYou = false;
                        TempData.winners.Add(gaWinData);
                    }
                }
            }
        }
    }
}

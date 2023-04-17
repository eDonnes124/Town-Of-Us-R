using System;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            if (br.KillAge > CustomGameOptions.DetectiveFactionDuration * 1000)
                return
                    $"Body Report: The corpse is too old to gain information from. (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return
                    $"Body Report: The kill appears to have been a suicide! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            var role = Role.GetRole(br.Killer);

            if (br.KillAge < CustomGameOptions.DetectiveRoleDuration * 1000)
                return
                    $"Body Report: The killer appears to be a {role.Name}! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.Is(Faction.Crewmates))
                return
                    $"Body Report: The killer appears to be a Crewmate! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            else if (br.Killer.Is(Faction.NeutralKilling) || br.Killer.Is(Faction.NeutralOther))
                return
                    $"Body Report: The killer appears to be a Neutral Role! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            else
                return
                    $"Body Report: The killer appears to be an Impostor! (Killed {Math.Round(br.KillAge / 1000)}s ago)";
        }
        public static string PlayerReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Glitch) || player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                 || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Spy))
                return "Your target views the world through a different lens";
            else if (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker))
                return "Your target has an unusual obsession with dead bodies";
            else if (player.Is(RoleEnum.Grenadier) || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic)
                 || player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Veteran))
                return "Your target tries to protect themselves or others by any means necessary";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester)
                 || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Transporter))
                return "Your target is a causer of chaos";
            else if (player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Mayor) || player.Is(RoleEnum.Snitch)
                 || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Trapper))
                return "Your target is concealing information";
            else if (player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence) || player.Is(RoleEnum.Sheriff)
                 || player.Is(RoleEnum.Traitor) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Werewolf))
                return "Your target started innocent but gained the capability to kill";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Detective) || player.Is(RoleEnum.Plaguebearer)
                 || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Tracker))
                return "Your target likes to interact with others";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Investigator)
                 || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Mystic))
                return "Your target likes exploring";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return "Your target appears to be roleless";
            else
                return "Error";
        }
    }
}

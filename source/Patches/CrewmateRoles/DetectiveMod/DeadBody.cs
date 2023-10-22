using System;
using TownOfUs.Patches.Localization;
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
                    $"{string.Format(LocalizationManager.Instance.GetString("BodyReportTooOld"), Math.Round(br.KillAge / 1000))}";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return
                    $"{string.Format(LocalizationManager.Instance.GetString("BodyReportSuicide"), Math.Round(br.KillAge / 1000))}";

            var role = Role.GetRole(br.Killer);

            if (br.KillAge < CustomGameOptions.DetectiveRoleDuration * 1000)
                return
                    $"{string.Format(LocalizationManager.Instance.GetString("BodyReportRole"), role.Name, Math.Round(br.KillAge / 1000))}";

            if (br.Killer.Is(Faction.Crewmates))
                return
                    $"{string.Format(LocalizationManager.Instance.GetString("BodyReportCrewmate"), Math.Round(br.KillAge / 1000))}";

            else if (br.Killer.Is(Faction.NeutralKilling) || br.Killer.Is(Faction.NeutralBenign))
                return
                    $"{string.Format(LocalizationManager.Instance.GetString("BodyReportNeutralRole"), Math.Round(br.KillAge / 1000))}";

            else
                return
                    $"{string.Format(LocalizationManager.Instance.GetString("BodyReportImpostor"), Math.Round(br.KillAge / 1000))}";
        }
    }
}

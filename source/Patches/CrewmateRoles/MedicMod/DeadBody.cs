using System;
using System.Collections.Generic;
using TownOfUs.Extensions;
using TownOfUs.Patches.Localization;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    public class DeadPlayer
    {
        public byte KillerId { get; set; }
        public byte PlayerId { get; set; }
        public DateTime KillTime { get; set; }
    }

    //body report class for when medic reports a body
    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            //System.Console.WriteLine(br.KillAge);
            if (br.KillAge > CustomGameOptions.MedicReportColorDuration * 1000)
                return
                    $"{string.Format(LocalizationManager.Instance.GetString("BodyReportTooOld"), Math.Round(br.KillAge / 1000))}";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return
                    $"{string.Format(LocalizationManager.Instance.GetString("BodyReportSuicide"), Math.Round(br.KillAge / 1000))}";

            if (br.KillAge < CustomGameOptions.MedicReportNameDuration * 1000)
                return
                    $"{string.Format(LocalizationManager.Instance.GetString("BodyReportKillerAppearance"), br.Killer.Data.PlayerName, Math.Round(br.KillAge / 1000))}";

            var colors = new Dictionary<int, string>
            {
                {0, "darker"},// red
                {1, "darker"},// blue
                {2, "darker"},// green
                {3, "lighter"},// pink
                {4, "lighter"},// orange
                {5, "lighter"},// yellow
                {6, "darker"},// black
                {7, "lighter"},// white
                {8, "darker"},// purple
                {9, "darker"},// brown
                {10, "lighter"},// cyan
                {11, "lighter"},// lime
                {12, "darker"},// maroon
                {13, "lighter"},// rose
                {14, "lighter"},// banana
                {15, "darker"},// gray
                {16, "darker"},// tan
                {17, "lighter"},// coral
                {18, "darker"},// watermelon
                {19, "darker"},// chocolate
                {20, "lighter"},// sky blue
                {21, "lighter"},// beige
                {22, "darker"},// magenta
                {23, "lighter"},// turquoise
                {24, "lighter"},// lilac
                {25, "darker"},// olive
                {26, "lighter"},// azure
                {27, "darker"},// plum
                {28, "darker"},// jungle
                {29, "lighter"},// mint
                {30, "lighter"},// chartreuse
                {31, "darker"},// macau
                {32, "darker"},// tawny
                {33, "lighter"},// gold
                {34, "lighter"},// rainbow
            };
            var typeOfColor = colors[br.Killer.GetDefaultOutfit().ColorId];
            return
                $"{string.Format(LocalizationManager.Instance.GetString("BodyReportKillerAppearance"), typeOfColor, Math.Round(br.KillAge / 1000))}";
        }
    }
}

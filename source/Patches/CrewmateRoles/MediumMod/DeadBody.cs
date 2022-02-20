using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.MediumMod
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
            try {
                int randomNumber = Random.RandomRangeInt(0, 5);
                if (randomNumber == 0) {
                    if (br.Body == br.Killer) {
                        return $"Ghost: I shot myself!";
                    } else {
                        //If this number is rolled, and they didn't suicide, roll again
                        randomNumber = Random.RandomRangeInt(1, 5);
                    }
                }
                if (randomNumber == 1) {
                    return $"Ghost: My role was {Role.GetRole(br.Body).Name}!";
                } else if (randomNumber == 2) {
                    return $"Ghost: I was killed {Math.Round(br.KillAge / 1000)}s ago!";
                } else if (randomNumber == 3) {
                    return
                        $"Ghost: My killers role was {Role.GetRole(br.Killer).Name}";
                } else if (randomNumber == 4) {
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
                        {21, "darker"},// beige
                        {22, "lighter"},// hot pink
                        {23, "lighter"},// turquoise
                        {24, "lighter"},// lilac
                        {25, "lighter"},// rainbow
                        {26, "lighter"},// azure
                    };
                    return $"Ghost: My killer was a {colors[br.Killer.CurrentOutfit.ColorId]} color!";
                } else {
                    return "Ghost: I have no idea what happened!";
                }
            } catch {
                return "Ghost: I have no idea what happened!";
            }
        }
    }
}

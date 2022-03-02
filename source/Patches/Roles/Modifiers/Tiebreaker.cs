using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Tiebreaker : Modifier
    {
        public Tiebreaker(PlayerControl player) : base(player)
        {
            Name = "Tiebreaker";
            TaskText = () => "Your vote breaks ties";
            Color = Patches.Colors.Tiebreaker;
            ModifierType = ModifierEnum.Tiebreaker;
        }
    }
}
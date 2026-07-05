using UnityEngine;

namespace TownOfUs.Modifiers
{
    public class Immovable : Modifier
    {
        public Vector3 Location = Vector3.zero;
        public Immovable(PlayerControl player) : base(player)
        {
            Name = "Immovable";
            TaskText = () => "Stay put";
            Color = Patches.Colors.Immovable;
            ModifierType = ModifierEnum.Immovable;
        }
    }
}
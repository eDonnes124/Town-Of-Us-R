using System;

namespace TownOfUs.Modifiers
{
    public class Shy : Modifier
    {
        public bool Moving = false;
        public DateTime LastMoved { get; set; }
        public float Opacity = 1;
        public Shy(PlayerControl player) : base(player)
        {
            Name = "Shy";
            TaskText = () => "Stand still to hide";
            Color = Patches.Colors.Shy;
            ModifierType = ModifierEnum.Shy;
            LastMoved = DateTime.UtcNow;
        }
    }
}
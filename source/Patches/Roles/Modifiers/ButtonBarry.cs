using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class ButtonBarry : Modifier
    {
        public KillButton ButtonButton;

        public bool ButtonUsed;

        public ButtonBarry(PlayerControl player) : base(player)
        {
            Name = "Button Barry";
            TaskText = () => "Call a button from anywhere!";
            Color = Patches.Colors.ButtonBarry;
            ModifierType = ModifierEnum.ButtonBarry;
        }
    }
}
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Flash : Modifier, IVisualAlteration
    {
        public static float SpeedFactor = 1.25f;

        public Flash(PlayerControl player) : base(player)
        {
            Name = "Flash";
            TaskText = () => "Superspeed!";
            Color = Patches.Colors.Flash;
            ModifierType = ModifierEnum.Flash;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = SpeedFactor;
            return true;
        }
    }
}
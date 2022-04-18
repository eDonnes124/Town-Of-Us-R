using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    class Horse : Modifier, IVisualAlteration
    {
        public static float SpeedFactor = 1.2f;
        public Horse(PlayerControl player) : base(player)
        {
            Name = "Horse";
            TaskText = () => "You are horse";
            Color = Patches.Colors.Horse;
            ModifierType = ModifierEnum.Horse;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = SpeedFactor;
            appearance.IsHorse = true;
            return true;
        }
    }
}

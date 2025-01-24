using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Mini : Modifier, IVisualAlteration
    {
        public Mini(PlayerControl player) : base(player)
        {
            Name = "Mini";
            TaskText = () => "You are tiny!";
            Color = Patches.Colors.Mini;
            ModifierType = ModifierEnum.Mini;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SizeFactor = new Vector3(0.4f, 0.4f, 1.0f);
            return true;
        }
    }
}
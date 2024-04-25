using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class UpsideDown : Modifier, IVisualAlteration
    {
        public UpsideDown(PlayerControl player) : base(player)
        {
            Name = "Upside-down";
            TaskText = () => "umop episdn era noY";
            Color = Patches.Colors.Mini;
            ModifierType = ModifierEnum.UpsideDown;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SizeFactor = new Vector3(appearance.SizeFactor.x, -appearance.SizeFactor.y, appearance.SizeFactor.z);

            if (Player == PlayerControl.LocalPlayer)
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 180);
            
            return true;
        }
    }
}

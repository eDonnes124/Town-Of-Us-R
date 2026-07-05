using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Modifiers
{
    public class Mini : Modifier, IVisualAlteration
    {
        public Mini(PlayerControl player) : base(player)
        {
            Name = "Mini";
            TaskText = () => "You are tiny!";
            Color = Patches.Colors.Mini;
            ModifierType = ModifierEnum.Mini;

            if (Player == PlayerControl.LocalPlayer && GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4)
            {
                Console download = Object.FindObjectsOfType<Console>().ToList()
                    .Find(console => console.name == "panel_data" && console.Room == SystemTypes.Brig);
                download.transform.localPosition = new Vector3(5.6f, 9.49f, 0.1f);
            }
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SizeFactor = new Vector3(0.4f, 0.4f, 1.0f);
            return true;
        }
    }
}
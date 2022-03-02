using UnityEngine;
using System.Collections.Generic;

namespace TownOfUs.Roles.Modifiers
{
    public class Sleuth : Modifier
    {
        public List<byte> Reported = new List<byte>();
        public Sleuth(PlayerControl player) : base(player)
        {
            Name = "Sleuth";
            TaskText = () => "Know the roles of bodies you report";
            Color = Patches.Colors.Sleuth;
            ModifierType = ModifierEnum.Sleuth;
        }
    }
}
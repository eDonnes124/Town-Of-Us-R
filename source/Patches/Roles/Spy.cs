using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Spy : Role
    {
        public Dictionary<byte, TMP_Text> PlayerNumbers = new Dictionary<byte, TMP_Text>();
        public Spy(PlayerControl player) : base(player)
        {
            Name = "Spy";
            ImpostorText = () => "Snoop around and find stuff out";
            TaskText = () => "Spy on people and find the Impostors";
            Color = Patches.Colors.Spy;
            RoleType = RoleEnum.Spy;
        }
    }
}
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Jailor : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Jailed;
        public bool CanJail;
        public GameObject ExecuteButton = new GameObject();
        public GameObject JailCell = new GameObject();
        public TMP_Text UsesText = new TMP_Text();
        public int Executes { get; set; }

        public Jailor(PlayerControl player) : base(player)
        {
            Name = "Jailor";
            ImpostorText = () => "Jail And Execute The <color=#FF0000FF>Impostors</color>";
            TaskText = () => "Execute evildoers but not Crewmates";
            Color = Patches.Colors.Jailor;
            LastJailed = DateTime.UtcNow;
            RoleType = RoleEnum.Jailor;
            AddToRoleHistory(RoleType);
            Executes = CustomGameOptions.MaxExecutes;
            CanJail = true;
        }

        public DateTime LastJailed { get; set; }

        public float JailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastJailed;
            var num = CustomGameOptions.JailCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.CrewKillersContinue) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()) > 0 && Executes > 0) return false;

            return true;
        }
    }
}
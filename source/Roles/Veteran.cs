using System;
using UnityEngine;
using TMPro;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Veteran : Role
    {
        public bool Enabled;
        public DateTime LastAlerted;
        public float TimeRemaining;

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public Veteran(PlayerControl player) : base(player)
        {
            Name = "Veteran";
            ImpostorText = () => "Alert To Kill Anyone Who Interacts With You";
            TaskText = () => "Alert to kill whoever interacts with you";
            Color = Patches.Colors.Veteran;
            LastAlerted = DateTime.UtcNow;
            RoleType = RoleEnum.Veteran;
            AddToRoleHistory(RoleType);

            UsesLeft = CustomGameOptions.MaxAlerts;
        }

        public bool OnAlert => TimeRemaining > 0f;

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.CrewKillersContinue) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()) > 0 && (UsesLeft > 0 || Enabled)) return false;

            return true;
        }

        public float AlertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAlerted;
            var num = CustomGameOptions.AlertCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void UnAlert()
        {
            Enabled = false;
            LastAlerted = DateTime.UtcNow;
        }
    }
}
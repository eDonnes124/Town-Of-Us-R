using Reactor.Utilities;
using System;
using TownOfUs.Patches;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Escapist : Role
    {
        public KillButton _escapeButton;
        public DateTime LastEscape;
        public Vector3 EscapePoint = new();

        public Escapist(PlayerControl player) : base(player)
        {
            Name = "Escapist";
            ImpostorText = () => "Get Away From Kills With Ease";
            TaskText = () => "Teleport to get away from bodies";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Escapist;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public KillButton EscapeButton
        {
            get => _escapeButton;
            set
            {
                _escapeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float EscapeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastEscape;
            var num = CustomGameOptions.EscapeCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public static void Escape(PlayerControl escapist)
        {
            escapist.MyPhysics.ResetMoveState();
            var escapistRole = Role.GetRole<Escapist>(escapist);
            if (escapistRole.EscapePoint == Vector3.zero) return;
            var position = escapistRole.EscapePoint;
            escapist.NetTransform.SnapTo(new Vector2(position.x, position.y));

            if (SubmergedCompatibility.isSubmerged())
            {
                if (PlayerControl.LocalPlayer.PlayerId == escapist.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(escapist.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }

            if (PlayerControl.LocalPlayer.PlayerId == escapist.PlayerId)
            {
                Coroutines.Start(Utils.FlashCoroutine(new Color(0.6f, 0.1f, 0.2f, 1f)));
                if (Minigame.Instance) Minigame.Instance.Close();
            }

            escapist.moveable = true;
            escapist.Collider.enabled = true;
            escapist.NetTransform.enabled = true;
        }
    }
}
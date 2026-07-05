using System;

namespace TownOfUs.Roles
{
    public class Undertaker : Role
    {
        public KillButton _dragDropButton;

        public Undertaker(PlayerControl player) : base(player)
        {
            Name = "Undertaker";
            ImpostorText = () => "Drag Bodies And Hide Them";
            TaskText = () => "Drag bodies around to hide them from being reported";
            Color = Patches.Colors.Impostor;
            LastDragged = DateTime.UtcNow;
            RoleType = RoleEnum.Undertaker;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public DateTime LastDragged { get; set; }
        public DeadBody CurrentTarget { get; set; }
        public DeadBody CurrentlyDragging { get; set; }

        public KillButton DragDropButton
        {
            get => _dragDropButton;
            set
            {
                _dragDropButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDragged;
            var num = CustomGameOptions.DragCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
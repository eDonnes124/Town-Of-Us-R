using System;

namespace TownOfUs.Roles
{
    public class Blackmailer : Role, IExtraButton
    {
        public KillButton RoleAbilityButton { get; set; }
        
        public PlayerControl ClosestPlayer;
        public PlayerControl Blackmailed;
        public DateTime LastBlackmailed { get; set; }

        public Blackmailer(PlayerControl player) : base(player)
        {
            Name = "Blackmailer";
            ImpostorText = () => "Silence Crewmates During Meetings";
            TaskText = () => "Silence a crewmate for the next meeting";
            Color = Patches.Colors.Impostor;
            LastBlackmailed = DateTime.UtcNow;
            RoleType = RoleEnum.Blackmailer;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlackmailed;
            var num = CustomGameOptions.BlackmailCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
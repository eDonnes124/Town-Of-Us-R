using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Lawyer : Role
    {
        public PlayerControl target;
        public bool TargetVotedOut;

        public Lawyer(PlayerControl player) : base(player)
        {
            Name = "Lawyer";
            ImpostorText = () =>
                target == null ? "You don't have a target for some reason... weird..." : $"Keep {target.name} From Getting Voted Out";
            TaskText = () =>
                target == null
                    ? "You don't have a target for some reason... weird..."
                    : $"Prevent {target.name} from getting voted out!";
            Color = Patches.Colors.Lawyer;
            RoleType = RoleEnum.Lawyer;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralBenign;
            Scale = 1.4f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            var lwyrTeam = new List<PlayerControl>();
            lwyrTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = lwyrTeam;
        }
    }
}
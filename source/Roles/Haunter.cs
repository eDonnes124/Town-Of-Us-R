using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Haunter : GhostRole
    {
        public bool Revealed => TasksLeft <= CustomGameOptions.HaunterTasksRemainingAlert;

        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();

        public List<PlayerControl> HaunterTargets = new List<PlayerControl>();

        public List<ArrowBehaviour> HaunterArrows = new List<ArrowBehaviour>();

        public Haunter(PlayerControl player) : base(player)
        {
            Name = "Haunter";
            ImpostorText = () => "";
            TaskText = () => "Complete all your tasks to reveal Impostors!";
            Color = Patches.Colors.Haunter;
            RoleType = RoleEnum.Haunter;
            AddToRoleHistory(RoleType);
        }
    }
}
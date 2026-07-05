namespace TownOfUs.Roles
{
    public class Phantom : GhostRole
    {
        public Phantom(PlayerControl player) : base(player)
        {
            Name = "Phantom";
            ImpostorText = () => "";
            TaskText = () => "Complete all your tasks without being caught!";
            Color = Patches.Colors.Phantom;
            RoleType = RoleEnum.Phantom;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }
    }
}
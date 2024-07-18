namespace TownOfUs.Roles
{
    public class Traitor : Role
    {
        public RoleEnum formerRole = new RoleEnum();
        public Traitor(PlayerControl player) : base(player)
        {
            Name = "Traitor";
            ImpostorText = () => "";
            TaskText = () => "Betray the Crewmates!";
            Color = Patches.Colors.Traitor;
            RoleType = RoleEnum.Traitor;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
    }
}
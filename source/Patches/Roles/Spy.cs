namespace TownOfUs.Roles
{
    public class Spy : Role
    {
        public Spy(PlayerControl player) : base(player)
        {
            Name = "Spy";
            ImpostorText = () => "Snoop Around And Find Stuff Out";
            TaskText = () => "Gain extra information from all devices";
            Color = Patches.Colors.Spy;
            RoleType = RoleEnum.Spy;
            AddToRoleHistory(RoleType);
        }
    }
}
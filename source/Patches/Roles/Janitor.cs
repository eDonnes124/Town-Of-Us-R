namespace TownOfUs.Roles
{
    public class Janitor : Role, ITargetsDeadBody, IExtraButton
    {
        public KillButton RoleAbilityButton { get; set; }
        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            ImpostorText = () => "Clean Up Bodies";
            TaskText = () => "Clean bodies to prevent Crewmates from discovering them";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Janitor;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public DeadBody CurrentTarget { get; set; }
    }
}
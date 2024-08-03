namespace TownOfUs.Roles.Modifiers
{
    public class DoubleShot : Modifier
    {
        public bool LifeUsed;
        public DoubleShot(PlayerControl player) : base(player)
        {
            Name = "Double Shot";
            TaskText = () => "You have an extra life when assassinating";
            Color = Patches.Colors.DoubleShot;
            ModifierType = ModifierEnum.DoubleShot;
            LifeUsed = false;
        }
    }
}
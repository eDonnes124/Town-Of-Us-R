namespace TownOfUs.Modifiers
{
    public class Taskmaster : Modifier
    {
        public Taskmaster(PlayerControl player) : base(player)
        {
            Name = "Taskmaster";
            TaskText = () => "Get tasks done fast";
            Color = Patches.Colors.Taskmaster;
            ModifierType = ModifierEnum.Taskmaster;
        }
    }
}
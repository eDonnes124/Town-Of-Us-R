namespace TownOfUs.Modifiers
{
    public class SixthSense : Modifier
    {
        public SixthSense(PlayerControl player) : base(player)
        {
            Name = "Sixth Sense";
            TaskText = () => "Know when someone interacts with you";
            Color = Patches.Colors.SixthSense;
            ModifierType = ModifierEnum.SixthSense;
        }
    }
}
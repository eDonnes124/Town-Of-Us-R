namespace TownOfUs.Roles.Modifiers
{
    public class Saboteur : Modifier
    {
        public float Timer = 0f;
        public Saboteur(PlayerControl player) : base(player)
        {
            Name = "Saboteur";
            TaskText = () => "You have reduced sabotage cooldowns";
            Color = Patches.Colors.Impostor;
            ModifierType = ModifierEnum.Saboteur;
        }
    }
}
namespace TownOfUs.CustomOption
{
    public class CustomHeaderOption : CustomOption
    {
        protected internal CustomHeaderOption(int id, MultiMenu menu, string name, CustomOption dependsOn = null, int? stringoptionneedstobe = null) : base(id, menu, name, CustomOptionType.Header, 0, dependsOn: dependsOn, stringoptionneedstobe: stringoptionneedstobe)
        {
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
        }
    }
}
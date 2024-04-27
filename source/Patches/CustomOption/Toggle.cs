namespace TownOfUs.CustomOption
{
    public class CustomToggleOption : CustomOption
    {
        protected internal CustomToggleOption(int id, MultiMenu menu, string name, bool value = true, CustomOption dependsOn = null, int? stringoptionneedstobe = null) : base(id, menu, name,
            CustomOptionType.Toggle, value, dependsOn: dependsOn, stringoptionneedstobe: stringoptionneedstobe)
        {
            Format = val => (bool) val ? "On" : "Off";
        }

        protected internal bool Get()
        {
            return (bool) Value;
        }

        protected internal void Toggle()
        {
            Set(!Get());
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
            Setting.Cast<ToggleOption>().CheckMark.enabled = Get();
        }
    }
}
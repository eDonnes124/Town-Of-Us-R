namespace TownOfUs.CustomOption
{
    public class CustomStringOption : CustomOption
    {
        protected internal CustomStringOption(int id, MultiMenu menu, string name, string[] values) : base(id, menu, name,
            CustomOptionType.String,
            0)
        {
            Values = values;
            Format = value => Values[(int) value];
        }

        protected string[] Values { get; set; }

        protected internal int Get()
        {
            return (int) Value;
        }

        protected internal void Increase()
        {
            if (Get() >= Values.Length)
                Set(0);
            else
                Set(Get() + 1);
        }

        protected internal void Decrease()
        {
            if (Get() <= 0)
                Set(Values.Length - 1);
            else
                Set(Get() - 1);
        }

        public override void OptionCreated()
        {
            var str = Setting.Cast<StringOption>();

            str.TitleText.text = Name;
            str.Value = str.oldValue = Get();
            str.ValueText.text = ToString();
        }
    }
}
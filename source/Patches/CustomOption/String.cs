using System;

namespace TownOfUs.CustomOption
{
    public class CustomStringOption : CustomOption
    {
        protected internal CustomStringOption(int id, MultiMenu menu, string name, string[] values, int startingId = 0) : base(id, menu, name,
            CustomOptionType.String,
            startingId)
        {
            Values = values;
            Format = value => Values[(int)value];
        }

        protected string[] Values { get; set; }

        protected internal int Get()
        {
            return (int)Value;
        }

        protected internal void Increase()
        {
            if (Get() >= Values.Length - 1)
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
            base.OptionCreated();
            var str = Setting.Cast<StringOption>();
            str.Value = str.oldValue = Get();
            str.ValueText.text = ToString();
        }
    }
}
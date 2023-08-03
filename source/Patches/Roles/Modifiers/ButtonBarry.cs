using System;
using TMPro;

namespace TownOfUs.Roles.Modifiers
{
    public class ButtonBarry : Modifier, IExtraButton
    {
        public KillButton RoleAbilityButton { get; set; }

        public bool ButtonUsed;
        public DateTime StartingCooldown { get; set; }

        public ButtonBarry(PlayerControl player) : base(player)
        {
            Name = "Button Barry";
            TaskText = () => "Call a button from anywhere!";
            Color = Patches.Colors.ButtonBarry;
            StartingCooldown = DateTime.UtcNow;
            ModifierType = ModifierEnum.ButtonBarry;
        }
        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        void IExtraButton.SetupAndActive(IExtraButton role)
        {
            if (role.RoleAbilityButton == null)
            {
                role.RoleAbilityButton = UnityEngine.Object.Instantiate(HudManager.Instance.KillButton, HudManager.Instance.transform.parent);
                role.RoleAbilityButton.GetComponentsInChildren<TextMeshPro>()[0].text = string.Empty;
                role.RoleAbilityButton.graphic.enabled = true;
            }
            role.RoleAbilityButton.graphic.sprite = TownOfUs.ButtonSprite;

            role.RoleAbilityButton.gameObject.SetActive((HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && GameManager.Instance.GameHasStarted);
        }
    }
}
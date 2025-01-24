using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine.UI;

namespace TownOfUs.CrewmateRoles.DeputyMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    public static class RemoveButtons
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Deputy))
            {
                var dep = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                HideButtons(dep);
            }
        }

        public static void HideButtons(Deputy role)
        {
            foreach (var (_, button) in role.Buttons)
            {
                if (button == null) continue;
                button.SetActive(false);
                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            }
            role.Buttons.Clear();
        }
    }
}
using HarmonyLib;
using TownOfUs.Modifiers;

namespace TownOfUs.Modifiers.CelebrityMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Celebrity)) return;
            var celeb = Modifier.GetModifier<Celebrity>(PlayerControl.LocalPlayer);
            if (celeb.JustDied && !celeb.MessageShown) celeb.PrintMessage();
        }
    }
}

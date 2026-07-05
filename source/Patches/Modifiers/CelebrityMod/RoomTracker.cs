using HarmonyLib;
using TownOfUs.Modifiers;

namespace TownOfUs
{
    public class Reportmessage
    {
        [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.FixedUpdate))]
        public class Recordlocation
        {
            [HarmonyPostfix]
            public static void Postfix(RoomTracker __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Celebrity)) return;
                var celeb = Modifier.GetModifier<Celebrity>(PlayerControl.LocalPlayer);
                if (!PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.Disconnected)
                {
                    if (__instance.text.transform.localPosition.y != -3.25f) celeb.Room = __instance.text.text;
                    else celeb.Room = string.Empty;
                }
            }
        }
    }
}

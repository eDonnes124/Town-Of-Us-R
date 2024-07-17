using System.Drawing;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.Patches
{

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
  public static class MapBehaviourPatch
 {
   public static void Postfix(MapBehaviour __instance)
  {
    if (HudUpdate.Colorful) __instance.ColorControl.SetColor(Role.GetRole(PlayerControl.LocalPlayer).Color);
    else if (PlayerControl.LocalPlayer.Data.IsImpostor()) __instance.ColorControl.SetColor(Palette.ImpostorRed);
    else __instance.ColorControl.SetColor(Palette.Blue);
  }
 }
}
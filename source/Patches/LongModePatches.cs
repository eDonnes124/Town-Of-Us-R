using System;
using HarmonyLib;

namespace TownOfUs.Patches
{
  public static class EnableAprilFoolsMode
    {

      [HarmonyPatch(typeof(AprilFoolsMode), nameof(AprilFoolsMode.ShouldShowAprilFoolsToggle))]
      public static bool EnableAprilFools(out bool __result)
      {
       if (CustomGameOptions.EnableAprilFoolsMode)
       {
        __result = true;
         return false;
        }
         else
         {
           __result = false;
           return false;
         }
  
      }
    }
}   
            

using HarmonyLib;


namespace TownOfUs.Patches
{

    [HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
    public static class SplashLogoAnimatorPatch
   {
    public static void Prefix(SplashManager __instance)
    {
        
        {
            __instance.sceneChanger.AllowFinishLoadingScene();
            __instance.startedSceneLoad = true;

        }
    }
  } 
}
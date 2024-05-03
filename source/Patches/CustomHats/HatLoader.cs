using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using Reactor.Utilities;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace TownOfUs.Patches.CustomHats
{
    internal static class HatLoader
    {
        private static ManualLogSource Log => PluginSingleton<TownOfUs>.Instance.Log;
        private static Assembly Assembly => typeof(TownOfUs).Assembly;

        private static bool LoadedHats = false;

        internal static void LoadHatsRoutine()
        {
            if (LoadedHats || !HatManager.InstanceExists || HatManager.Instance.allHats.Count == 0) return;
            LoadedHats = true;
            Coroutines.Start(LoadHats());
        }

        internal static IEnumerator LoadHats()
        {
            
            try
            {
                var hatBehaviours = DiscoverHatBehaviours();

                var hatData = new List<HatData>();
                hatData.AddRange(HatManager.Instance.allHats);
                hatData.ForEach(x => x.StoreName = "Vanilla");

                var originalCount = HatManager.Instance.allHats.ToList().Count;
                hatBehaviours.Reverse();
                for (var i = 0; i < hatBehaviours.Count; i++)
                {
                    hatBehaviours[i].displayOrder = originalCount + i;
                    hatData.Add(hatBehaviours[i]);
                }
                HatManager.Instance.allHats = hatData.ToArray();
            }
            catch (Exception e)
            {
                Log.LogError($"Error while loading hats: {e.Message}\nStack: {e.StackTrace}");
            }
            yield return null;
        }

        private static List<HatData> DiscoverHatBehaviours()
        {
            var hatBehaviours = new List<HatData>();
            var path = TownOfUs.RuntimeLocation + "\\touhats.catalog";
            Addressables.AddResourceLocator(Addressables.LoadContentCatalog(path).WaitForCompletion());
            var all_hat_locations = Addressables.LoadResourceLocationsAsync("touhats").WaitForCompletion();
            var assets = Addressables.LoadAssetsAsync<HatData>(all_hat_locations, null, false).WaitForCompletion();
            var array = new Il2CppSystem.Collections.Generic.List<HatData>(assets.Pointer);
            hatBehaviours.AddRange(array.ToArray());
            return hatBehaviours;
        }
    }
}
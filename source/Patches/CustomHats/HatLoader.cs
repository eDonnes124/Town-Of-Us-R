using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx.Logging;
using System.Text.Json;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;
using System.Linq;

namespace TownOfUs.Patches.CustomHats
{
    internal static class HatLoader
    {
        private const string HAT_RESOURCE_NAMESPACE = "TownOfUs.Resources.Hats";
        private const string HAT_METADATA_JSON = "metadata.json";
        private const int HAT_ORDER_BASELINE = 99;

        private static ManualLogSource Log => PluginSingleton<TownOfUs>.Instance.Log;
        private static Assembly Assembly => typeof(TownOfUs).Assembly;

        private static bool LoadedHats = false;

        internal static void LoadHatsRoutine()
        {
            if (LoadedHats || !DestroyableSingleton<HatManager>.InstanceExists || DestroyableSingleton<HatManager>.Instance.allHats.Count == 0)
                return;
            LoadedHats = true;
            Coroutines.Start(LoadHats());
        }

        internal static IEnumerator LoadHats()
        {
            
            try
            {
                var hatJson = LoadJson();
                var hatBehaviours = DiscoverHatBehaviours(hatJson);

                var hatData = new List<HatData>();
                hatData.AddRange(DestroyableSingleton<HatManager>.Instance.allHats);
                hatData.ForEach((Action<HatData>)(x => x.StoreName = "Vanilla"));

                var originalCount = DestroyableSingleton<HatManager>.Instance.allHats.ToList().Count;
                hatBehaviours.Reverse();
                for (var i = 0; i < hatBehaviours.Count; i++)
                {
                    hatBehaviours[i].displayOrder = originalCount + i;
                    hatData.Add(hatBehaviours[i]);
                }
                DestroyableSingleton<HatManager>.Instance.allHats = hatData.ToArray();
            }
            catch (Exception e)
            {
                Log.LogError($"Error while loading hats: {e.Message}\nStack: {e.StackTrace}");
            }
            yield return null;
        }

        private static HatMetadataJson LoadJson()
        {
            var stream = Assembly.GetManifestResourceStream($"{HAT_RESOURCE_NAMESPACE}.{HAT_METADATA_JSON}");
            return JsonSerializer.Deserialize<HatMetadataJson>(Encoding.UTF8.GetString(stream.ReadFully()), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });
        }

        private static List<HatData> DiscoverHatBehaviours(HatMetadataJson metadata)
        {
            var hatBehaviours = new List<HatData>();

            foreach (var hatCredit in metadata.Credits)
            {
                try
                {
                    var stream = Assembly.GetManifestResourceStream($"{HAT_RESOURCE_NAMESPACE}.{hatCredit.Id}.png");
                    if (stream != null)
                    {
                        var hatBehaviour = GenerateHatBehaviour(hatCredit.Id, stream.ReadFully());
                        hatBehaviour.StoreName = hatCredit.Artist;
                        hatBehaviour.ProductId = hatCredit.Id;
                        hatBehaviour.name = hatCredit.Name;
                        hatBehaviour.Free = true;
                        hatBehaviours.Add(hatBehaviour);

                    }
                }
                catch (Exception e)
                {
                    Log.LogError(
                         $"Error loading hat {hatCredit.Id} in metadata file ({HAT_METADATA_JSON})");
                    Log.LogError($"{e.Message}\nStack:{e.StackTrace}");
                }
            }
            return hatBehaviours;
        }

        private static HatData GenerateHatBehaviour(string s, byte[] mainImg)
        {

            //TODO: Move to Graphics Utils class
            Sprite sprite;
            if (HatCache.hatViewDatas.ContainsKey(s))
            {
                sprite = HatCache.hatViewDatas[s];
            }
            else
            {
                var tex2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                TownOfUs.LoadImage(tex2D, mainImg, false);
                sprite = Sprite.Create(tex2D, new Rect(0.0f, 0.0f, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100);
                HatCache.hatViewDatas.Add(s, sprite);
            }

            var hat = ScriptableObject.CreateInstance<HatData>();

            hat.ChipOffset = new Vector2(-0.1f, 0.35f);
            hat.SpritePreview = sprite;
            hat.InFront = true;
            hat.NoBounce = true;

            return hat;
        }
    }
}
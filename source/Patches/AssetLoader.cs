using Il2CppInterop.Runtime;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TownOfUs.Patches
{
    public class AssetLoader
    {

        public AssetLoader() { Initialize(); }

        public void Initialize()
        {
            var arch = OperatingSystem.IsAndroid() ? "android" : "win";
            var bundle = $"tour-assets-{arch}.bundle";
            var b = loadBundle(bundle);
            bundles.Add(b.name, b);
            b.GetAllAssetNames().ToList().ForEach(y => {
                objectname_to_bundle.Add(ConvertToBaseName(y), bundle);
            });
        }

        private static AssetBundle loadBundle(string bundlename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream($"TownOfUs.Resources.{bundlename}");
            var assets = stream.ReadFully();
            return AssetBundle.LoadFromMemory(assets);
        }

        private string ConvertToBaseName(string name)
        {
            return name.Split('/').Last().Split('.').First();
        }

        private T LoadAsset<T>(AssetBundle assetBundle, string name) where T : UnityObject
        {
            var asset = assetBundle.LoadAsset(name, Il2CppType.Of<T>())?.Cast<T>().DontUnload();
            loadedObjects.Add(name, asset);
            return asset;
        }

        private Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
        private Dictionary<string, string> objectname_to_bundle = new Dictionary<string, string>();
        private Dictionary<string, UnityObject> loadedObjects = new Dictionary<string, UnityObject>();


        public T Get<T>(string name) where T : UnityObject
        {
            if (loadedObjects.TryGetValue(name, out var obj)) return (T)obj;
            if (objectname_to_bundle.TryGetValue(name.ToLower(), out var obj2))
            {
                return LoadAsset<T>(bundles[obj2], name);
            }
            return null;
        }
    }
}
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using UnityEngine;
using Reactor.Utilities.Extensions;

namespace TownOfUs.Patches.ScreenEffects
{

    public class CameraEffect : MonoBehaviour
    {
        static CameraEffect() => ClassInjector.RegisterTypeInIl2Cpp<CameraEffect>();
        public CameraEffect(IntPtr ptr) : base(ptr) { }
        public static CameraEffect singleton { get; private set; }
        public static void Initialize()
        {
            if (singleton != null) singleton.Destroy();
            singleton = Camera.main.gameObject.AddComponent<CameraEffect>();
        }
        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (materials.Count == 0)
            {
                Graphics.Blit(source, destination);
                return;
            }
            foreach (Material material in materials) Graphics.Blit(source, destination, material);
        }

        public List<Material> materials = new List<Material>();
    }

}
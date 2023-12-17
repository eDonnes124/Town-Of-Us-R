using Reactor.Utilities;
using System.Collections;
using UnityEngine;

namespace TownOfUs.Patches.ScreenEffects
{
    public abstract class ScriptEffect
    {
        protected Camera camera = CameraEffect.singleton.gameObject.GetComponent<Camera>();
        protected ScriptEffect()
        {
            Coroutines.Start(runEffect());
        }

        public abstract IEnumerator runEffect();
    }
}
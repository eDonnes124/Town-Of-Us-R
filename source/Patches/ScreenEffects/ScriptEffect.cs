using Reactor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
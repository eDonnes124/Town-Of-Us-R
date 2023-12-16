using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TownOfUs.Patches.ScreenEffects
{
#if DEBUG
    [Obsolete("There is a built in shake Enumerator, but this is just an example")]
    public class CameraShake : ScriptEffect
    {
        private float cameraShakeDuration = 0.5f;
        private float cameraShakeDecreaseFactor = 1f;
        private float cameraShakeAmount = 1f;

        public CameraShake(float _cameraShakeDuration = 0.5f, float _cameraShakeDecreaseFactor = 1f, float _cameraShakeAmount = 1f)
        {
            cameraShakeDuration = _cameraShakeDuration;
            cameraShakeDecreaseFactor = _cameraShakeDecreaseFactor;
            _cameraShakeAmount = cameraShakeAmount;
        }
        public override IEnumerator runEffect()
        {
            var originalPos = camera.transform.localPosition;
            var duration = cameraShakeDuration;
            while (duration > 0)
            {
                camera.transform.localPosition = originalPos + Random.insideUnitSphere * cameraShakeAmount;
                duration -= Time.deltaTime * cameraShakeDecreaseFactor;
                yield return null;
            }
            camera.transform.localPosition = originalPos;
        }
    }
#endif
}
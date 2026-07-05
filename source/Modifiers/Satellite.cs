using System;
using System.Collections.Generic;
using System.Collections;
using Object = UnityEngine.Object;
using UnityEngine;
using Reactor.Utilities;

namespace TownOfUs.Modifiers
{
    public class Satellite : Modifier
    {
        public KillButton DetectButton;

        public bool DetectUsed;
        public DateTime StartingCooldown { get; set; }
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();

        public Satellite(PlayerControl player) : base(player)
        {
            Name = "Satellite";
            TaskText = () => "Detect dead bodies";
            Color = Patches.Colors.Satellite;
            StartingCooldown = DateTime.UtcNow;
            ModifierType = ModifierEnum.Satellite;
        }

        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public IEnumerator Detect()
        {
            var bodies = Object.FindObjectsOfType<DeadBody>();
            if (bodies.Count == 0) yield break;
            Coroutines.Start(Utils.FlashCoroutine(Color));
            foreach (var body in bodies)
            {
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = TownOfUs.Arrow;
                arrow.image = renderer;
                arrow.target = body.transform.localPosition;
                gameObj.layer = 5;
                BodyArrows.Add(body.ParentId, arrow);
            }

            yield return (object)new WaitForSeconds(CustomGameOptions.DetectDuration);

            BodyArrows.Values.DestroyAll();
            BodyArrows.Clear();
        }
    }
}
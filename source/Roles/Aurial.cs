using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Aurial : Role
    {
        public Dictionary<(Vector3, int), ArrowBehaviour> SenseArrows = new Dictionary<(Vector3, int), ArrowBehaviour>();
        public static Sprite Sprite => TownOfUs.Arrow;
        public Aurial(PlayerControl player) : base(player)
        {
            Name = "Aurial";
            ImpostorText = () => "Sense Disturbances In Your Aura";
            TaskText = () => "Sense any player ability used inside your aura";
            Color = Patches.Colors.Aurial;
            RoleType = RoleEnum.Aurial;
            AddToRoleHistory(RoleType);
        }

        public IEnumerator Sense(PlayerControl player)
        {
            if (!CheckRange(player, CustomGameOptions.AuraOuterRadius)) yield break;
            var position = player.GetTruePosition();
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();
            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
            var renderer = gameObj.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite;
            int colourID = player.GetDefaultOutfit().ColorId;
            if (CheckRange(player, CustomGameOptions.AuraInnerRadius) && !CamouflageUnCamouflage.IsCamoed)
            {
                if (RainbowUtils.IsRainbow(colourID))
                {
                    renderer.color = RainbowUtils.Rainbow;
                }
                else
                {
                    renderer.color = Palette.PlayerColors[colourID];
                }
            }
            arrow.image = renderer;
            gameObj.layer = 5;
            arrow.target = player.transform.position;

            try { DestroyArrow(position, colourID); }
            catch { }

            SenseArrows.Add((position, colourID), arrow);

            yield return (object)new WaitForSeconds(CustomGameOptions.SenseDuration);

            try { DestroyArrow(position, colourID); }
            catch { }
        }

        public bool CheckRange(PlayerControl player, float radius)
        {
            float lightRadius = radius * ShipStatus.Instance.MaxLightRadius;
            Vector2 vector2 = new Vector2(player.GetTruePosition().x - Player.GetTruePosition().x, player.GetTruePosition().y - Player.GetTruePosition().y);
            float magnitude = vector2.magnitude;
            if (magnitude <= lightRadius) return true;
            else return false;
        }

        public void DestroyArrow(Vector3 targetArea, int colourID)
        {
            var arrow = SenseArrows.FirstOrDefault(x => x.Key == (targetArea, colourID));
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            SenseArrows.Remove(arrow.Key);
        }
    }
}

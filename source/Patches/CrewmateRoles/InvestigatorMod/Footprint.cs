using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Extensions;
using TownOfUs.Patches;
using TownOfUs.ImpostorRoles.CamouflagerMod;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    public class Footprint
    {
        public readonly PlayerControl Player;
        private GameObject _gameObject;
        private SpriteRenderer _spriteRenderer;
        private readonly float _time;
        private readonly Vector2 _velocity;

        public Color Color;
        public Vector3 Position;
        public Investigator Role;
        public bool IsRainbow = false;

        public Footprint(PlayerControl player, Investigator role)
        {
            Role = role;
            Position = player.transform.position;
            _velocity = player.gameObject.GetComponent<Rigidbody2D>().velocity;

            Player = player;
            _time = (int) Time.time;
            Color = Palette.PlayerColors[player.GetDefaultOutfit().ColorId];
            if (RainbowUtils.IsRainbow(player.GetDefaultOutfit().ColorId)) IsRainbow = true;
            if (Grey || (player.Is(RoleEnum.Venerer) && Roles.Role.GetRole<Venerer>(player).IsCamouflaged))
            {
                Color = new Color(0.2f, 0.2f, 0.2f, 1f);
                IsRainbow = false;
            }
            if (player.Is(RoleEnum.Morphling))
            {
                var morphling = Roles.Role.GetRole<Morphling>(player);
                if (morphling.Morphed)
                {
                    Color = Palette.PlayerColors[morphling.MorphedPlayer.GetDefaultOutfit().ColorId];
                    if (RainbowUtils.IsRainbow(morphling.MorphedPlayer.GetDefaultOutfit().ColorId)) IsRainbow = true;
                    else IsRainbow = false;
                }
            }
            if (player.Is(RoleEnum.Glitch))
            {
                var glitch = Roles.Role.GetRole<Glitch>(player);
                if (glitch.IsUsingMimic)
                {
                    Color = Palette.PlayerColors[glitch.MimicTarget.GetDefaultOutfit().ColorId];
                    if (RainbowUtils.IsRainbow(glitch.MimicTarget.GetDefaultOutfit().ColorId)) IsRainbow = true;
                    else IsRainbow = false;
                }
            }

            Start();
            role.AllPrints.Add(this);
        }

        public static float Duration => CustomGameOptions.FootprintDuration;

        public static bool Grey =>
            CustomGameOptions.AnonymousFootPrint || CamouflageUnCamouflage.IsCamoed || CamouflagerUnCamouflage.CamouflagerIsCamoed;

        public static void DestroyAll(Investigator role)
        {
            while (role.AllPrints.Count != 0) role.AllPrints[0].Destroy();
        }


        private void Start()
        {
            _gameObject = new GameObject("Footprint");
            _gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            _gameObject.transform.position = Position;
            _gameObject.transform.Rotate(Vector3.forward * Vector2.SignedAngle(Vector2.up, _velocity));
            _gameObject.transform.SetParent(Player.transform.parent);

            _spriteRenderer = _gameObject.AddComponent<SpriteRenderer>();
            _spriteRenderer.sprite = TownOfUs.Footprint;
            _spriteRenderer.color = Color;
            _gameObject.transform.localScale *= new Vector2(1.2f, 1f) * (CustomGameOptions.FootprintSize / 10);

            _gameObject.SetActive(true);
        }

        private void Destroy()
        {
            Object.Destroy(_gameObject);
            Role.AllPrints.Remove(this);
        }

        public bool Update()
        {
            var currentTime = Time.time;
            var alpha = Mathf.Max(1f - (currentTime - _time) / Duration, 0f);

            if (alpha < 0 || alpha > 1)
                alpha = 0;

            if (IsRainbow) Color = RainbowUtils.Rainbow;

            Color = new Color(Color.r, Color.g, Color.b, alpha);
            _spriteRenderer.color = Color;

            if (_time + (int) Duration < currentTime)
            {
                Destroy();
                return true;
            }

            return false;
        }
    }
}
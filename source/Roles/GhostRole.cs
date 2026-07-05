using UnityEngine;
using TownOfUs.Extensions;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class GhostRole : Role
    {
        public static readonly Dictionary<byte, GhostRole> GhostRoleDictionary = new Dictionary<byte, GhostRole>();
        public bool Caught;
        public bool CompletedTasks;
        public bool Faded;

        public GhostRole(PlayerControl player) : base(player)
        {
            GhostRoleDictionary.Add(player.PlayerId, this);
        }

        public void Fade()
        {
            Faded = true;
            Player.Visible = true;
            var color = new Color(1f, 1f, 1f, 0f);

            var maxDistance = ShipStatus.Instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;

            if (PlayerControl.LocalPlayer == null)
                return;

            var distance = (PlayerControl.LocalPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + velocity / Player.MyPhysics.GhostSpeed * 0.13f;
            color.a = Mathf.Lerp(color.a, 0, distPercent);

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            {
                Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, new NetworkedPlayerInfo.PlayerOutfit()
                {
                    ColorId = Player.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " ",
                    PetId = ""
                });
            }
            Player.myRend().color = color;
            Player.nameText().color = Color.clear;
            Player.cosmetics.colorBlindText.color = Color.clear;
            Player.cosmetics.SetBodyCosmeticsVisible(false);
        }

        public static GhostRole GetGhostRole(PlayerControl player)
        {
            if (player == null) return null;
            if (GhostRoleDictionary.TryGetValue(player.PlayerId, out var role))
                return role;

            return null;
        }
    }
}
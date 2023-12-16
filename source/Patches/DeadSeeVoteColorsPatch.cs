using HarmonyLib;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    public static class DeadSeeVoteColorsPatch
    {
        public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] GameData.PlayerInfo voterPlayer,
            [HarmonyArgument(1)] int index, [HarmonyArgument(2)] Transform parent)
        {
            SpriteRenderer spriteRenderer = Object.Instantiate<SpriteRenderer>(__instance.PlayerVotePrefab);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor))
            {
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
            }
            else if (GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes && (!CustomGameOptions.DeadSeeRoles || !PlayerControl.LocalPlayer.Data.IsDead))
            {
                //PlayerControl.SetPlayerMaterialColors(Palette.DisabledGrey, spriteRenderer);
                PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
            }
            else
            {
                //PlayerControl.SetPlayerMaterialColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
            }
            spriteRenderer.transform.SetParent(parent);
            spriteRenderer.transform.localScale = Vector3.zero;
            var component = parent.GetComponent<PlayerVoteArea>();
            if (component != null)
            {
                spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);
            }

            var Base = __instance as MonoBehaviour;
            Base.StartCoroutine(Effects.Bloop((float)index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
            parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
            return false;
        }
    }
}
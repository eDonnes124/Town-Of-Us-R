using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch]

    public class ShowNewDead
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CoIntro))]
        [HarmonyPostfix]

        public static void Postfix(MeetingHud __instance, ref GameData.PlayerInfo reportedBody, ref Il2CppReferenceArray<GameData.PlayerInfo> deadBodies)
        {
            if (!CustomGameOptions.IndicateRecentDead) return;

            foreach (var player in __instance.playerStates)
            {
                if (deadBodies.Any(x => x.PlayerId == player.TargetPlayerId))
                {
                    player.Megaphone.gameObject.SetActive(true);
                    player.Megaphone.enabled = true;
                    player.Megaphone.transform.localEulerAngles = Vector3.zero;
		            player.Megaphone.transform.localScale = Vector3.one;
                    
                    if (player.TargetPlayerId != reportedBody.PlayerId)
                    {
                        player.Megaphone.sprite = GameManager.Instance.DeadBodyPrefab.bodyRenderers[0].sprite;
                        player.Megaphone.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        player.Megaphone.transform.localPosition -= new Vector3(0.2f, 0, 0);
                    }
                }
            }
        }
    }
}
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.CrewmateRoles.MedicMod;
using UnityEngine;

namespace TownOfUs.Patches.CrewmateRoles.SpyMod
{
    [HarmonyPatch]
    public static class Vitals
    {
        private static List<TMPro.TextMeshPro> SpyText = new List<TMPro.TextMeshPro>();

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
        class VitalsMinigameStartPatch
        {
            static void Postfix(VitalsMinigame __instance)
            {
                if (CustomGameOptions.GameMode == GameMode.Cultist) return;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Spy))
                {
                    SpyText = new List<TMPro.TextMeshPro>();
                    foreach (VitalsPanel panel in __instance.vitals)
                    {
                        TMPro.TextMeshPro text = UnityEngine.Object.Instantiate(__instance.SabText, panel.transform);
                        SpyText.Add(text);
                        UnityEngine.Object.DestroyImmediate(text.GetComponent<AlphaBlink>());
                        text.gameObject.SetActive(false);
                        text.transform.localScale = Vector3.one * 0.75f;
                        text.transform.localPosition = new Vector3(-0.75f, -0.23f, 0f);

                    }
                }
            }
        }

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
        class VitalsMinigameUpdatePatch
        {
            static void Postfix(VitalsMinigame __instance)
            {
                if (CustomGameOptions.GameMode == GameMode.Cultist) return;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Spy))
                {
                    for (int R = 0; R < __instance.vitals.Length; R++)
                    {
                        VitalsPanel vitalsPanel = __instance.vitals[R];
                        GameData.PlayerInfo player = vitalsPanel.PlayerInfo;

                        if (vitalsPanel.IsDead)
                        {
                            DeadPlayer deadPlayer = Murder.KilledPlayers?.Where(x => x.PlayerId == player?.PlayerId)?.FirstOrDefault();
                            if (deadPlayer != null && R < SpyText.Count && SpyText[R] != null)
                            {
                                float Deathtime = ((float)(DateTime.UtcNow - deadPlayer.KillTime).TotalMilliseconds);
                                SpyText[R].gameObject.SetActive(true);
                                SpyText[R].text = Math.Round(Deathtime / 1000) + "s";
                            }
                        }
                    }
                }
                else
                {
                    foreach (TMPro.TextMeshPro text in SpyText)
                        if (text != null && text.gameObject != null)
                            text.gameObject.SetActive(false);
                }
            }
        }
    }
}

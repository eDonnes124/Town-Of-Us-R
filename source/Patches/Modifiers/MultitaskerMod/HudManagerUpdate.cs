using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Modifiers.MultitaskerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class Multitasker
    {
        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Multitasker)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.Disconnected) return;
            if (!Minigame.Instance) return;
            if (IsExemptTask()) return;
            var Base = Minigame.Instance as MonoBehaviour;
            SpriteRenderer[] rends = Base.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < rends.Length; i++)
            {
                var oldColor1 = rends[i].color[0];
                var oldColor2 = rends[i].color[1];
                var oldColor3 = rends[i].color[2];
                rends[i].color = new Color(oldColor1, oldColor2, oldColor3, 0.5f);
            }
        }

        public static bool IsExemptTask()
        {
            return Minigame.Instance.TryCast<VitalsMinigame>() ||
                   Minigame.Instance.TryCast<CollectShellsMinigame>() ||
                   Minigame.Instance.TryCast<MushroomDoorSabotageMinigame>();
        }
    }
}
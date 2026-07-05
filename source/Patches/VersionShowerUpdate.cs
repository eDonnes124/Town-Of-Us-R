using HarmonyLib;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShowerUpdate
    {
        public static void Postfix(VersionShower __instance)
        {
            var text = __instance.text;
            text.text += " - <color=#00FF00FF>Town Of Us: Reactivated v" + TownOfUs.VersionString + "</color>" + TownOfUs.VersionTag;
            text.transform.localPosition += new Vector3(-0.8f, -0.16f, 0f);

            if (GameObject.Find("RightPanel"))
            {
                text.transform.SetParent(GameObject.Find("RightPanel").transform);

                var aspect = text.gameObject.AddComponent<AspectPosition>();
                aspect.Alignment = AspectPosition.EdgeAlignments.Center;
                aspect.DistanceFromEdge = new Vector3(-0.2f, -0.25f, 8f);

                aspect.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) =>
                {
                    aspect.AdjustPosition();
                })));

                return;
            }
        }
    }
}

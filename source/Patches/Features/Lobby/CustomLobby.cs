/// Code : https://github.com/Loonie-Toons/TownOfHost-ReEdited/blob/48587d86c58c874e539d35bf4f7fd4e2bb2d6949/Patches/LobbyPatch.cs

using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches.Features;

[HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.FixedUpdate))]
public class LobbyFixedUpdatePatch
{
    private static GameObject Paint;
    public static void Postfix()
    {
        if (Paint == null)
        {
            var LeftBox = GameObject.Find("Leftbox");
            if (LeftBox != null)
            {
                Paint = Object.Instantiate(LeftBox, LeftBox.transform.parent.transform);
                Paint.name = "Lobby Paint";
                Paint.transform.localPosition = new Vector3(0.042f, -2.59f, -10.5f);
                SpriteRenderer renderer = Paint.GetComponent<SpriteRenderer>();
                renderer.sprite = Utils.LoadSprite("TownOfUs.Resources.LobbyPaint.png", 290f);
            }
        }
    }
}
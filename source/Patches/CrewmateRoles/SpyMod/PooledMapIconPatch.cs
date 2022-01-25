using HarmonyLib;
using UnityEngine;
using TMPro;

namespace TownOfUs.CrewmateRoles.SpyMod {
	[HarmonyPatch(typeof(PooledMapIcon), nameof(PooledMapIcon.Reset))]
	public static class PooledMapIconPatch {
		public static void Postfix(PooledMapIcon __instance) {
			var sprite = __instance.GetComponent<SpriteRenderer>();
			if (sprite != null) {
				PlayerControl.SetPlayerMaterialColors(new Color(0.8793f, 1, 0, 1), sprite); // Reset Color to default. Fixes Shifted Spy issue
			}
			var text = __instance.GetComponentInChildren<TextMeshPro>(true);
			if (text == null) {
				text = new GameObject("Text").AddComponent<TextMeshPro>();
				text.transform.SetParent(__instance.transform, false);
				text.fontSize = 1.5f;
				text.fontSizeMin = 1;
				text.fontSizeMax = 1.5f;
				text.enableAutoSizing = true;
				text.fontStyle = FontStyles.Bold;
				text.alignment = TextAlignmentOptions.Center;
				text.horizontalAlignment = HorizontalAlignmentOptions.Center;
				text.gameObject.layer = 5;
				text.fontMaterial.EnableKeyword("OUTLINE_ON");
				text.fontMaterial.SetFloat("_OutlineWidth", 0.1745f);
				text.fontMaterial.SetFloat("_FaceDilate", 0.151f);
			}
			text.transform.localPosition = new Vector3(0, 0, -20);
			text.text = "";
			text.gameObject.SetActive(false);
			var sprite = __instance.GetComponent<SpriteRenderer>();
			if (sprite != null) {
				PlayerControl.SetPlayerMaterialColors(new Color(0.8793f, 1, 0, 1), sprite); // Reset Color to default. Fixes Shifted Spy issue
			}
		}
	}
}

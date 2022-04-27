using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Patches.CrewmateRoles.SpyMod {
	[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
	public class AddNumbers {

		public static void GenNumber(Spy role, PlayerVoteArea voteArea) {
			var targetId = voteArea.TargetPlayerId;

			var nameText = Object.Instantiate(voteArea.NameText, voteArea.transform);
			nameText.transform.localPosition = new Vector3(-1.211f, -0.18f, -0.1f);
			nameText.text = GameData.Instance.GetPlayerById(targetId).DefaultOutfit.ColorId.ToString();
			role.PlayerNumbers[targetId] = nameText;
		}

		public static void Postfix(MeetingHud __instance) {
			foreach (var role in Role.GetRoles(RoleEnum.Spy)) {
				var spy = (Spy)role;
				spy.PlayerNumbers.Clear();
			}

			if (PlayerControl.LocalPlayer.Data.IsDead) return;
			if (!PlayerControl.LocalPlayer.Is(RoleEnum.Spy)) return;

			var spyRole = Role.GetRole<Spy>(PlayerControl.LocalPlayer);

			foreach (var voteArea in __instance.playerStates) {
				GenNumber(spyRole, voteArea);
			}
		}
	}
}
using HarmonyLib;

namespace TownOfUs.Patches;

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
	public static void Postfix(PingTracker __instance)
	{
		var position = __instance.GetComponent<AspectPosition>();
		position.Alignment = AspectPosition.EdgeAlignments.Top;
		position.DistanceFromEdge = new(0f, 0.1f, 0);
		position.AdjustPosition();

		__instance.text.text =
			"<size=120%><color=#00FF00FF>TownOfUs v" + TownOfUs.VersionString + "</color>" + TownOfUs.VersionTag + "\n</size>" +
			$"Ping: {AmongUsClient.Instance?.Ping}ms\n" +
			(!MeetingHud.Instance
				? "Modded By: <color=#00FF00FF>Donners</color> & <color=#FF8000>MyDragonBreath</color>\n" : "") +
			(AmongUsClient.Instance?.GameState != InnerNet.InnerNetClient.GameStates.Started
				? "Formerly: <color=#00FF00FF>Slushiegoose</color> & <color=#8600FF>Polus.gg</color></color>\n" +
				"</size>" : "");
	}
}

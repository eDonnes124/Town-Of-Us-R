using HarmonyLib;
using Object = UnityEngine.Object;
using Reactor.Utilities.Extensions;
using UnityEngine;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.Extensions;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHud_Start
    {
        public static void Postfix(MeetingHud __instance)
        {
            Utils.ShowDeadBodies = PlayerControl.LocalPlayer.Data.IsDead;

            foreach (var player in PlayerControl.AllPlayerControls)
                player.MyPhysics.ResetAnimState();

            HudUpdate.Zooming = false;
            Camera.main.orthographicSize = 3f;

            foreach (var cam in Camera.allCameras)
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = 3f;

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);

            // Added: Prosecutor vote color logic
            foreach (var playerVote in __instance.playerStates)
            {
                var player = GameData.Instance.GetPlayerById(playerVote.TargetPlayerId).Object;
                if (player == null) continue; // Critical null check

                if (player.Is(RoleEnum.Prosecutor))
                {
                    var prosecutor = Role.GetRole<ProsecutorRole>(player);
                    if (prosecutor?.ProsecutedPlayer?.Is(RoleEnum.Jester) == true)
                        playerVote.NameText.color = Palette.Purple;
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public class MeetingHud_Close
    {
        public static void Postfix(MeetingHud __instance)
        {
            Utils.Rpc(CustomRPC.RemoveAllBodies);
            var buggedBodies = Object.FindObjectsOfType<DeadBody>();
            foreach (var body in buggedBodies)
            {
                body.gameObject.Destroy();
            }
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    public class ExileAnimStart
    {
        public static void Postfix(ExileController __instance, [HarmonyArgument(0)] NetworkedPlayerInfo exiled, [HarmonyArgument(1)] bool tie)
        {
            Utils.ShowDeadBodies = PlayerControl.LocalPlayer.Data.IsDead || exiled?.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        }
    }
}
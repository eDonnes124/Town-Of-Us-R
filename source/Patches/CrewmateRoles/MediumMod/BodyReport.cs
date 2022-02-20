using System;
using System.Linq;
using HarmonyLib;
using TownOfUs.CrewmateRoles.MedicMod;

namespace TownOfUs.CrewmateRoles.MediumMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHud_Start
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medium) || PlayerControl.LocalPlayer.Data.IsDead) {
                return;
            }
            var matches = Murder.KilledPlayers.ToArray();

            foreach (var match in matches) {
                if (match != null) {
                    var br = new BodyReport
                    {
                        Killer = Utils.PlayerById(match.KillerId),
                        Reporter = PlayerControl.LocalPlayer,
                        Body = Utils.PlayerById(match.PlayerId),
                        KillAge = (float) (DateTime.UtcNow - match.KillTime).TotalMilliseconds
                    };
                    var reportMsg = BodyReport.ParseBodyReport(br);
                    if (!string.IsNullOrWhiteSpace(reportMsg)) {
                        if (DestroyableSingleton<HudManager>.Instance) {
                            // Send the message through chat only visible to the medium
                            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reportMsg);
                        }
                    }
                }
            }
        }
    }
}
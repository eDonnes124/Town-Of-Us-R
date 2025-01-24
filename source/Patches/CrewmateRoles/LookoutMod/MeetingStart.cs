using HarmonyLib;
using System;
using System.Linq;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.LookoutMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Lookout)) return;
            var loRole = Role.GetRole<Lookout>(PlayerControl.LocalPlayer);
            foreach (var (key, value) in loRole.Watching)
            {
                var name = Utils.PlayerById(key).Data.PlayerName;
                if (value.Count == 0)
                {
                    if (DestroyableSingleton<HudManager>.Instance)
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"No players interacted with {name}");
                }
                else
                {
                    string message = $"Roles seen interacting with {name}:\n";
                    foreach (RoleEnum role in value.OrderBy(x => Guid.NewGuid()))
                    {
                        message += $" {role},";
                    }
                    message = message.Remove(message.Length - 1, 1);
                    if (DestroyableSingleton<HudManager>.Instance)
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                }
            }
        }
    }
}

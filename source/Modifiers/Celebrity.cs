using Hazel.Dtls;
using System;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.Modifiers
{
    public class Celebrity : Modifier
    {
        public bool JustDied = false;
        public bool MessageShown = false;
        public DateTime DeathTime { get; set; }
        public string Message = string.Empty;
        public string Room = string.Empty;
        public Celebrity(PlayerControl player) : base(player)
        {
            Name = "Celebrity";
            TaskText = () => "Announce how you died on your passing";
            Color = Patches.Colors.Celebrity;
            ModifierType = ModifierEnum.Celebrity;
        }

        public void GenMessage(PlayerControl killer)
        {
            JustDied = true;
            DeathTime = DateTime.UtcNow;
            Room = Room == string.Empty ? "Unknown" : Room;
            if (Player == killer) Message = $"The Celebrity, {Player.GetDefaultOutfit().PlayerName}, was killed! Location: {Room}, Death: By Suicide, Time: ";
            else Message = $"The Celebrity, {Player.GetDefaultOutfit().PlayerName}, was killed! Location: {Room}, Death: By {Role.GetRole(killer).RoleType.ToString()}, Time: ";
            if (MeetingHud.Instance) PrintMessage();
        }

        public void PrintMessage()
        {
            var milliSeconds = (float) (DateTime.UtcNow - DeathTime).TotalMilliseconds;
            Message += $"{Math.Round(milliSeconds / 1000)} seconds ago.";
            MessageShown = true;
            Utils.Rpc(CustomRPC.CelebDied, Message);
        }
    }
}
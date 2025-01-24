using TMPro;
using System;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Patches;

namespace TownOfUs.Roles
{
    public class Scavenger : Role
    {
        public Scavenger(PlayerControl player) : base(player)
        {
            Name = "Scavenger";
            ImpostorText = () => "Hunt Down Prey";
            TaskText = () => 
                Target == null ? "Kill your given targets for a reduced kill cooldown" : "Hunt Down " + Target.GetDefaultOutfit().PlayerName;
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Scavenger;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            ScavengeEnd = DateTime.UtcNow;
        }

        public TextMeshPro ScavengeCooldown;
        public DateTime ScavengeEnd;
        public PlayerControl Target = null;
        public bool Scavenging = false;
        public bool GameStarted = false;
        public ArrowBehaviour PreyArrow;

        public float ScavengeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = ScavengeEnd - utcNow;
            var flag = (float)timeSpan.TotalMilliseconds < 0f;
            if (flag) return 0;
            return ((float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public PlayerControl GetClosestPlayer(PlayerControl toRemove = null)
        {
            var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.Impostors) && x != toRemove && x != ShowRoundOneShield.FirstRoundShielded).ToList();
            if (Player.IsLover()) targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.Impostors) && !x.Is(ModifierEnum.Lover) && x != toRemove && x != ShowRoundOneShield.FirstRoundShielded).ToList();
            if (targets.Count == 0) targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != PlayerControl.LocalPlayer && !x.Is(ModifierEnum.Lover) && x != toRemove).ToList();
            if (targets.Count == 0) targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != toRemove).ToList();

            var num = double.MaxValue;
            var refPosition = Player.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in targets)
            {
                if (player.Data.IsDead || player.Data.Disconnected || player.PlayerId == Player.PlayerId) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                num = distBetweenPlayers;
                result = player;
            }

            return result;
        }

        public void StopScavenge()
        {
            Scavenging = false;
            Target = null;
            if (PreyArrow != null) UnityEngine.Object.Destroy(PreyArrow);
            if (PreyArrow.gameObject != null) UnityEngine.Object.Destroy(PreyArrow.gameObject);
            PreyArrow = null;
            RegenTask();
        }
    }
}
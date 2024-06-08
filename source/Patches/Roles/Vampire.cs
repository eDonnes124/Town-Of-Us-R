using System;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Vampire : Role
    {
        public Vampire(PlayerControl player) : base(player)
        {
            Name = "Vampire";
            ImpostorText = () => "Convert Crewmates And Kill The Rest";
            TaskText = () => "Bite all other players\nFake Tasks:";
            Color = Patches.Colors.Vampire;
            LastBit = DateTime.UtcNow;
            RoleType = RoleEnum.Vampire;
            Faction = Faction.NeutralKilling;
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastBit { get; set; }

        public float BiteTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBit;
            var num = CustomGameOptions.BiteCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            // Can't win if no Vampires are alive
            var vampsAlives = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleEnum.Vampire)).ToList();
            if (vampsAlives.Count == 0) return false;
            // Can't win if any other killers are still alive
            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling)) && !x.Is(RoleEnum.Vampire)) > 0) return false;
            // Can't win if living players outnumber living Vampires
            var playersAlive = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected);
            if (vampsAlives.Count < Math.Ceiling((double)(playersAlive / 2f))) return false;
            // No other killers are alive and Vampires are equal to or greater in number than the living, so they win
            VampWin();
            Utils.EndGame();
            return false;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var vampTeam = new List<PlayerControl>();
            vampTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = vampTeam;
        }
    }
}
using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Executioner : Role
    {
        public PlayerControl target;
        public bool TargetVotedOut;

        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            ImpostorText = () =>
                target == null ? "You don't have a target for some reason... weird..." : $"Vote {target.name} Out";
            TaskText = () =>
                target == null
                    ? "You don't have a target for some reason... weird..."
                    : $"Vote {target.name} out!\nFake Tasks:";
            Color = Patches.Colors.Executioner;
            RoleType = RoleEnum.Executioner;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
            Scale = 1.4f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var exeTeam = new List<PlayerControl>();
            exeTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = exeTeam;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            if (!TargetVotedOut || !target.Data.IsDead) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;
            TargetVotedOut = true;
        }
    }
}
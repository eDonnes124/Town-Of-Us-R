using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Jester : Role
    {
        public bool VotedOut;
        public bool SpawnedAs = true;

        public PlayerControl ProsecutedBy;

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            ImpostorText = () => "Get Voted Out";
            TaskText = () => SpawnedAs ? "Get voted out!\nFake Tasks:" : "Your target was killed. Now you get voted out!\nFake Tasks:";
            Color = Patches.Colors.Jester;
            RoleType = RoleEnum.Jester;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var jestTeam = new List<PlayerControl>();
            jestTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = jestTeam;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (!VotedOut || !Player.Data.IsDead && !Player.Data.Disconnected) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Jester edition");
            VotedOut = true;
        }

        public override void Haunt()
        {
            List<PlayerControl> targets = new List<PlayerControl>();

            if (ProsecutedBy != null && !ProsecutedBy.Data.IsDead)
            {
                targets.Add(ProsecutedBy);
            }
            else
            {
                var voters = MeetingHud.Instance.playerStates
                    .Where(p => p.VotedFor == Player.PlayerId)
                    .Select(p => GameData.Instance.GetPlayerById(p.TargetPlayerId).Object);
                targets.AddRange(voters);
            }

            ShowHauntMenu(targets);
        }
    }
}
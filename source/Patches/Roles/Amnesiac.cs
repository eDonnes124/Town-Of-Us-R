using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Amnesiac : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();

        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            ImpostorText = () => "Remember a role of a deceased player";
            TaskText = () => "Remember who you were.\nFake Tasks:";
            Color = Patches.Colors.Amnesiac;
            RoleType = RoleEnum.Amnesiac;
            AddToRoleHistory(RoleType);
            Faction = Faction.Neutral;
        }

        public DeadBody CurrentTarget;

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__18 __instance)
        {
            var amnesiacTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            amnesiacTeam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = amnesiacTeam;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }
    }
}
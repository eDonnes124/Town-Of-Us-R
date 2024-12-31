using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.CamouflagerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class CamouflagerUnCamouflage
    {
        public static bool CommsEnabled;
        public static bool CamouflagerEnabled;

        public static bool CamouflagerIsCamoed => CommsEnabled | CamouflagerEnabled;

        public static void Postfix(HudManager __instance)
        {
            CamouflagerEnabled = false;
            foreach (var role in Role.GetRoles(RoleEnum.Camouflager))
            {
                var camouflager = (Camouflager) role;
                if (camouflager.Camouflaged)
                {
                    CamouflagerEnabled = true;
                    camouflager.Camouflage();
                }
                else if (camouflager.Enabled)
                {
                    CamouflagerEnabled = false;
                    camouflager.UnCamouflage();
                }
            }

            if (CustomGameOptions.ColourblindComms)
            {
                if (ShipStatus.Instance != null)
                    switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
                    {
                        case 0:
                        case 2:
                        case 3:
                        case 4:
                            var comms1 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                            if (comms1.IsActive)
                            {
                                CommsEnabled = true;
                                Utils.GroupCamouflage();
                                return;
                            }

                            break;
                        case 1:
                            var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                            if (comms2.IsActive)
                            {
                                CommsEnabled = true;
                                Utils.GroupCamouflage();
                                return;
                            }

                            break;
                    }

                if (CommsEnabled)
                {
                    CommsEnabled = false;
                    Utils.UnCamouflage();
                }
            }
        }
    }
}
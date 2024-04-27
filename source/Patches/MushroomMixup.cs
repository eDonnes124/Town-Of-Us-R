using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;

namespace TownOfUs.Patches
{
    [HarmonyPatch]

    public class MushroomMixupPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AddSystemTask))]
        [HarmonyPostfix]

        public static void Postfix(ref SystemTypes system)
        {
            if (system == SystemTypes.MushroomMixupSabotage)
            {
                foreach (Glitch glitch in Role.GetRoles(RoleEnum.Glitch).Cast<Glitch>())
                {
                    glitch.LastMimic = DateTime.UtcNow;
                    glitch.IsUsingMimic = false;
                    glitch.MimicTarget = null;
                    Utils.Unmorph(glitch.Player);
                }
                ResetAbilities();
            }
        }

        [HarmonyPatch(typeof(MushroomMixupSabotageSystem), nameof(MushroomMixupSabotageSystem.GenerateRandomOutfit))]
        [HarmonyPostfix]

        public static void Postfix(MushroomMixupSabotageSystem __instance, ref MushroomMixupSabotageSystem.CondensedOutfit __result)
        {
            List<byte> list = [.. __instance.cachedOutfitsByPlayerId.keys];
            list.Remove(PlayerControl.LocalPlayer.PlayerId);
            
            while (__result.ColorPlayerId == 34)
            {
                __result.ColorPlayerId = list.Random();
            }
        }

        public static void ResetAbilities()
        {
            foreach (Morphling Morphling in Role.GetRoles(RoleEnum.Morphling).Cast<Morphling>())
            {
                Morphling.Unmorph();
            }
            foreach (Swooper Swooper in Role.GetRoles(RoleEnum.Swooper).Cast<Swooper>())
            {
                Swooper.UnSwoop();
            }
            foreach (Venerer Venerer in Role.GetRoles(RoleEnum.Venerer).Cast<Venerer>())
            {
                Venerer.StopAbility();
            }
            foreach (Chameleon Chameleon in Role.GetRoles(RoleEnum.Chameleon).Cast<Chameleon>())
            {
                Chameleon.UnSwoop();
            }
        }
    }
}
using HarmonyLib;

namespace TownOfUs.Patches;

[HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.AddVote))]
internal class VoteBanSystemPatch
{
    public static bool Prefix()
    {
        return !AmongUsClient.Instance.AmHost || !true;
    }
}

[HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.CmdAddVote))]
internal class VoteBanSystemPatchCmd
{
    public static bool Prefix()
    {
        return !AmongUsClient.Instance.AmHost || !true;
    }
}
using HarmonyLib;
using UnityEngine;
using System.Text;
using System.IO;
using TownOfUs.CustomOption;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    public static class ExitGamePatch
    {
        public static void Prefix()
        {
            var builder = new StringBuilder();

            foreach (var option in CustomOption.CustomOption.AllOptions)
            {
                if (option.Type is CustomOptionType.Button or CustomOptionType.Header or CustomOptionType.Nested)
                    continue;

                builder.AppendLine(option.Name);
                builder.AppendLine(option.Value.ToString());
            }

            var text = Path.Combine(Application.persistentDataPath, "LastUsedSettings-temp");

            try
            {
                File.WriteAllText(text, builder.ToString());
                var text2 = Path.Combine(Application.persistentDataPath, "LastUsedSettings");
                File.Delete(text2);
                File.Move(text, text2);
            } catch {}
        }
    }
}
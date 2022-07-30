using HarmonyLib;
using ProjectM;

namespace SkanksAIO;

[HarmonyPatch]
internal static class Initialization
{
    [HarmonyPatch(typeof(ProjectM.GameBootstrap), nameof(ProjectM.GameBootstrap.Start))]
    [HarmonyPostfix]
    public static void Start_Postfix(GameBootstrap __instance)
    {
        Plugin.Init();
    }
}
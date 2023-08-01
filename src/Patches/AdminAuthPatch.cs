using System;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using Bloodstone.API;
using HarmonyLib;
using ProjectM;
using Unity.Collections;

namespace SkanksAIO.Patches;


public class AdminAuthPatch
{
    [HarmonyPatch(typeof(AdminAuthSystem), nameof(AdminAuthSystem.IsAdmin))]
    public static class IsAdmin_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(ulong platformId, ref bool __result)
        {
            if (!Settings.EnableVIPFunctionality.Value) return;
            var vips = JsonConfigHelper.GetVips();
            foreach (var vip in vips)
            {
                if (vip != platformId) continue;
                Plugin.Logger?.LogDebug("Giving admin to user from vip list " + platformId);
                __result = true;
            }
        }
    }
}
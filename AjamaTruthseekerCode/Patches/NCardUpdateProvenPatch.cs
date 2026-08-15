using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Patches;

[HarmonyPatch(typeof(NCard))]
public static class NCardUpdateProvenPatch
{
    [HarmonyPatch(nameof(NCard.UpdateVisuals))]
    [HarmonyPostfix]
    public static void UpdateVisualsPostfix(NCard __instance)
    {
        Proven.UpdateProven(__instance);
    }
    
    [HarmonyPatch(nameof(NCard.Reload))]
    [HarmonyPostfix]
    public static void ReloadPostfix(NCard __instance)
    {
        Proven.UpdateProven(__instance);
    }
}
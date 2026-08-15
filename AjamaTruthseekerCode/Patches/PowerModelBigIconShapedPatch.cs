using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Patches;

[HarmonyPatch(typeof(PowerModel), nameof(PowerModel.BigIcon), MethodType.Getter)]
public static class PowerModelBigIconShapedPatch
{
    [HarmonyPostfix]
    public static void BigIconPostfix(ref Texture2D __result, PowerModel __instance)
    {
        if (__instance is ShapedPower shapedPower)
            __result = ResourceLoader.Load<Texture2D>(shapedPower.CustomBigIconPath);
    }
}
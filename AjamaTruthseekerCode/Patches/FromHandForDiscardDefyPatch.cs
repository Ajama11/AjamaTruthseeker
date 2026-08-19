using System.Reflection;
using System.Reflection.Emit;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Patches;

[HarmonyPatch]
public static class FromHandForDiscardDefyPatch
{
    private static bool _isFromDiscard;
    
    [HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromHandForDiscard))]
    [HarmonyPrefix]
    static void PrefixDiscard(out bool __state)
    {
        __state = _isFromDiscard;
        _isFromDiscard = true;
    }
    
    [HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromHandForDiscard))]
    [HarmonyPostfix]
    static void PostfixDiscard(bool __state)
    {
        _isFromDiscard = __state;
    }
    
    [HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromHand))]
    [HarmonyPrefix]
    static void PrefixFromHand(ref CardSelectorPrefs prefs)
    {
        if (!_isFromDiscard || prefs.ShouldGlowGold is null) return;
        
        var originalGlow = prefs.ShouldGlowGold;
        
        prefs.ShouldGlowGold = c =>
        {
            if (originalGlow(c)) return true;
            return c is IOnPotentiallyDefy;
        };
    }
}
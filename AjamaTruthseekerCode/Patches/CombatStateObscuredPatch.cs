using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Patches;

[HarmonyPatch(typeof(CombatState))]
public static class CombatStateObscuredPatch
{
    [HarmonyPatch(nameof(CombatState.CreateCard), typeof(CardModel), typeof(Player))]
    [HarmonyPostfix]
    public static void CreateCardPostfix(ref CardModel __result)
    {
        if (!__result.Keywords.Contains(MyEnums.Obscured)) return;
        
        Obscured.CreateDisguise(__result);
    }
    
    [HarmonyPatch(nameof(CombatState.CloneCard))]
    [HarmonyPostfix]
    public static void CloneCardPostfix(ref CardModel __result)
    {
        if (!__result.Keywords.Contains(MyEnums.Obscured)) return;
        if (Obscured.Disguise[__result] != null) return;
        
        Obscured.CreateDisguise(__result);
    }
}
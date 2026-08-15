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
    [HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromHandForDiscard), MethodType.Async)]
    [HarmonyTranspiler]
    static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo adjustPrefs = typeof(FromHandForDiscardDefyPatch).Method(nameof(AdjustPrefs));

        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchStartForward([
                new CodeMatch(OpCodes.Ldfld), // prefs
                new CodeMatch(OpCodes.Ldarg_0), // this
                new CodeMatch(OpCodes.Ldfld), // filter
                new CodeMatch(OpCodes.Ldarg_0), // this
                new CodeMatch(OpCodes.Ldfld), // source
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Stloc_3),
            ])
            .ThrowIfInvalid("FromHandForDiscardDefyPatch could not find the correct position")
            .InsertAfter([
                new CodeInstruction(OpCodes.Call, adjustPrefs)
            ]);

        return matcher.InstructionEnumeration().ToList();
    }

    private static CardSelectorPrefs AdjustPrefs(CardSelectorPrefs prefs)
    {
        CardSelectorPrefs newPrefs = prefs;

        newPrefs.ShouldGlowGold = c => prefs.ShouldGlowGold!(c) || c is IOnPotentiallyDefy;
        
        return newPrefs;
    }
}
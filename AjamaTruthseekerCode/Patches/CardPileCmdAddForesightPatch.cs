using System.Reflection;
using System.Reflection.Emit;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Patches;

[HarmonyPatch]
public static class CardPileCmdAddForesightPatch
{
    [HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Add),
        typeof(IEnumerable<CardModel>),
        typeof(CardPile),
        typeof(CardPilePosition),
        typeof(AbstractModel),
        typeof(bool),
        typeof(bool)
    )]
    [HarmonyPatch(MethodType.Async)]
    [HarmonyTranspiler]
    static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        MethodInfo isPositionRandomOutsideForesight = typeof(CardPileCmdAddForesightPatch).Method(nameof(IsPositionRandomOutsideForesight));
        MethodInfo calculateIndex = typeof(CardPileCmdAddForesightPatch).Method(nameof(CalculateIndex));

        Label originalDefaultCaseLabel = generator.DefineLabel();
        Label myDefaultCaseLabel = generator.DefineLabel();
        Label successLabel = generator.DefineLabel();

        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchStartForward([
                new CodeMatch(OpCodes.Br_S), // After switch, go to default case
                
                new CodeMatch(OpCodes.Ldc_I4_M1), // CardPilePosition Bottom
                new CodeMatch(OpCodes.Stloc_S),
                new CodeMatch(OpCodes.Br_S), // Go to success branch
                
                new CodeMatch(OpCodes.Ldc_I4_0), // CardPilePosition Top
                new CodeMatch(OpCodes.Stloc_S),
                new CodeMatch(OpCodes.Br_S), // Go to success branch
                
                new CodeMatch(OpCodes.Ldarg_0), // this; CardPilePosition Random
                new CodeMatch(OpCodes.Ldfld), // load card parameter
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Ldloc_S), // cardPile
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Ldc_I4_1),
                new CodeMatch(OpCodes.Add),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Stloc_S),
                new CodeMatch(OpCodes.Br_S), // Go to success branch
                
                new CodeMatch(OpCodes.Ldstr), // Beginning of default case
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld), // load position parameter
                new CodeMatch(OpCodes.Box),
                new CodeMatch(OpCodes.Ldnull),
                new CodeMatch(OpCodes.Newobj),
                new CodeMatch(OpCodes.Throw),
                new CodeMatch(OpCodes.Ldloc_S), // Beginning of success branch
                new CodeMatch(OpCodes.Stloc_S),
            ])
            .ThrowIfInvalid("CardPileCmdAddForesightPatch could not find the correct position");

        matcher.Advance(21); // Onto Ldstr, Beginning of original default case
        matcher.Labels.Add(originalDefaultCaseLabel);
        matcher.Advance(7); // Onto Ldloc_S, Beginning of success branch
        matcher.Labels.Add(successLabel);
        matcher.Advance(-(21 + 7)); // Back to the start

        matcher.Advance(2); // Onto Bottom's Stloc_S
        var num = matcher.Operand;
        matcher.Advance(-2); // Back to the start
        
        matcher.Advance(8); // Onto Ldfld, load card parameter
        var loadCard = matcher.Operand;
        matcher.Advance(5); // Onto Ldloc_S, load cardPile
        var loadCardPile = matcher.Operand;
        matcher.Advance(10); // Onto Ldfld, load position
        var loadPosition = matcher.Operand;
        matcher.Advance(-(8 + 5 + 10)); // Back to the start

        matcher.Advance(3); // Onto Bottom's Br_S to success
        matcher.Operand = successLabel;
        matcher.Advance(3); // Onto Top's Br_S to success
        matcher.Operand = successLabel;
        matcher.Advance(14); // Onto Random's Br_S to success
        matcher.Operand = successLabel;

        matcher.Advance() // Beginning of default case, time to insert
            .Insert([
                new CodeInstruction(OpCodes.Ldarg_0), // this
                new CodeInstruction(OpCodes.Ldfld, loadPosition), // position
                new CodeInstruction(OpCodes.Call, isPositionRandomOutsideForesight),
                new CodeInstruction(OpCodes.Brfalse_S, originalDefaultCaseLabel),
                new CodeInstruction(OpCodes.Ldarg_0), // this
                new CodeInstruction(OpCodes.Ldfld, loadCard), // card
                new CodeInstruction(OpCodes.Ldloc_S, loadCardPile), // cardPile
                new CodeInstruction(OpCodes.Call, calculateIndex),
                new CodeInstruction(OpCodes.Stloc_S, num),
                new CodeInstruction(OpCodes.Br_S, successLabel)
            ]);

        matcher.Labels.Add(myDefaultCaseLabel);

        matcher.Advance(-(3 + 3 + 14 + 1)); // Back to the start
        matcher.Operand = myDefaultCaseLabel;
        
        return matcher.InstructionEnumeration().ToList();
    }

    private static bool IsPositionRandomOutsideForesight(CardPilePosition position)
    {
        return position == MyEnums.RandomOutsideForesight;
    }

    private static int CalculateIndex(CardModel card, CardPile cardPile)
    {
        int foresightAmount = card.Owner.Creature.GetPowerAmount<ForesightPower>();
        
        return foresightAmount > cardPile.Cards.Count ?
            card.Owner.RunState.Rng.Shuffle.NextInt(cardPile.Cards.Count + 1) :
            card.Owner.RunState.Rng.Shuffle.NextInt(Math.Min(foresightAmount, cardPile.Cards.Count), cardPile.Cards.Count + 1);
    }
}
using AjamaTruthseeker.AjamaTruthseekerCode.Character;
using AjamaTruthseeker.AjamaTruthseekerCode.Nodes;
using AjamaTruthseeker.AjamaTruthseekerCode.Singletons;
using AjamaTruthseeker.AjamaTruthseekerCode.Systems;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Patches;

[HarmonyPatch]
public static class DrawPileObscuredCountPatches
{
    [HarmonyPatch(typeof(NDrawPileButton), nameof(NDrawPileButton._Ready))]
    [HarmonyPostfix]
    public static void ReadyPostfix(NDrawPileButton __instance)
    {
        var nObscuredCount = PreloadManager.Cache.GetScene("res://AjamaTruthseeker/scenes/obscured_container.tscn")
            .Instantiate<NObscuredCount>();
        
        __instance.AddChildSafely(nObscuredCount);
        nObscuredCount.GlobalPosition = __instance.GlobalPosition;
    }
    
    [HarmonyPatch(typeof(NHoverTipSet), nameof(NHoverTipSet.CreateAndShow),
        typeof(Control), 
        typeof(IHoverTip),
        typeof(HoverTipAlignment))]
    [HarmonyPrefix]
    public static void HoverTipPrefix(Control owner, ref IHoverTip hoverTip)
    {
        if (owner is not NDrawPileButton) return;
        if (hoverTip is not HoverTip realHoverTip) return;
        if (RunManager.Instance.State == null) return;

        var player = LocalContext.GetMe(RunManager.Instance.State.Players);
        var combatState = player?.PlayerCombatState;
        if (combatState == null) return;
        
        if (!combatState.AllCards.Any(c =>
                c.Keywords.Contains(MyEnums.Obscured)) &&
            player!.Character is not Truthseeker)
        {
            return;
        }

        LocString addition = new LocString("gameplay_ui", "AJAMATRUTHSEEKER-OBSCURED_COUNT");

        int allObscuredVisible = ForesightSingleton.NumberOfAllObscuredCardsInForesight[combatState];
        int allObscuredTotal = combatState.DrawPile.Cards
            .Count(c => c.Keywords.Contains(MyEnums.Obscured));

        int hiddenObscuredVisible = ForesightSingleton.NumberOfHiddenObscuredCardsInForesight[combatState];
        int hiddenObscuredTotal = combatState.DrawPile.Cards
            .Count(c => c.Keywords.Contains(MyEnums.Obscured) && !Proven.IsProven(c));
        
        addition.Add("AllObscuredVisible", allObscuredVisible);
        addition.Add("AllObscuredTotal", allObscuredTotal);
        
        addition.Add("HiddenObscuredVisible", hiddenObscuredVisible);
        addition.Add("HiddenObscuredTotal", hiddenObscuredTotal);
        
        addition.Add("DifferentVisible", allObscuredVisible != hiddenObscuredVisible);
        addition.Add("DifferentTotal", allObscuredTotal != hiddenObscuredTotal);
        
        realHoverTip.Description += addition.GetFormattedText();

        hoverTip = realHoverTip;
    }
}
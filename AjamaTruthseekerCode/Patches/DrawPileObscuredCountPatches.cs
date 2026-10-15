using AjamaTruthseeker.AjamaTruthseekerCode.Singletons;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Patches;

[HarmonyPatch]
public static class DrawPileObscuredCountPatches
{
    [HarmonyPatch(typeof(NHoverTipSet), nameof(NHoverTipSet.CreateAndShow),
        typeof(Control), 
        typeof(IHoverTip),
        typeof(HoverTipAlignment))]
    [HarmonyPrefix]
    public static void CreateAndShowPrefix(Control owner, ref IHoverTip hoverTip)
    {
        if (owner is not NDrawPileButton) return;
        if (hoverTip is not HoverTip realHoverTip) return;
        if (RunManager.Instance.State == null) return;

        var player = LocalContext.GetMe(RunManager.Instance.State.Players);
        var combatState = player?.PlayerCombatState;
        if (combatState == null) return;
        
        realHoverTip.Description += $"\n\n{ForesightSingleton.NumberOfObscuredCardsInForesight[combatState]}.";

        hoverTip = realHoverTip;
    }
}
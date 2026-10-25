using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;
using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class DrawTheory : AbstractTheoryMod
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Player);
    }

    public override bool ApplyStacked(CardModifier newApplied)
    {
        DynamicVars.Cards.BaseValue += newApplied.DynamicVars.Cards.BaseValue;
        return true;
    }

    public static DrawTheory Create(int amount = 1)
    {
        DrawTheory mod = ModelDb.CardModifier<DrawTheory>().TypedMutableClone();

        mod.DynamicVars.Cards.BaseValue = amount;

        return mod;
    }
}
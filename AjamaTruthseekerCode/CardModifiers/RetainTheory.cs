using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;
using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class RetainTheory : AbstractTheoryMod
{
    public override void OnInitialApplication()
    {
        Card.AddKeyword(CardKeyword.Retain);
    }

    public override bool ApplyStacked(CardModifier newApplied)
    {
        return true;
    }

    public static RetainTheory Create()
    {
        RetainTheory mod = ModelDb.CardModifier<RetainTheory>().TypedMutableClone();
        return mod;
    }
}
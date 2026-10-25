using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;
using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class EnergyTheory : AbstractTheoryMod
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Player);
    }

    public override bool ApplyStacked(CardModifier newApplied)
    {
        DynamicVars.Energy.BaseValue += newApplied.DynamicVars.Energy.BaseValue;
        return true;
    }

    public static EnergyTheory Create(int amount = 1)
    {
        EnergyTheory mod = ModelDb.CardModifier<EnergyTheory>().TypedMutableClone();

        mod.DynamicVars.Energy.BaseValue = amount;

        return mod;
    }
}
using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Basic;

public class FutureQuestions() : AjamaTruthseekerCard(1,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self), IOnPotentiallyDefy
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ForesightVar(1)
    ];
    
    public override List<NonDynamicEffect> MyNonDynamicEffects =>
    [
        NonDynamicEffect.Rewrite,
        NonDynamicEffect.Held
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MyActions.Rewrite(choiceContext, Owner);
    }

    public override async Task OnHeld(PlayerChoiceContext choiceContext)
    {
        await CommonActions.ApplySelf<ForesightPower>(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Foresight.UpgradeValueBy(1);
    }

    public async Task OnDefy(CardModel card, PlayerChoiceContext choiceContext)
    {
        await MyActions.Prove(this, choiceContext, Owner, 2, [PileType.Hand, PileType.Draw]);
    }
}
using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;
using static AjamaTruthseeker.AjamaTruthseekerCode.Utils.Shape;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Common.Skill;

public class TimeLoss() : AjamaTruthseekerCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self),
    IOnPotentiallyForeseen
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new CardsVar(1)
    ];

    public override List<ShapeType> MyShapeEffects =>
    [
        ShapeType.Friend
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DoShapeEffect(ShapeType.Friend, choiceContext, async () =>
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner));
    }
    
    public async Task OnForeseen(PlayerChoiceContext choiceContext)
    {
        await CommonActions.Draw(this, choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
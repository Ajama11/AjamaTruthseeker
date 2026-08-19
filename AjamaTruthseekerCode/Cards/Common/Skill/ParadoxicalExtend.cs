using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using static AjamaTruthseeker.AjamaTruthseekerCode.Utils.Shape;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Common.Skill;

public class ParadoxicalExtend() : AjamaTruthseekerCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self),
    IOnPotentiallyForeseen
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3, BlockProps.card),
        new ForesightVar(1)
    ];

    public override List<ShapeType> MyShapeEffects =>
    [
        ShapeType.Abstract
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Obscured
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DoShapeEffect(ShapeType.Abstract, choiceContext, async () => 
            await CommonActions.CardBlock(this, play));
    }
    
    public async Task OnForeseen(PlayerChoiceContext choiceContext)
    {
        await CommonActions.ApplySelf<ForesightPower>(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}
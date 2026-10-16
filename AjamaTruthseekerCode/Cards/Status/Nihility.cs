using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.Character;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Status;

[Pool(typeof(StatusTruthseekerCardPool))]
public class Nihility() : AjamaTruthseekerCard(1,
    CardType.Status, CardRarity.Status,
    TargetType.None)
{
    public override int MaxUpgradeLevel => 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new DamageVar(3, DamageProps.cardUnpowered)
    ];

    public override List<NonDynamicEffect> MyNonDynamicEffects =>
    [
        NonDynamicEffect.Drawn,
        NonDynamicEffect.Held
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Obscured
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {

    }

    public override async Task OnDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        await MyActions.Draw(choiceContext, this);
    }

    public override async Task OnHeld(PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.Damage, this, null);
    }
}
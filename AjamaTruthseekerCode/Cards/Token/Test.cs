using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;
using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Test() : AjamaTruthseekerCard(0,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1, DamageProps.card)
    ];

    public override List<NonDynamicEffect> MyNonDynamicEffects =>
    [

    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [

    ];

    public override BundledHoverTipManager MyBundles =>
    [

    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (var theory in Owner.Hand.OfType<Theory>())
        {
            int randomType = this.Rng.CombatCardGeneration.NextInt(0, 3);

            var damage = randomType switch
            {
                0 => DamageTheory.Create(single: DynamicVars.Damage.BaseValue),
                1 => DamageTheory.Create(aoe: DynamicVars.Damage.BaseValue),
                _ => DamageTheory.Create(random: DynamicVars.Damage.BaseValue)
            };

            theory.AddModifier(damage);
            
            theory.AddModifier<WeakTheory>();
        }
    }

    protected override void OnUpgrade()
    {

    }
}
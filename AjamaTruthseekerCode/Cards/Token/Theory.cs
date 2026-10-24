using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;
using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
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
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Theory() : AjamaTruthseekerCard(0,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    public IEnumerable<AbstractTheoryMod> Modifiers =>
        this.GetModifiers().OfType<AbstractTheoryMod>();
    
    public override CardType Type =>
        Modifiers.OfType<DamageTheory>().Any() ? 
            CardType.Attack : CardType.Skill;

    public override TargetType TargetType =>
        Modifiers.Any(m => m.Targeting == TargetType.AnyEnemy) ? TargetType.AnyEnemy :
        Modifiers.Any(m => m.Targeting == TargetType.AllEnemies) ? TargetType.AllEnemies :
        TargetType.Self;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    public override BundledHoverTipManager MyBundles
    {
        get
        {
            BundledHoverTipManager result = [];
            
            foreach (var modifier in Modifiers)
            {
                result.AddRange(modifier.BundlesToUse);
            }

            return result;
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
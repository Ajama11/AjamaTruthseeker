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
using MegaCrit.Sts2.Core.Models.Powers;
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

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // int randomType = this.Rng.CombatCardGeneration.NextInt(0, 3);
        // var damage = randomType switch
        // {
        //     0 => DamageTheory.Create(single: DynamicVars.Damage.BaseValue, singleTimes: 1),
        //     1 => DamageTheory.Create(aoe: DynamicVars.Damage.BaseValue),
        //     _ => DamageTheory.Create(random: DynamicVars.Damage.BaseValue, randomTimes: 2)
        // };
        //
        // await MyActions.Theorize(Owner, damage);
        MainFile.Logger.Warn($"Test is playing");
        
        MainFile.Logger.Warn($"Adding 1 damage +1 time");
        await MyActions.Theorize(Owner, DamageTheory.Create(single: DynamicVars.Damage.BaseValue, singleTimes: 1));
        MainFile.Logger.Warn($"1 damage +1 time was added");
        
        MainFile.Logger.Warn($"Adding Self Vuln");
        await MyActions.Theorize(Owner, VulnerableSelfTheory.Create());
        MainFile.Logger.Warn($"Self Vuln was added");
        
        MainFile.Logger.Warn($"Adding 1 Aoe damage");
        await MyActions.Theorize(Owner, DamageTheory.Create(aoe: DynamicVars.Damage.BaseValue));
        MainFile.Logger.Warn($"1 Aoe damage was added");
        
        MainFile.Logger.Warn($"Adding Aoe Weak");
        await MyActions.Theorize(Owner, WeakAoeTheory.Create());
        MainFile.Logger.Warn($"Aoe Weak was added");
        
        MainFile.Logger.Warn($"Adding 1 random damage +2 times");
        await MyActions.Theorize(Owner, DamageTheory.Create(random: DynamicVars.Damage.BaseValue, randomTimes: 2));
        MainFile.Logger.Warn($"1 random damage +2 times was added");
    }
}
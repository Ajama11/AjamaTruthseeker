using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips;
using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class DamageTheory : AbstractTheoryMod
{
    public DamageTheory()
    {
        Priority = -1;
    }
    
    public override bool IsAttack => true;
    public override TargetType Targeting => HasSingle ?
        TargetType.AnyEnemy : TargetType.AllEnemies;

    private const string Single = "Single";
    private const string Aoe = "Aoe";
    private const string Random = "Random";
    private const string SingleTimes = "SingleTimes";
    private const string AoeTimes = "AoeTimes";
    private const string RandomTimes = "RandomTimes";
    private const string IncreaseSingleTimes = "IncreaseSingleTimes";
    private const string IncreaseAoeTimes = "IncreaseAoeTimes";
    private const string IncreaseRandomTimes = "IncreaseRandomTimes";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(Single, 0, DamageProps.card),
        new DamageVar(Aoe, 0, DamageProps.card),
        new DamageVar(Random, 0, DamageProps.card),
        new RepeatVar(SingleTimes, 0),
        new RepeatVar(AoeTimes, 0),
        new RepeatVar(RandomTimes, 0),
        new (IncreaseSingleTimes, 0),
        new (IncreaseAoeTimes, 0),
        new (IncreaseRandomTimes, 0)
    ];
    
    private bool HasSingle => DynamicVars[Single].BaseValue > 0;
    private bool HasAoe => DynamicVars[Aoe].BaseValue > 0;
    private bool HasRandom => DynamicVars[Random].BaseValue > 0;
    private bool HasSingleTimes => DynamicVars[SingleTimes].BaseValue > 1;
    private bool HasAoeTimes => DynamicVars[AoeTimes].BaseValue > 1;
    private bool HasRandomTimes => DynamicVars[RandomTimes].BaseValue > 1;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await using var attackContext = await AttackContext.CreateAsync(CombatState!, choiceContext, cardPlay);

        decimal singleTimes = DynamicVars[SingleTimes].BaseValue;
        decimal aoeTimes = DynamicVars[AoeTimes].BaseValue;
        decimal randomTimes = DynamicVars[RandomTimes].BaseValue;
        
        if (HasSingle)
        {
            var target = cardPlay.Target ?? Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
            
            if (target != null)
            {
                for (int i = 0; i < singleTimes; i++)
                {
                    const int strongDamageThreshold = 20;
                    decimal damageAmount = DynamicVars[Single].PreviewValue;
                    
                    await CreatureCmd.TriggerAnim(Player.Creature, "Attack", Player.Character.AttackAnimDelay);

                    if (damageAmount >= strongDamageThreshold)
                    {
                        VfxCmd.PlayOnCreature(target, VfxCmd.heavyBluntPath);
                    }
                    else
                    {
                        VfxCmd.PlayOnCreatureCenter(target, VfxCmd.slashPath);
                    }
                
                    SfxCmd.PlayTmp(damageAmount >= strongDamageThreshold ? TmpSfx.heavyAttack : TmpSfx.slashAttack);
                
                    attackContext.AddHit(
                        await CreatureCmd.Damage(choiceContext, target,
                            (DamageVar) DynamicVars[Single], Card, cardPlay));
                }
            }
        }

        if (HasAoe)
        {
            var aoeVar = (DamageVar) DynamicVars[Aoe];
            
            for (int i = 0; i < aoeTimes; i++)
            {
                await CreatureCmd.TriggerAnim(Player.Creature, "Attack", Player.Character.AttackAnimDelay);
                
                VfxCmd.PlayOnSide(CombatSide.Enemy, VfxCmd.giantHorizontalSlashPath, CombatState!);
                SfxCmd.PlayTmp(TmpSfx.heavyAttack);
            
                attackContext.AddHit(
                    await CreatureCmd.Damage(choiceContext, CombatState!.HittableEnemies,
                        aoeVar.BaseValue, aoeVar.Props, Player.Creature, Card, cardPlay));
            }
        }

        if (HasRandom)
        {
            for (int i = 0; i < randomTimes; i++)
            {
                await CreatureCmd.TriggerAnim(Player.Creature, "Attack", Player.Character.AttackAnimDelay);
                
                var target = Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
                if (target == null) continue;
                
                VfxCmd.PlayOnCreatureCenter(target, VfxCmd.scratchPath);
                SfxCmd.PlayTmp(TmpSfx.slashAttack);
                
                attackContext.AddHit(
                    await CreatureCmd.Damage(choiceContext, target,
                        (DamageVar) DynamicVars[Random], Card, cardPlay));
            }
        }
    }

    public override bool ApplyStacked(CardModifier newApplied)
    {
        DynamicVars[Single].BaseValue += newApplied.DynamicVars[Single].BaseValue;
        DynamicVars[Aoe].BaseValue += newApplied.DynamicVars[Aoe].BaseValue;
        DynamicVars[Random].BaseValue += newApplied.DynamicVars[Random].BaseValue;
        
        DynamicVars[SingleTimes].BaseValue += newApplied.DynamicVars[IncreaseSingleTimes].BaseValue;
        DynamicVars[AoeTimes].BaseValue += newApplied.DynamicVars[IncreaseAoeTimes].BaseValue;
        DynamicVars[RandomTimes].BaseValue += newApplied.DynamicVars[IncreaseRandomTimes].BaseValue;

        if (DynamicVars[SingleTimes].BaseValue == 0 &&
            newApplied.DynamicVars[SingleTimes].BaseValue > 0)
        {
            DynamicVars[SingleTimes].BaseValue = newApplied.DynamicVars[SingleTimes].BaseValue;
        }
        
        if (DynamicVars[AoeTimes].BaseValue == 0 &&
            newApplied.DynamicVars[AoeTimes].BaseValue > 0)
        {
            DynamicVars[AoeTimes].BaseValue = newApplied.DynamicVars[AoeTimes].BaseValue;
        }
        
        if (DynamicVars[RandomTimes].BaseValue == 0 &&
            newApplied.DynamicVars[RandomTimes].BaseValue > 0)
        {
            DynamicVars[RandomTimes].BaseValue = newApplied.DynamicVars[RandomTimes].BaseValue;
        }
        
        return true;
    }

    public override void AddExtraArgsToLoc(LocString loc)
    {
        loc.Add("HasSingle", HasSingle);
        loc.Add("HasAoe", HasAoe);
        loc.Add("HasRandom", HasRandom);
        
        loc.Add("HasSingleTimes", HasSingleTimes);
        loc.Add("HasAoeTimes", HasAoeTimes);
        loc.Add("HasRandomTimes", HasRandomTimes);
        
        loc.Add("NewLineAfterSingle", HasSingle && (HasAoe || HasRandom));
        loc.Add("NewLineAfterAoe", HasAoe && HasRandom);
    }

    public static DamageTheory Create(
        decimal single = 0, decimal? singleTimes = null,
        decimal aoe = 0, decimal? aoeTimes = null,
        decimal random = 0, decimal? randomTimes = null)
    {
        var damage = ModelDb.CardModifier<DamageTheory>().TypedMutableClone();

        damage.DynamicVars[Single].BaseValue = single;
        damage.DynamicVars[Aoe].BaseValue = aoe;
        damage.DynamicVars[Random].BaseValue = random;

        if (singleTimes == null && single > 0)
            damage.DynamicVars[SingleTimes].BaseValue = 1;
        
        if (aoeTimes == null && aoe > 0)
            damage.DynamicVars[AoeTimes].BaseValue = 1;
        
        if (randomTimes == null && random > 0)
            damage.DynamicVars[RandomTimes].BaseValue = 1;

        if (singleTimes != null)
        {
            damage.DynamicVars[SingleTimes].BaseValue = singleTimes.Value;
            damage.DynamicVars[IncreaseSingleTimes].BaseValue = singleTimes.Value;
        }
        
        if (aoeTimes != null)
        {
            damage.DynamicVars[AoeTimes].BaseValue = aoeTimes.Value;
            damage.DynamicVars[IncreaseAoeTimes].BaseValue = aoeTimes.Value;
        }
        
        if (randomTimes != null)
        {
            damage.DynamicVars[RandomTimes].BaseValue = randomTimes.Value;
            damage.DynamicVars[IncreaseRandomTimes].BaseValue = randomTimes.Value;
        }

        return damage;
    }
}
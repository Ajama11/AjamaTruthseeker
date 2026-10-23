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

    public const string Single = "Single";
    public const string Aoe = "Aoe";
    public const string Random = "Random";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(Single, 0, DamageProps.card),
        new DamageVar(Aoe, 0, DamageProps.card),
        new DamageVar(Random, 0, DamageProps.card)
    ];
    
    bool HasSingle => DynamicVars[Single].BaseValue > 0;
    bool HasAoe => DynamicVars[Aoe].BaseValue > 0;
    bool HasRandom => DynamicVars[Random].BaseValue > 0;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await using var attackContext = await AttackContext.CreateAsync(CombatState!, choiceContext, cardPlay);

        if (HasSingle)
        {
            var target = cardPlay.Target ?? Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
            
            if (target != null)
            {
                decimal damageAmount = DynamicVars[Single].PreviewValue;

                if (damageAmount >= 20)
                {
                    VfxCmd.PlayOnCreature(target, VfxCmd.heavyBluntPath);
                }
                else
                {
                    VfxCmd.PlayOnCreatureCenter(target, VfxCmd.slashPath);
                }
                
                SfxCmd.PlayTmp(damageAmount >= 20 ? TmpSfx.heavyAttack : TmpSfx.slashAttack);
                
                attackContext.AddHit(
                    await CreatureCmd.Damage(choiceContext, target,
                        (DamageVar) DynamicVars[Single], Card, cardPlay));
            }
        }

        if (HasAoe)
        {
            var aoeVar = (DamageVar) DynamicVars[Aoe];
            
            VfxCmd.PlayOnSide(CombatSide.Enemy, VfxCmd.giantHorizontalSlashPath, CombatState!);
            SfxCmd.PlayTmp(TmpSfx.heavyAttack);
            
            attackContext.AddHit(
                await CreatureCmd.Damage(choiceContext, CombatState!.HittableEnemies,
                    aoeVar.BaseValue, aoeVar.Props, Player.Creature, Card, cardPlay));
        }

        if (HasRandom)
        {
            var target = Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
            
            if (target != null)
            {
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
        
        return true;
    }

    public override void AddExtraArgsToLoc(LocString loc)
    {
        loc.Add("HasSingle", HasSingle);
        loc.Add("HasAoe", HasAoe);
        loc.Add("HasRandom", HasRandom);
        
        loc.Add("NewLineAfterSingle", HasSingle && (HasAoe || HasRandom));
        loc.Add("NewLineAfterAoe", HasAoe && HasRandom);
    }

    public static DamageTheory Create(decimal single = 0, decimal aoe = 0, decimal random = 0)
    {
        var damage = (DamageTheory) ModelDb.CardModifier<DamageTheory>().MutableClone();

        damage.DynamicVars[Single].BaseValue += single;
        damage.DynamicVars[Aoe].BaseValue += aoe;
        damage.DynamicVars[Random].BaseValue += random;

        return damage;
    }
}
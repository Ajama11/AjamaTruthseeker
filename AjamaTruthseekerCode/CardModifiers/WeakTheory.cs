using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Cards.Status;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class WeakTheory : AbstractTheoryMod
{
    public override TargetType Targeting => TargetType.AnyEnemy;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MyPowerVar<WeakPower>(1)
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target ?? Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
        if (target == null) return;

        await PowerCmd.Apply<WeakPower>(choiceContext, target, DynamicVars.Weak.BaseValue, Player.Creature, Card);
    }
    
    public override bool ApplyStacked(CardModifier newApplied)
    {
        DynamicVars.Weak.BaseValue += newApplied.DynamicVars.Weak.BaseValue;
        return true;
    }
}
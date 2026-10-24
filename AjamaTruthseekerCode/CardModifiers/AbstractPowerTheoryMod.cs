using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Cards.Status;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public abstract class AbstractPowerTheoryMod<T> : AbstractTheoryMod where T : PowerModel
{
    public override string MainLocKeyReplacement => "AJAMATRUTHSEEKER-POWER_THEORY";
    public override TargetType Targeting { get; set; } = TargetType.Self;
    public virtual PowerType PowerType { get; set; } = PowerType.Buff;

    public T PowerModel => ModelDb.Power<T>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MyPowerVar<T>(1),
        new DisplayVar<AbstractPowerTheoryMod<T>>("PowerName", mod =>
            mod.PowerModel.Title.GetFormattedText())
    ];

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<Creature> targets = [];

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (Targeting)
        {
            case TargetType.None:
            case TargetType.Self:
                targets.Add(Player.Creature);
                break;
            case TargetType.AnyEnemy:
                var singleTarget = cardPlay.Target ?? Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
                if (singleTarget != null) targets.Add(singleTarget);
                break;
            case TargetType.AllEnemies:
                targets.AddRange(CombatState!.HittableEnemies);
                break;
            case TargetType.RandomEnemy:
                var randomTarget = Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
                if (randomTarget != null) targets.Add(randomTarget);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await PowerCmd.Apply<T>(choiceContext, targets, Amount, Player.Creature, Card);
    }
    
    public override bool ApplyStacked(CardModifier newApplied)
    {
        DynamicVars.Power<T>().BaseValue += newApplied.DynamicVars.Power<T>().BaseValue;
        Amount += newApplied.Amount;
        return true;
    }

    protected static AbstractPowerTheoryMod<T> CreateInternal(
        AbstractPowerTheoryMod<T> powerTheory,
        TargetType targeting = TargetType.Self, int amount = 1, PowerType powerType = PowerType.Buff)
    {
        powerTheory.Targeting = targeting;
        powerTheory.DynamicVars.Power<T>().BaseValue = amount;
        powerTheory.Amount = amount;
        powerTheory.PowerType = powerType;

        return powerTheory;
    }

    public override void AddExtraArgsToLoc(LocString loc)
    {
        loc.Add("IsDebuff", PowerType == PowerType.Debuff);
        loc.Add("Aoe", Targeting == TargetType.AllEnemies);
        loc.Add("Self", Targeting == TargetType.Self);
    }
}
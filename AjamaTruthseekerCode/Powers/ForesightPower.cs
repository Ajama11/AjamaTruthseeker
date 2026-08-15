using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Singletons;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Powers;

public class ForesightPower() : AjamaTruthseekerPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        await PowerCmd.Decrement(this);
    }

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier,
        out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (canonicalPower is not ForesightPower) return false;
        if (target != Owner) return false;

        int totalAmountBeforeApplication = Owner.GetPowerAmount<ForesightPower>();
        int totalAmountAfterApplication = totalAmountBeforeApplication + (int) amount;

        if (totalAmountAfterApplication <= ForesightSingleton.MaxPreviewCards) return false;

        modifiedAmount = ForesightSingleton.MaxPreviewCards - totalAmountBeforeApplication;
        
        return true;
    }

    public override string CustomBigIconPath => ImageHelper.GetImagePath("powers/sentry_mode_power.png");
    public override string CustomPackedIconPath => ImageHelper.GetImagePath("atlases/power_atlas.sprites/sentry_mode_power.tres");
}
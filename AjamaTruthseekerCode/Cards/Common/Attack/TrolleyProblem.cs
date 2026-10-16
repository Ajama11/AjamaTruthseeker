using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Common.Attack;

public class TrolleyProblem() : AjamaTruthseekerCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    private const string Held = "Held";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, DamageProps.card),
        new DamageVar(Held, 4, DamageProps.cardUnpowered)
    ];

    public override List<NonDynamicEffect> MyNonDynamicEffects =>
    [
        NonDynamicEffect.Held
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                vfx: VfxCmd.giantHorizontalSlashPath,
                tmpSfx: TmpSfx.heavyAttack)
            .Execute(choiceContext);
    }

    public override async Task OnHeld(PlayerChoiceContext choiceContext)
    {
        VfxCmd.PlayOnSide(CombatSide.Enemy, VfxCmd.giantHorizontalSlashPath, CombatState!);
        SfxCmd.PlayTmp(TmpSfx.heavyAttack);
        
        await CreatureCmd.Damage(choiceContext,
            CombatState!.HittableEnemies, (DamageVar) DynamicVars[Held],
            Owner.Creature, this, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars[Held].UpgradeValueBy(2);
    }
}
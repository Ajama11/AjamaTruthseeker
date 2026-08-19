using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;
using static AjamaTruthseeker.AjamaTruthseekerCode.Utils.Shape;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Common;

public class DissociativeStrike() : AjamaTruthseekerCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy),
    IOnPotentiallyDefy
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, DamageProps.card),
        new ObscureVar(2),
        new ProveVar(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [

    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [

    ];

    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: VfxCmd.flyingSlashPath, tmpSfx: TmpSfx.heavyAttack)
            .Execute(choiceContext);

        MyActions.ObscureRandom(Owner, DynamicVars.Obscure.IntValue, [PileType.Draw]);
    }
    
    public Task OnDefy(CardModel card, PlayerChoiceContext choiceContext)
    {
        MyActions.ProveRandom(Owner, DynamicVars.Prove.IntValue, [PileType.Draw]);
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars.Prove.UpgradeValueBy(1);
    }
}
using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
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
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Ancient;

public class EverpresentTruths() : AjamaTruthseekerCard(1,
    CardType.Attack, CardRarity.Ancient,
    TargetType.AllEnemies),
    IOnDefyThis
{
    private bool AutoplayedFromDefy { get; set; }
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, DamageProps.card),
        new RepeatVar(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        MyEnums.Obscured
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play,
                DynamicVars.Repeat.IntValue,
                vfx: VfxCmd.flyingSlashPath,
                tmpSfx: TmpSfx.heavyAttack)
            .Execute(choiceContext);
    }

    public override CardLocation ModifyCardPlayResultLocation(CardModel card, bool isAutoPlay, ResourceInfo resources,
        CardLocation cardLocation)
    {
        if (!AutoplayedFromDefy) return cardLocation;
        if (cardLocation.pileType != PileType.Discard) return cardLocation;
        if (!isAutoPlay) return cardLocation;

        cardLocation.pileType = PileType.Hand;
        AutoplayedFromDefy = false;
        return cardLocation;
    }

    public async Task OnDefy(CardModel card, PlayerChoiceContext choiceContext)
    {
        AutoplayedFromDefy = true;
        await CardCmd.AutoPlay(choiceContext, this, null);
    }

    public override PileType? DestinationPileAfterDefy => null;

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
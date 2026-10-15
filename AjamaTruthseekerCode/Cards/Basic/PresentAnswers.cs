using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;
using static AjamaTruthseeker.AjamaTruthseekerCode.Utils.Shape;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Basic;

public class PresentAnswers() : AjamaTruthseekerCard(1,
    CardType.Attack, CardRarity.Basic,
    TargetType.AnyEnemy),
    IOnDefyThis, ITranscendenceCard
{
    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<DontCrashAncientOrobas>();
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, DamageProps.card),
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
                vfx: null,
                tmpSfx: null)
            .Execute(choiceContext);
    }
    
    public async Task OnDefy(CardModel card, PlayerChoiceContext choiceContext)
    {
        await CardCmd.AutoPlay(choiceContext, this, null);
    }

    public override PileType? DestinationPileAfterDefy => null;

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
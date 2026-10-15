using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Common.Skill;

public class Disorient() : AjamaTruthseekerCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyEnemy),
    IOnDefyThis
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    public override BundledHoverTipManager MyBundles =>
    [
        BundledHoverTipFactory.FromPower<WeakPower>()
    ];
    
    public override List<List<string>> BundleReorders =>
    [
        [
            BundledHoverTipFactory.FromPower<WeakPower>().Name,
            BundledHoverTipFactory.Static(MyEnums.Defy).Name
        ]
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        await CommonActions.Apply<WeakPower>(choiceContext, this, play);
    }
    
    public Task OnDefy(CardModel card, PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner) return;
        if (CombatManager.Instance.History.Entries.OfType<CardDiscardedEntry>().All(e => e.Card != this)) return;
        if (Pile == null) return;
        if (Pile.Type == PileType.Hand) return;

        await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<WeakPower>().UpgradeValueBy(1);
    }
}
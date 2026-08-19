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
using static AjamaTruthseeker.AjamaTruthseekerCode.Utils.Shape;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards.Common.Skill;

public class Disorient() : AjamaTruthseekerCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.None),
    IOnPotentiallyDefy
{
    public override TargetType TargetType => IsAbstractShaped ? TargetType.AnyEnemy : TargetType.None;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1)
    ];

    public override List<ShapeType> MyShapeEffects =>
    [
        ShapeType.Abstract
    ];

    public override IEnumerable<IHoverTip> MyHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DoShapeEffect(ShapeType.Abstract, choiceContext, async () =>
            await CommonActions.Apply<WeakPower>(choiceContext, this, play));
    }
    
    private bool IsAbstractShaped
    {
        get
        {
            if (!IsMutable) return false;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (Owner == null) return false;
            
            ShapedPower? shapedPower = Owner.Creature.GetPower<ShapedPower>();
            return shapedPower != null && shapedPower.IsShape(ShapeType.Abstract);
        }
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
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using AjamaTruthseeker.AjamaTruthseekerCode.Character;
using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using AjamaTruthseeker.AjamaTruthseekerCode.Nodes;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Cards;

[Pool(typeof(TruthseekerCardPool))]
public abstract class AjamaTruthseekerCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    
    public enum NonDynamicEffect
    {
        Rewrite,
        Drawn,
        Held
    }
    
    public virtual List<NonDynamicEffect> MyNonDynamicEffects => [];
    
    public virtual List<Shape.ShapeType> MyShapeEffects => [];

    public bool HasRewrite => MyNonDynamicEffects.Contains(NonDynamicEffect.Rewrite);
    public bool HasDrawnEffect => MyNonDynamicEffects.Contains(NonDynamicEffect.Drawn);
    public virtual bool HasForeseenEffect => this is IOnPotentiallyForeseen;
    public virtual bool HasDefyEffect => this is IOnPotentiallyDefy;
    public bool HasHeldEffect => MyNonDynamicEffects.Contains(NonDynamicEffect.Held);
    public bool HasShapeEffect => MyShapeEffects.Count != 0;

    public override bool HasTurnEndInHandEffect => HasHeldEffect;

    public virtual IEnumerable<IHoverTip> MyHoverTips => [];
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            IEnumerable<IHoverTip> start = [];
            IEnumerable<IHoverTip> beforeMiddle = [];
            IEnumerable<IHoverTip> middle = [..MyHoverTips];
            IEnumerable<IHoverTip> afterMiddle = [];
            IEnumerable<IHoverTip> end = [];

            #region Start

            if (Keywords.Contains(MyEnums.Obscured))
            {
                start = [..start, HoverTipFactory.FromKeyword(MyEnums.Obscured), HoverTipFactory.FromPower<ForesightPower>()];
            }
            
            if (Keywords.Contains(CardKeyword.Unplayable))
            {
                start = [..start, HoverTipFactory.FromKeyword(CardKeyword.Unplayable)];
            }
            
            if (Keywords.Contains(CardKeyword.Innate))
            {
                start = [..start, HoverTipFactory.FromKeyword(CardKeyword.Innate)];
            }
            
            if (Keywords.Contains(CardKeyword.Retain))
            {
                start = [..start, HoverTipFactory.FromKeyword(CardKeyword.Retain)];
            }
            
            if (Keywords.Contains(CardKeyword.Sly))
            {
                start = [..start, HoverTipFactory.FromKeyword(CardKeyword.Sly)];
            }
            
            if (Keywords.Contains(CardKeyword.Ethereal))
            {
                start = [..start, HoverTipFactory.FromKeyword(CardKeyword.Ethereal)];
            }

            #endregion
            
            #region Before Middle

            if (GainsBlock)
            {
                beforeMiddle = [..beforeMiddle, HoverTipFactory.Static(StaticHoverTip.Block)];
            }

            #endregion
            
            #region After Middle
            
            if (HasRewrite)
            {
                afterMiddle = [..afterMiddle, HoverTipFactory.Static(MyEnums.Rewrite), HoverTipFactory.FromPower<ForesightPower>()];
            }

            if (HasShapeEffect)
            {
                foreach (var shapeType in MyShapeEffects)
                {
                    afterMiddle = [..afterMiddle, ..Shape.GetHoverTips(shapeType)];
                }
            }
            
            if (HasDrawnEffect)
            {
                afterMiddle = [..afterMiddle, HoverTipFactory.Static(MyEnums.Drawn)];
            }
            
            if (HasForeseenEffect)
            {
                afterMiddle = [..afterMiddle, HoverTipFactory.Static(MyEnums.Foreseen), HoverTipFactory.FromPower<ForesightPower>()];
            }
            
            if (HasDefyEffect)
            {
                afterMiddle = [..afterMiddle, HoverTipFactory.Static(MyEnums.Defy)];
            }
            
            if (HasHeldEffect)
            {
                afterMiddle = [..afterMiddle, HoverTipFactory.Static(MyEnums.Held)];
            }

            #endregion

            #region End

            if (Keywords.Contains(CardKeyword.Exhaust))
            {
                end = [..end, HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
            }

            #endregion

            return [..start, ..beforeMiddle, ..middle, ..afterMiddle, ..end];
        }
    }

    public async Task OnDefyWrapper(CardModel card, PlayerChoiceContext choiceContext)
    {
        if (card != this) return;
        if (this is not IOnPotentiallyDefy thisDefy) return;

        await CardPileCmd.Add(this, PileType.Play);

        if (LocalContext.IsMe(Owner))
            await Cmd.CustomScaledWait(0.3f, 0.6f);

        await thisDefy.OnDefy(this, choiceContext);

        await CardPileCmd.Add(this, PileType.Discard.GetPile(Owner));

        await MyHooks.OnDefyActivated(card.CombatState!, choiceContext, this);
    }

    public async Task OnForeseenWrapper(CardModel card, PlayerChoiceContext choiceContext)
    {
        if (card != this) return;
        if (this is not IOnPotentiallyForeseen thisForeseen) return;

        AjamaTruthseekerCard dupe = (AjamaTruthseekerCard) CreateDupe(Owner);
        
        PreviewForeseen(await CardPileCmd.AddGeneratedCardToCombat(dupe, PileType.Play, null), 1f);

        // if (LocalContext.IsMe(Owner))
        // await Cmd.CustomScaledWait(1.0f, 1.0f);

        await thisForeseen.OnForeseen(choiceContext);
        await MyHooks.OnForeseenActivated(card.CombatState!, choiceContext, this);
    }

    #region ForeseenVfx
    
    private static void PreviewForeseen(
        CardPileAddResult result,
        float time = 1.2f,
        CardPreviewStyle style = CardPreviewStyle.HorizontalLayout)
    {
        if (TestMode.IsOn || CombatManager.Instance.IsEnding || !result.success || !LocalContext.IsMine(result.cardAdded)) 
            return;
        PreviewForeseenInternal(result.cardAdded, time, style);
    }

    private static void PreviewForeseenInternal(CardModel card,
        float time = 1.2f,
        CardPreviewStyle style = CardPreviewStyle.HorizontalLayout)
    {
        if (card.Pile == null) return; 
        if (TestMode.IsOn) return; 
        if (CombatManager.Instance.IsEnding) return; 
        if (!LocalContext.IsMine(card)) return; 
        if (CardCmd.GetTotalCardsBeingPreviewed() > 50) return;
        
        PileType pileType = card.Pile.Type; 
        Control? control;
        
        switch (style) 
        { 
            case CardPreviewStyle.HorizontalLayout: 
                control = pileType.IsCombatPile() ? NCombatRoom.Instance!.Ui.CardPreviewContainer : NRun.Instance?.GlobalUi.CardPreviewContainer; 
                break; 
            case CardPreviewStyle.MessyLayout:
                control = pileType.IsCombatPile() ? NCombatRoom.Instance!.Ui.MessyCardPreviewContainer : NRun.Instance?.GlobalUi.MessyCardPreviewContainer; 
                break;
            case CardPreviewStyle.EventLayout:
                if (pileType.IsCombatPile()) throw new InvalidOperationException();
                control = NRun.Instance?.GlobalUi.EventCardPreviewContainer;
                break;
            case CardPreviewStyle.GridLayout:
                if (pileType.IsCombatPile()) throw new InvalidOperationException();
                control = NRun.Instance?.GlobalUi.GridCardPreviewContainer;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof (style), $"Unexpected CardPreviewStyle {style}!");
        }
        
        if (control == null) return;
        
        if (style == CardPreviewStyle.HorizontalLayout && control.GetChildCount() > 5) 
            control = pileType.IsCombatPile() ? NCombatRoom.Instance!.Ui.MessyCardPreviewContainer : NRun.Instance!.GlobalUi.MessyCardPreviewContainer;
        
        NCard node = NCard.Create(card)!; 
        control.AddChildSafely(node); 
        node.UpdateVisuals(pileType, CardPreviewMode.Normal);
        
        TaskCompletionSource source = new TaskCompletionSource();
        
        Tween tween = node.CreateTween();
        
        tween.TweenProperty(node, (NodePath) "scale", new Vector2(0.85f, 0.85f), 0.25).From(Vector2.Zero)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);
        
        tween.TweenCallback(Callable.From((Action) (() => 
        { 
            ForesightNCardFlyVfx? child = null; 
            Node? parent2 = pileType != PileType.Deck ? card.Owner.Creature.GetVfxContainer() : NRun.Instance?.GlobalUi.TopBar.TrailContainer;
            
            if (parent2 != null) 
                child = ForesightNCardFlyVfx.ForesightCreate(node, card.Owner.Creature, card.Owner.Character.TrailPath);
            
            if (child != null && parent2 != null) 
            { 
                parent2.AddChildSafely(child); 
                TaskHelper.RunSafely(child.SwooshAwayCompletion!.Task.ContinueWith(_ => source.SetResult())); 
            }
            else 
            { 
                node.QueueFreeSafely(); 
                source.SetResult(); 
            }
        }))).SetDelay(time);
    }
    
    #endregion
    
    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        await OnHeld(choiceContext);
    }

    public virtual Task OnHeld(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card == this) await OnDrawn(choiceContext, card, fromHandDraw);
    }
    
    public virtual Task OnDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        return Task.CompletedTask;
    }
    
    public async Task DoShapeEffect(Shape.ShapeType shapeType, PlayerChoiceContext choiceContext, Func<Task> effect)
    {
        ShapedPower? shapedPower = Owner.Creature.GetPower<ShapedPower>();

        if (shapedPower != null && shapedPower.IsShape(shapeType))
        {
            await effect();
        }
        else
        {
            if (!Shape.IsRequired(shapeType))
            {
                await Shape.SetShape(Owner.Creature, shapeType, choiceContext);
            }
        }
    }
}
using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using AjamaTruthseeker.AjamaTruthseekerCode.Nodes;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using Timer = Godot.Timer;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Singletons;

public class RunScopeForesightSingleton() : CustomSingletonModel(HookType.Run)
{
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        await ModelDb.Singleton<ForesightSingleton>().RunScopeAfterRoomEntered(room);
    }
}

public class ForesightSingleton() : CustomSingletonModel(HookType.Combat)
{
    private List<ForesightNPreviewCardHolder> _previewHolders = [];
    private Dictionary<int, bool> _previewFocus = [];
    
    public const int MaxPreviewCards = 10;
    private const float PreviewDefaultScale = 0.4f;
    private const float PreviewSpacingY = -19f;
    private readonly Vector2 _previewPosition = new (65, 910);
    
    public static readonly SpireField<CardModel, bool> ForeseenThisDeckCycle = new(() => false);

    public async Task RunScopeAfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom combatRoom) return;

        foreach (var creature in combatRoom.Allies.Where(a => a.IsPlayer))
        {
            Player player = creature.Player!;
            
            CardPile drawPile = CardPile.Get(PileType.Draw, player)!;

            if (LocalContext.IsMe(player))
            {
                CreateCardPreviews(player, drawPile);
            }
            
            drawPile.ContentsChanged += async () => await HandleForesightContentsPotentiallyChanged(player, drawPile);
        }
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player.PlayerCombatState!.TurnNumber != 1) await HandleForesightContentsPotentiallyChanged(player, player.PlayerCombatState!.DrawPile);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if ((oldPileType == PileType.Draw && card.Pile?.Type != PileType.Draw) ||
            oldPileType == PileType.Play)
            ForeseenThisDeckCycle[card] = false;
        return Task.CompletedTask;
    }
    
    /// Update card previews whenever anyone plays a card, because of situations like Midnight updating its energy cost from another player's actions.
    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Player localPlayer = LocalContext.GetMe(cardPlay.Card.CombatState)!;
        // UpdateCardPreviews(localPlayer.PlayerCombatState!.DrawPile, localPlayer.Creature.GetPowerAmount<ForesightPower>());
        // return Task.CompletedTask;
        ICombatState? combatState = cardPlay.Card.Owner.Creature.CombatState;
        if (combatState == null) return;
        
        foreach (var player in combatState.Players)
        {
            await HandleForesightContentsPotentiallyChanged(player, player.PlayerCombatState!.DrawPile);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        // if (power is not ForesightPower) return Task.CompletedTask;
        //
        // Player localPlayer = LocalContext.GetMe(power.Owner.CombatState)!;
        // if (power.Owner != localPlayer.Creature) return Task.CompletedTask; 
        //
        // UpdateCardPreviews(localPlayer.PlayerCombatState!.DrawPile, localPlayer.Creature.GetPowerAmount<ForesightPower>());
        // return Task.CompletedTask;
        if (power is not ForesightPower) return;
        if (power.Owner.Player == null) return;
        
        await HandleForesightContentsPotentiallyChanged(power.Owner.Player, power.Owner.Player.PlayerCombatState!.DrawPile);
    }

    private async Task HandleForesightContentsPotentiallyChanged(Player player, CardPile drawPile)
    {
        int foresightAmount = player.Creature.GetPowerAmount<ForesightPower>();
        
        if (LocalContext.IsMe(player))
        {
            UpdateCardPreviews(drawPile, foresightAmount);
        }
        
        for (var i = 0; i < Math.Min(drawPile.Cards.Count, foresightAmount); i++)
        {
            var card = drawPile.Cards[i];

            if (ForeseenThisDeckCycle[card]) continue;
            
            ForeseenThisDeckCycle[card] = true;
            await MyHooks.OnPotentiallyForeseen(player.Creature.CombatState!, new ThrowingPlayerChoiceContext(), card);
        }
    }
    
    private void CreateCardPreviews(Player player, CardPile drawPile)
    {
        var ui = NCombatRoom.Instance!.Ui;
        Control holderParent = new Control();
        
        ui.AddChildSafely(holderParent);
        ui.MoveChild(holderParent, 0);

        Timer refreshTimer = new Timer();
        refreshTimer.WaitTime = 1;
        refreshTimer.OneShot = false;
        refreshTimer.Timeout += () => RefreshTimerOnTimeout(player, drawPile);
        
        ui.AddChildSafely(refreshTimer);
        refreshTimer.Start();
        
        for (var i = 0; i < MaxPreviewCards; i++)
        {
            var yOffset = i * PreviewSpacingY;
            var holder = CreatePreviewCard(ModelDb.Card<StrikeIronclad>(), i, yOffset, PreviewDefaultScale, holderParent);
            if (holder == null) continue;
            
            _previewHolders.Add(holder);
            _previewFocus.Add(i, false);
            
            holder.Focused += OnHolderFocused;
            holder.Unfocused += OnHolderUnfocused;
            holder.TreeExiting += RemoveCardPreviews;
        }
    }

    private void RefreshTimerOnTimeout(Player player, CardPile drawPile)
    {
        if (LocalContext.IsMe(player))
        {
            UpdateCardPreviews(drawPile, player.Creature.GetPowerAmount<ForesightPower>());
        }
    }

    private ForesightNPreviewCardHolder? CreatePreviewCard(CardModel card, int index, float yOffset, float scale, Control parent)
    {
        var cardNode = NCard.Create(card);
        if (cardNode == null) return null;

        ForesightNPreviewCardHolder holder =
            ForesightNPreviewCardHolder.ForesightCreate(cardNode, true, true);
        
        parent.AddChildSafely(holder);
        parent.MoveChild(holder, 0);
        
        holder.Visible = false;
        holder.Index = index;
        
        holder.MouseFilter = Control.MouseFilterEnum.Ignore;
        holder.FocusMode = Control.FocusModeEnum.None;
        holder.Hitbox.MouseFilter = Control.MouseFilterEnum.Pass;
        
        holder.SetCardScale(new Vector2(scale, scale));
        holder.OriginalPosition = _previewPosition + new Vector2(0, yOffset);
        holder.GlobalPosition = holder.OriginalPosition;

        holder.Hitbox.Size = holder.Hitbox.Size with {X = holder.Hitbox.Size.X + 200};
        holder.Hitbox.Position = holder.Hitbox.Position with {X = holder.Hitbox.Position.X - 200};
        
        cardNode.UpdateVisuals(PileType.Draw, CardPreviewMode.Normal);

        return holder;
    }

    private void UpdateCardPreviews(CardPile drawPile, int foresightAmount)
    {
        int amountOfVisibleHolders = Math.Min(Math.Min(foresightAmount, drawPile.Cards.Count), MaxPreviewCards);

        foreach (ForesightNPreviewCardHolder holder in _previewHolders)
        {
            if (holder.Index >= amountOfVisibleHolders) // Index and drawPile.Cards[] are 0-indexed while amountOfVisibleHolders and its parts are 1-indexed, hence =
            {
                holder.Visible = false;
            }
            else
            {
                holder.Visible = true;
                
                CardModel cardToDisplay = !drawPile.Cards[holder.Index].Keywords.Contains(MyEnums.Obscured) || Proven.IsProven(drawPile.Cards[holder.Index])
                    ? drawPile.Cards[holder.Index]
                    : Obscured.Disguise[drawPile.Cards[holder.Index]]!;
                
                holder.ReassignToCard(cardToDisplay, PileType.Draw, null, ModelVisibility.Visible);
            }
        }
    }
    
    private void OnHolderFocused(int index)
    {
        _previewFocus[index] = true;
        HandleHolderOpacity();
    }
    
    private void OnHolderUnfocused(int index)
    {
        _previewFocus[index] = false;
        HandleHolderOpacity();
    }
    
    private void HandleHolderOpacity()
    {
        Color gray = new(0.5f, 0.5f, 0.5f);
        Color white = new(1f, 1f, 1f);
        
        if (_previewFocus.Any(p => p.Value))
        {
            foreach (KeyValuePair<int, bool> pair in _previewFocus)
            {
                _previewHolders[pair.Key].SetModulate(pair.Value ? white : gray);
                _previewHolders[pair.Key].ZIndex = pair.Value ? 1 : 0;
            }
        }
        else
        {
            foreach (ForesightNPreviewCardHolder holder in _previewHolders)
            {
                holder.SetModulate(white);
                holder.ZIndex = 0;
            }
        }
    }

    private void RemoveCardPreviews()
    { 
        _previewHolders.Clear();
        _previewFocus.Clear();
    }
}
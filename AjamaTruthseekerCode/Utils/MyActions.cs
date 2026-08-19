using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;
using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Singletons;
using BaseLib.Commands;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Utils;

public static class MyActions
{
    /// <summary>
    /// BaseLib Scry with some changes. Does all the discards at once so that I can have "Whenever you Defy, draw 1 card". Disguises Obscured cards.
    /// </summary>
    public static async Task Rewrite(PlayerChoiceContext choiceContext, Player player, int amountOverride = -1)
    {
        int amount = amountOverride != -1 ? amountOverride : player.Creature.GetPowerAmount<ForesightPower>();
        
        var drawPile = PileType.Draw.GetPile(player);
        var discardPile = PileType.Discard.GetPile(player);
        
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        
        var cardsToRewrite = drawPile.Cards.Take(amount).ToList();
        if (cardsToRewrite.Count == 0) return;

        for (var i = 0; i < cardsToRewrite.ToList().Count; i++)
        {
            CardModel card = cardsToRewrite[i];
            if (!card.Keywords.Contains(MyEnums.Obscured)) continue;
            if (Proven.IsProven(card)) continue;
            
            CardModel fakeCard = Obscured.Disguise[card]!;
            Obscured.Disguise[fakeCard] = card;

            cardsToRewrite.RemoveAt(i);
            cardsToRewrite.Insert(i, fakeCard);
        }

        var prefs = new CardSelectorPrefs(
            CardSelectorPrefs.DiscardSelectionPrompt,
            0,
            cardsToRewrite.Count
        )
        {
            ShouldGlowGold = c => c is IOnPotentiallyDefy
        };

        var cardsToDiscard = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            cardsToRewrite,
            player,
            prefs
        )).ToList();
        
        for (var i = 0; i < cardsToDiscard.ToList().Count; i++)
        {
            CardModel card = cardsToDiscard[i];
            if (Obscured.Disguise[card] == null) continue;
            if (Proven.IsProven(card)) continue;

            cardsToDiscard.RemoveAt(i);
            cardsToDiscard.Insert(i, Obscured.Disguise[card]!);
        }

        // A crucial difference between Scry and Rewrite!
        // All discarded cards need to be discarded before Hook.AfterCardDiscarded!
        await CardPileCmd.Add(cardsToDiscard, discardPile);
        
        foreach (var card in cardsToDiscard)
        {
            CombatManager.Instance.History.CardDiscarded(combatState, card);
            await Hook.AfterCardDiscarded(combatState, choiceContext, card);
        }
        discardPile.InvokeContentsChanged();
    }
    
    public static async Task<IEnumerable<CardModel>> CreateCards(CardModel canonicalCard, int amount,
        AjamaTruthseekerCard sourceCard, PileType pile = PileType.Hand, CardPilePosition position = CardPilePosition.Bottom, bool preview = true, float previewTime = 1.2f, Func<List<CardModel>, List<CardModel>>? modifyCardsBeforePreview = null)
    {
        return await CreateCards(canonicalCard, amount, sourceCard.Owner, sourceCard.CombatState!, pile, position, preview, previewTime, modifyCardsBeforePreview);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateCards(CardModel canonicalCard, int amount, Player owner, ICombatState combatState, PileType pile = PileType.Hand, CardPilePosition position = CardPilePosition.Bottom, bool preview = true, float previewTime = 1.2f, Func<List<CardModel>, List<CardModel>>? modifyCardsBeforePreview = null)
    {
        if (amount == 0 || CombatManager.Instance.IsOverOrEnding)
        {
            return [];
        }

        List<CardModel> cards = [];

        for (int i = 0; i < amount; i++)
        {
            cards.Add(combatState.CreateCard(canonicalCard, owner));
        }

        if (modifyCardsBeforePreview != null)
        {
            cards = modifyCardsBeforePreview(cards);
        }

        IReadOnlyList<CardPileAddResult> results = await CardPileCmd.AddGeneratedCardsToCombat(cards, pile, owner, position);

        if (pile != PileType.Hand && preview) CardCmd.PreviewCardPileAdd(results, previewTime);

        return cards;
    }
    
    public static List<CardModel> GetRandomCards(Player player, int amount, List<PileType>? piles = null, Func<CardModel?, bool>? filter = null, MyEnums.PositionForesight positionForesight = MyEnums.PositionForesight.DontCare)
    {
        piles ??= [PileType.Draw];
        filter ??= _ => true; 

        List<PileType> pilesToUse = piles.ToList();
        List<CardModel> potentialCards = [];

        // If we're Excluding Foresight cards, remove the Draw Pile from the list of piles to use, and append the cards that are outside of Foresight.
        if (positionForesight is MyEnums.PositionForesight.TryExclude or MyEnums.PositionForesight.Exclude &&
            piles.Contains(PileType.Draw) &&
            player.Creature.GetPowerAmount<ForesightPower>() > 0)
        {
            pilesToUse.Remove(PileType.Draw);

            List<CardModel> nonForesightDrawPile = PileType.Draw.GetPile(player).Cards
                .Skip(player.Creature.GetPowerAmount<ForesightPower>())
                .ToList();
            
            potentialCards = [..potentialCards, ..nonForesightDrawPile.Where(filter)];
        }

        // If we're Only Including Foresight cards, remove the Draw Pile from the list of piles to use, and append the cards in Foresight. 
        if (positionForesight == MyEnums.PositionForesight.OnlyInclude &&
            piles.Contains(PileType.Draw) &&
            player.Creature.GetPowerAmount<ForesightPower>() > 0)
        {
            pilesToUse.Remove(PileType.Draw);
            
            var foresightCards = PileType.Draw.GetPile(player).Cards
                .Chunk(player.Creature.GetPowerAmount<ForesightPower>())
                .FirstOrDefault();

            if (foresightCards != null)
            {
                potentialCards = [..potentialCards, ..foresightCards.Where(filter)];
            }
        }

        // Grab the cards from the piles we're using. If we're doing Foresight shenanigans, the Draw Pile has been removed from this and already dealt with.
        potentialCards = pilesToUse.Aggregate(potentialCards, (current, pile) => 
            [..current, ..pile.GetPile(player).Cards.Where(filter)]);

        // If we're Trying to Exclude but we're short on cards to grab, then reintroduce the Foresight cards to the party to fill in the gaps.
        // The only way we're still short after this is if there's straight up not enough cards to grab, which is expected behavior.
        if (positionForesight == MyEnums.PositionForesight.TryExclude &&
            piles.Contains(PileType.Draw) && 
            potentialCards.Count < amount &&
            player.Creature.GetPowerAmount<ForesightPower>() > 0)
        {
            var foresightCards = PileType.Draw.GetPile(player).Cards
                .Chunk(player.Creature.GetPowerAmount<ForesightPower>())
                .FirstOrDefault();

            if (foresightCards != null)
            {
                potentialCards = [..potentialCards, ..foresightCards.Where(filter)];
            }
        }

        List<CardModel> cards = potentialCards
            .TakeRandom(amount, player.RunState.Rng.CombatCardSelection)
            .ToList();

        return cards;
    }

    public static async Task Prove(PlayerChoiceContext choiceContext, AjamaTruthseekerCard sourceCard,
        PileType[]? piles = null, int amountOverride = -1)
    {
        piles ??= [PileType.Draw];
        
        int amount = amountOverride == -1 ? sourceCard.DynamicVars.Prove.IntValue : amountOverride;

        await Prove(sourceCard, choiceContext, sourceCard.Owner, amount, piles);
    }

    public static async Task Prove(AbstractModel sourceModel, PlayerChoiceContext choiceContext, Player player,
        int amount, PileType[]? piles = null)
    {
        await ProveOrCertifyInternal(sourceModel, choiceContext, player, amount, false, piles);
    }
    
    public static async Task Certify(PlayerChoiceContext choiceContext, AjamaTruthseekerCard sourceCard,
        PileType[]? piles = null, int amountOverride = -1)
    {
        piles ??= [PileType.Draw];
        
        int amount = amountOverride == -1 ? sourceCard.DynamicVars.Certify.IntValue : amountOverride;

        await Certify(sourceCard, choiceContext, sourceCard.Owner, amount, piles);
    }

    public static async Task Certify(AbstractModel sourceModel, PlayerChoiceContext choiceContext, Player player,
        int amount, PileType[]? piles = null)
    {
        await ProveOrCertifyInternal(sourceModel, choiceContext, player, amount, true, piles);
    }
    
    private static async Task ProveOrCertifyInternal(AbstractModel sourceModel, PlayerChoiceContext choiceContext,
        Player player, int amount, bool isCertify, PileType[]? piles = null)
    {
        piles ??= [PileType.Draw];
        
        Func<CardModel?, bool> filter = isCertify ? Proven.IsAbleToBeCertified : Proven.IsAbleToBeProven;
        LocString selectionPrompt = isCertify ? MySelectionPrompts.Certify : MySelectionPrompts.Prove;
        
        CardSelectorPrefs prefs = new CardSelectorPrefs(selectionPrompt, amount);
        List<CardModel> cards;

        if (piles.Length > 1)
        {
            cards = (await MultiPileCardSelect.Select(choiceContext, player, prefs,
                    filter, piles))
                .ToList();
        }
        else
        {
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (piles[0] == PileType.Hand)
            {
                cards = (await CardSelectCmd.FromHand(choiceContext, player, prefs, filter, sourceModel))
                    .ToList();
            }
            else
            {
                cards = (await CardSelectCmd.FromCombatPile(choiceContext, piles[0].GetPile(player), player, prefs, filter))
                    .ToList();
            }
        }
        
        Proven.SetProven(cards, true);
        if (LocalContext.IsMe(player)) MySounds.Prove.Play();
    }

    public static void ProveRandom(Player player, int amount, List<PileType>? piles = null, MyEnums.PositionForesight positionForesight = MyEnums.PositionForesight.DontCare)
    {
        piles ??= [PileType.Draw];

        List<CardModel> cards = GetRandomCards(player, amount, piles, Proven.IsAbleToBeProven, positionForesight);
        
        if (cards.Count == 0) return;
        
        Proven.SetProven(cards, true);
        CardCmd.Preview(cards);
        if (LocalContext.IsMe(player)) MySounds.Prove.Play();
    }

    public static void Obscure(List<CardModel> cards, bool preview = true, bool playSfx = true)
    {
        foreach (CardModel card in cards)
        {
            Obscure(card, preview, false);
        }
        if (playSfx && LocalContext.IsMe(cards.FirstOrDefault()?.Owner)) MySounds.Obscure.Play();
    }
    
    public static void Obscure(CardModel card, bool preview = true, bool playSfx = true)
    {
        Obscured.SetObscure(card, true);
        if (preview && card.Pile?.Type != PileType.Hand) CardCmd.Preview(card);
        if (playSfx && LocalContext.IsMe(card.Owner)) MySounds.Obscure.Play();
    }
    
    public static void ObscureRandom(Player player, int amount, List<PileType>? piles = null,
        MyEnums.PositionForesight positionForesight = MyEnums.PositionForesight.TryExclude)
    {
        piles ??= [PileType.Draw];

        List<CardModel> cards = GetRandomCards(player, amount, piles,
            Proven.IsAbleToBeProven, positionForesight);
        
        if (cards.Count == 0) return;
        
        Obscure(cards);
    }
}
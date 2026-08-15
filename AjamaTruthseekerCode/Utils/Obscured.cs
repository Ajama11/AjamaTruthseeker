using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Utils;

public class Obscured
{
    public static readonly SpireField<CardModel, CardModel?> Disguise = new(() => null);

    private const int MaxAmountOfRerollsWhenDisguisedAsSameCard = 10;

    public static void CreateDisguise(CardModel card)
    {
        CardModel? disguiseBase = null;

        for (int i = 0; i < MaxAmountOfRerollsWhenDisguisedAsSameCard; i++)
        {
            disguiseBase = card.Owner.RunState.Rng.CombatCardSelection.NextItem(card.Owner.Deck.Cards);
            if (disguiseBase == null) break;
            if (disguiseBase.Id != card.Id) break;
        }

        disguiseBase ??= card.Owner.Creature.CombatState!.CreateCard(
            card.Owner.RunState.Rng.CombatCardSelection.NextItem(
                card.Owner.Character.StartingDeck.Where(c => c.Id != card.Id))!, card.Owner);
        
        CardModel disguise = card.Owner.Creature.CombatState!.CloneCard(disguiseBase);
        
        Proven.SetProven(disguise, false);
        
        Disguise[card] = disguise;
    }

    public static bool IsAbleToObscure(CardModel? card)
    {
        return card != null && !card.Keywords.Contains(MyEnums.Obscured) && !Proven.IsProven(card);
    }

    public static void SetObscure(CardModel card, bool value)
    {
        if (value)
        {
            if (!IsAbleToObscure(card)) return;
            CreateDisguise(card);
            card.AddKeyword(MyEnums.Obscured);
        }
        else
        {
            card.RemoveKeyword(MyEnums.Obscured);
            Disguise[card] = null;
        }
    }
}
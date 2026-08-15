using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.Nodes;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Utils;

public class Proven
{
    private static readonly SavedSpireField<CardModel, bool> _isProvenField =
        (SavedSpireField<CardModel, bool>)
        new SavedSpireField<CardModel, bool>(() => false, "AjamaTruthseekerProven")
            .CopyOnClone();
    
    public static AddedNode<NCard, NTruthseekerProven> ProvenNode = new(
        "res://AjamaTruthseeker/scenes/proven_checkmark.tscn",
        (card, display) =>
        {
            display.Visible = IsProven(card.Model);
            
            var cardContainer = card.GetChild(0)!;
            cardContainer.AddChild(display);
        }
    );

    public static void UpdateProven(NCard nCard)
    {
        ProvenNode[nCard].Visible = IsProven(nCard.Model);
    }

    public static void SetProven(List<CardModel> cards, bool value)
    {
        foreach (CardModel card in cards)
        {
            SetProven(card, value);
        }
    }

    public static void SetProven(CardModel card, bool value)
    {
        _isProvenField[card] = value;
        
        NCard? nCard = NCard.FindOnTable(card);
        if (nCard == null) return;
        
        ProvenNode[nCard].Visible = value;
    }

    public static bool IsProven(CardModel? card)
    {
        return card != null && _isProvenField[card];
    }
    
    public static bool IsAbleToBeProven(CardModel? card)
    {
        return IsAbleToBeCertified(card) && !card!.Keywords.Contains(MyEnums.Obscured);
    }
    
    public static bool IsAbleToBeCertified(CardModel? card)
    {
        return card != null && !_isProvenField[card];
    }
}
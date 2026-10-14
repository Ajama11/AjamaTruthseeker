using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.Nodes;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Utils;

public class Proven
{
    private static readonly SavedSpireField<CardModel, bool> _isProvenField =
        (SavedSpireField<CardModel, bool>)
        new SavedSpireField<CardModel, bool>(() => false, "AjamaTruthseekerProven")
            .CopyOnClone();

    public static void UpdateProven(NCard nCard)
    {
        if (IsProven(nCard.Model) && nCard._titleLabel.IsValid())
        {
            nCard._titleLabel.SetTextAutoSize($"✅{nCard._titleLabel.Text}");
        }
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
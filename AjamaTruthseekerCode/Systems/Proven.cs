using AjamaTruthseeker.AjamaTruthseekerCode.Nodes;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Systems;

public class Proven
{
    public enum SusStatus
    {
        None,
        Flag,
        Genuine
    }
    
    private static readonly SavedSpireField<CardModel, SusStatus> _susField =
        (SavedSpireField<CardModel, SusStatus>)
        new SavedSpireField<CardModel, SusStatus>(() => SusStatus.None, "AjamaTruthseekerSusStatus")
            .CopyOnClone();

    public static readonly AddedNode<NCard, NSusMarker> CheckmarkNode = new(
        "res://AjamaTruthseeker/scenes/sus_marker.tscn",
        (nCard, marker) =>
        {
            nCard.GetNode("CardContainer").AddChild(marker); // Apparently .Body is null right now
        }
    );
    
    public static readonly AddedNode<NPreviewCardHolder, NSusMarker> FlagNode = new(
        "res://AjamaTruthseeker/scenes/sus_marker.tscn"
    );

    public static void UpdateCheckmark(NCard nCard)
    {
        if (nCard.Model == null) return;
        
        CheckmarkNode[nCard].Checkmark?.Visible = _susField[nCard.Model] == SusStatus.Genuine;
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
        _susField[card] = value ? SusStatus.Genuine : _susField[card];
    }

    public static void SetFlag(CardModel card, bool value)
    {
        _susField[card] = value ? SusStatus.Flag : SusStatus.None;
    }

    public static void ToggleFlag(CardModel card)
    {
        if (_susField[card] == SusStatus.Genuine) return;
        
        _susField[card] = _susField[card] == SusStatus.None ?
            SusStatus.Flag :
            SusStatus.None;

        if (Obscured.Disguise[card] != null)
        {
            _susField[Obscured.Disguise[card]!] = _susField[Obscured.Disguise[card]!] == SusStatus.None ?
                SusStatus.Flag :
                SusStatus.None;
        }
    }

    public static bool IsProven(CardModel? card)
    {
        return card != null && _susField[card] == SusStatus.Genuine;
    }

    public static bool IsFlagged(CardModel? card)
    {
        return card != null && _susField[card] == SusStatus.Flag;
    }
    
    public static bool IsAbleToBeProven(CardModel? card)
    {
        return IsAbleToBeCertified(card) && !card!.Keywords.Contains(MyEnums.Obscured);
    }
    
    public static bool IsAbleToBeCertified(CardModel? card)
    {
        return card != null && _susField[card] != SusStatus.Genuine;
    }
}
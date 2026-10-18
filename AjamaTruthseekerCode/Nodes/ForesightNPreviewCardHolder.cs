using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using AjamaTruthseeker.AjamaTruthseekerCode.Systems;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Nodes;

public partial class ForesightNPreviewCardHolder : NPreviewCardHolder
{
    protected override Vector2 HoverScale => _originalScale * 1.66f;
    private readonly Vector2 _focusOffset = new(44, 40);
    
    private Tween? _positionTween;

    public int Index;
    public Vector2 OriginalPosition;
    
    public event Action<int>? Focused;
    public event Action<int>? Unfocused;

    public static ForesightNPreviewCardHolder ForesightCreate(NCard card, bool showHoverTips, bool scaleOnHover)
    {
        GodotObject? originalHolder = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NPreviewCardHolder>();

        var godotObjectId = originalHolder.GetInstanceId();
        originalHolder.SetScript(ResourceLoader.Load("AjamaTruthseekerCode/Nodes/ForesightNPreviewCardHolder.cs"));
        ForesightNPreviewCardHolder newHolder = (InstanceFromId(godotObjectId) as ForesightNPreviewCardHolder)!;
        
        newHolder.Initialize(card, showHoverTips, scaleOnHover);
        newHolder.Pressed += OnPressed;
        return newHolder;
    }

    protected override void OnFocus()
    {
        _positionTween?.Kill();
        GlobalPosition = OriginalPosition + _focusOffset;
        
        // base.OnFocus() creates HoverTips based off of GlobalPosition, so it has to happen after we change GlobalPosition
        base.OnFocus();
        Focused?.Invoke(Index);
    }
    
    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        Unfocused?.Invoke(Index);
        
        _positionTween?.Kill();
        _positionTween = CreateTween();
        _positionTween.TweenProperty(this, "global_position", OriginalPosition, 0.5)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
    }
    
    private static void OnPressed(NCardHolder cardHolder)
    {
        MainFile.Logger.Warn($"{cardHolder.CardModel?.Title} was pressed");
        // Proven.SetProven(cardHolder.CardModel!, true);
    }
}
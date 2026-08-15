using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Nodes;

public partial class ForesightNPreviewCardHolder : NPreviewCardHolder
{
    protected override Vector2 HoverScale => _originalScale * 1.5f;
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
        return newHolder;
    }
    
    protected override void OnFocus()
    {
        base.OnFocus();
        Focused?.Invoke(Index);
        
        _positionTween?.Kill();
        GlobalPosition = OriginalPosition + new Vector2(36, 28);
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
}
using AjamaTruthseeker.AjamaTruthseekerCode.Systems;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Patches;

[HarmonyPatch(typeof(NCard))]
public static class NCardPatches
{
    [HarmonyPatch(nameof(NCard._Ready))]
    [HarmonyPostfix]
    public static void ReadyPostfix(NCard __instance)
    {
        // I just have to hope no one's adding something as a child of Frame or AncientBorderGlassOverlay and /not/ wanting it clipped
        var obscuredNormalOverlay = PreloadManager.Cache.GetScene("res://AjamaTruthseeker/scenes/obscured_card_overlay.tscn")
            .Instantiate<TextureRect>();
        var obscuredAncientOverlay = PreloadManager.Cache.GetScene("res://AjamaTruthseeker/scenes/obscured_card_overlay.tscn")
            .Instantiate<TextureRect>();

        __instance._frame.AddChildSafely(obscuredNormalOverlay);
        __instance._frame.ClipChildren = CanvasItem.ClipChildrenMode.AndDraw;
        obscuredNormalOverlay.Visible = false;
        
        __instance._ancientBorderGlassOverlay.AddChildSafely(obscuredAncientOverlay);
        __instance._ancientBorderGlassOverlay.ClipChildren = CanvasItem.ClipChildrenMode.AndDraw;
        obscuredAncientOverlay.Visible = false;
    }
    
    [HarmonyPatch(nameof(NCard.UpdateVisuals))]
    [HarmonyPostfix]
    public static void UpdateVisualsPostfix(NCard __instance)
    {
        Proven.UpdateProven(__instance);
        
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        TextureRect? obscuredNormalOverlay = __instance._frame?.GetNodeOrNull<TextureRect>("AjamaTruthseekerObscured");
        
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        TextureRect? obscuredAncientOverlay = __instance._ancientBorderGlassOverlay?.GetNodeOrNull<TextureRect>("AjamaTruthseekerObscured");

        bool shouldBeVisible = __instance.Model != null &&
                               __instance.Model.Keywords.Contains(MyEnums.Obscured);

        obscuredNormalOverlay?.Visible = shouldBeVisible;
        obscuredAncientOverlay?.Visible = shouldBeVisible;
    }
    
    [HarmonyPatch(nameof(NCard.Reload))]
    [HarmonyPostfix]
    public static void ReloadPostfix(NCard __instance)
    {
        Proven.UpdateProven(__instance);
    }
}
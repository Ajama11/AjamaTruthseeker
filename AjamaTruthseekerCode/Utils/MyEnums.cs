using System.Diagnostics.CodeAnalysis;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
// ReSharper disable UnassignedField.Global

namespace AjamaTruthseeker.AjamaTruthseekerCode.Utils;

[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible")]
public class MyEnums
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Obscured;
    
    [CustomEnum]
    public static StaticHoverTip Prove;
    
    [CustomEnum]
    public static StaticHoverTip Certify;
    
    [CustomEnum]
    public static StaticHoverTip Rewrite;
    
    [CustomEnum]
    public static StaticHoverTip Obscure;
    
    [CustomEnum]
    public static StaticHoverTip Foreseen;
    
    [CustomEnum]
    public static StaticHoverTip Defy;
    
    [CustomEnum]
    public static StaticHoverTip Drawn;
    
    [CustomEnum]
    public static StaticHoverTip Held;
    
    [CustomEnum]
    public static StaticHoverTip WeaponShape;
    [CustomEnum]
    public static StaticHoverTip WeaponRequired;
    
    [CustomEnum]
    public static StaticHoverTip FriendShape;
    [CustomEnum]
    public static StaticHoverTip FriendRequired;
    
    [CustomEnum]
    public static StaticHoverTip AbstractShape;
    [CustomEnum]
    public static StaticHoverTip AbstractRequired;

    [CustomEnum]
    public static CardPilePosition RandomOutsideForesight;
    
    public enum ExcludeForesight
    {
        No,
        Try,
        Yes
    }
}
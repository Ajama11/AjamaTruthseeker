using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;

public static class BundledHoverTipFactory
{
    public static BundledHoverTip FromKeyword(CardKeyword keyword, BundledHoverTipManager.Category category = BundledHoverTipManager.Category.Middle, string? name = null)
    {
        return new BundledHoverTip(
            name ?? keyword.GetTitle().GetRawText(),
            HoverTipFactory.FromKeyword(keyword),
            category
        );
    }
    
    public static BundledHoverTip Static(StaticHoverTip tip, BundledHoverTipManager.Category category = BundledHoverTipManager.Category.Middle, string? name = null)
    {
        return new BundledHoverTip(
            name ?? tip.ToString(),
            HoverTipFactory.Static(tip),
            category
        );
    }
    
    public static BundledHoverTip FromPower<T>(int? amount = null, BundledHoverTipManager.Category category = BundledHoverTipManager.Category.Middle, string? name = null) where T : PowerModel
    {
        return new BundledHoverTip(
            name ?? typeof(T).Name,
            HoverTipFactory.FromPower<T>(amount),
            category
        );
    }
    
    public static BundledHoverTip FromCard<T>(bool upgrade = false, BundledHoverTipManager.Category category = BundledHoverTipManager.Category.Middle, string? name = null) where T : CardModel
    {
        return new BundledHoverTip(
            name ?? typeof(T).Name,
            HoverTipFactory.FromCard<T>(upgrade),
            category
        );
    }
    
    public static BundledHoverTip FromCard(CardModel card, bool upgrade = false, BundledHoverTipManager.Category category = BundledHoverTipManager.Category.Middle, string? name = null)
    {
        return new BundledHoverTip(
            name ?? card.TitleLocString.GetRawText(),
            HoverTipFactory.FromCard(card, upgrade),
            category
        );
    }
}
using MegaCrit.Sts2.Core.HoverTips;
using static AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core.BundledHoverTipManager;

namespace AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;

public class BundledHoverTip
{
    public string Name;
    
    public Category SortCategory = Category.Middle;
    public int SortOrder = 0;
    
    public IEnumerable<IHoverTip> HoverTips;

    public int InternalSortOrder => SortOrder + (int) SortCategory;

    public BundledHoverTip(string name, IHoverTip hoverTip)
    {
        Name = name;
        HoverTips = [hoverTip];
    }
    
    public BundledHoverTip(string name, IEnumerable<IHoverTip> hoverTips)
    {
        Name = name;
        HoverTips = hoverTips;
    }
    
    public BundledHoverTip(string name, IHoverTip hoverTip, Category sortCategory)
    {
        Name = name;
        HoverTips = [hoverTip];
        SortCategory = sortCategory;
    }
    
    public BundledHoverTip(string name, IEnumerable<IHoverTip> hoverTips, Category sortCategory)
    {
        Name = name;
        HoverTips = hoverTips;
        SortCategory = sortCategory;
    }
}
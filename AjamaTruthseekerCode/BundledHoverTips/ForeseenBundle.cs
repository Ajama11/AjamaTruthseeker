using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips;

public class ForeseenBundle(BundledHoverTipManager.Category category = BundledHoverTipManager.Category.Middle) : BundledHoverTip(
    nameof(ForeseenBundle),
    [
        HoverTipFactory.Static(MyEnums.Foreseen),
        HoverTipFactory.FromPower<ForesightPower>()
    ],
    category
)
{ }
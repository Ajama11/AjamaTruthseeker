using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips;

public class ProveBundle(BundledHoverTipManager.Category category = BundledHoverTipManager.Category.Middle) : BundledHoverTip(
    nameof(ProveBundle),
    [
        HoverTipFactory.Static(MyEnums.Prove),
        HoverTipFactory.FromPower<ForesightPower>(),
        HoverTipFactory.FromKeyword(MyEnums.Obscured)
    ],
    category
)
{ }
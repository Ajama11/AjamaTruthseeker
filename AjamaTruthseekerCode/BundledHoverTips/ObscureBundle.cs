using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips;

public class ObscureBundle(BundledHoverTipManager.Category category = BundledHoverTipManager.Category.Middle) : BundledHoverTip(
    nameof(ObscureBundle),
    [
        HoverTipFactory.Static(MyEnums.Obscure),
        HoverTipFactory.FromKeyword(MyEnums.Obscured),
        HoverTipFactory.FromPower<ForesightPower>()
    ],
    category
)
{ }
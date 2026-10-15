using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips;

public class ObscuredBundle(BundledHoverTipManager.Category category = BundledHoverTipManager.Category.Start) : BundledHoverTip(
    nameof(ObscuredBundle),
    [
        HoverTipFactory.FromKeyword(MyEnums.Obscured),
        HoverTipFactory.FromPower<ForesightPower>()
    ],
    category
)
{ }
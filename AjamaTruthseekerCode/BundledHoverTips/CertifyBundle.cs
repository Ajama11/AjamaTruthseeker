using AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips.Core;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using MegaCrit.Sts2.Core.HoverTips;

namespace AjamaTruthseeker.AjamaTruthseekerCode.BundledHoverTips;

public class CertifyBundle(BundledHoverTipManager.Category category = BundledHoverTipManager.Category.Middle) : BundledHoverTip(
    nameof(CertifyBundle),
    [
        HoverTipFactory.Static(MyEnums.Certify),
        HoverTipFactory.FromPower<ForesightPower>(),
        HoverTipFactory.FromKeyword(MyEnums.Obscured)
    ],
    category
)
{ }
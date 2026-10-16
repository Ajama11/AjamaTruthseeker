using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;

public class MyPowerVar<T>(string name, decimal baseValue, bool skipTooltip = false) : PowerVar<T>(name, baseValue), IPowerHoverTip where T : PowerModel
{
    public bool SkipTooltip = skipTooltip;

    public MyPowerVar(decimal baseValue, bool skipTooltip = false) : this(typeof(T).Name, baseValue, skipTooltip) { }
    
    public PowerHoverTip GetPowerHoverTip()
    {
        return new PowerHoverTip()
        {
            Name = Name,
            HoverTip = HoverTipFactory.FromPower<T>(),
            SkipTooltip = SkipTooltip
        };
    }
}

public struct PowerHoverTip
{
    public string Name;
    public IHoverTip HoverTip;
    public bool SkipTooltip;
}

public interface IPowerHoverTip
{
    public PowerHoverTip GetPowerHoverTip();
}
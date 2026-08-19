using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;

public class ForesightVar(string name, decimal baseValue, bool skipTooltip = false) : PowerVar<ForesightPower>(name, baseValue)
{
    public const string Key = "ForesightPower";
    public bool SkipTooltip = skipTooltip;

    public ForesightVar(decimal baseValue, bool skipTooltip = false) : this(Key, baseValue, skipTooltip) { }
}

public static class ForesightVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public ForesightVar Foresight => (ForesightVar) dynamicVars[ForesightVar.Key];
    }
}
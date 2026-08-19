using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;

public class ObscureVar(string name, decimal baseValue, bool skipTooltip = false) : DynamicVar(name, baseValue)
{
    public const string Key = "Obscure";
    public bool SkipTooltip = skipTooltip;

    public ObscureVar(decimal baseValue, bool skipTooltip = false) : this(Key, baseValue, skipTooltip) { }
}

public static class ObscureVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public ObscureVar Obscure => (ObscureVar) dynamicVars[ObscureVar.Key];
    }
}
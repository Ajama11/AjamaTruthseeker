using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;

public class ProveVar(string name, decimal baseValue, bool skipTooltip = false) : DynamicVar(name, baseValue)
{
    public const string Key = "Prove";
    public bool SkipTooltip = skipTooltip;
    
    public ProveVar(decimal baseValue, bool skipTooltip = false) : this(Key, baseValue, skipTooltip) { }
}

public static class ProveVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public ProveVar Prove => (ProveVar) dynamicVars[ProveVar.Key];
    }
}
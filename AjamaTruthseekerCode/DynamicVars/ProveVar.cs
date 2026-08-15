using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;

public class ProveVar : DynamicVar
{
    public const string Key = "Prove";
    
    public ProveVar(string name, decimal baseValue, bool skipTooltip = false) : base(name, baseValue)
    {
        if (!skipTooltip) this.WithTooltip();
    }
    
    public ProveVar(decimal baseValue, bool skipTooltip = false) : this(Key, baseValue, skipTooltip) { }
}

public static class ProveVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public ProveVar Prove => (ProveVar) dynamicVars[ProveVar.Key];
    }
}
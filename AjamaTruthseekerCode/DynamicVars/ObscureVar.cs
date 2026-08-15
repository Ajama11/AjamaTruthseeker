using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;

public class ObscureVar : DynamicVar
{
    public const string Key = "Obscure";
    
    public ObscureVar(string name, decimal baseValue, bool skipTooltip = false) : base(name, baseValue)
    {
        if (!skipTooltip) this.WithTooltip();
    }
    
    public ObscureVar(decimal baseValue, bool skipTooltip = false) : this(Key, baseValue, skipTooltip) { }
}

public static class ObscureVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public ObscureVar Obscure => (ObscureVar) dynamicVars[ObscureVar.Key];
    }
}
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;

public class CertifyVar : DynamicVar
{
    public const string Key = "Certify";
    
    public CertifyVar(string name, decimal baseValue, bool skipTooltip = false) : base(name, baseValue)
    {
        if (!skipTooltip) this.WithTooltip();
    }
    
    public CertifyVar(decimal baseValue, bool skipTooltip = false) : this(Key, baseValue, skipTooltip) { }
}

public static class CertifyVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public CertifyVar Certify => (CertifyVar) dynamicVars[CertifyVar.Key];
    }
}
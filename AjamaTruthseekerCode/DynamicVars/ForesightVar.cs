using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;

public class ForesightVar(string name, decimal baseValue) : PowerVar<ForesightPower>(name, baseValue)
{
    public const string Key = "ForesightPower";

    public ForesightVar(decimal baseValue) : this(Key, baseValue) { }
}

public static class ForesightVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public ForesightVar Foresight => (ForesightVar) dynamicVars[ForesightVar.Key];
    }
}
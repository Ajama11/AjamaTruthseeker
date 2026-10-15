using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AjamaTruthseeker.AjamaTruthseekerCode.DynamicVars;

public class NihilityVar(decimal baseValue, bool skipTooltip = false) : DynamicVar(Key, baseValue)
{
    public const string Key = "Nihility";
    public bool SkipTooltip = skipTooltip;
}

public static class NihilityVarExtension
{
    extension(DynamicVarSet dynamicVars)
    {
        public NihilityVar Nihility => (NihilityVar) dynamicVars[NihilityVar.Key];
    }
}
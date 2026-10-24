using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class VulnerableAoeTheory : AbstractPowerTheoryMod<VulnerablePower>
{
    public static VulnerableAoeTheory Create(int amount = 1)
    {
        var mod = ModelDb.CardModifier<VulnerableAoeTheory>().TypedMutableClone();
        return (VulnerableAoeTheory) CreateInternal(mod,
            TargetType.AllEnemies,
            amount,
            PowerType.Debuff);
    }
}
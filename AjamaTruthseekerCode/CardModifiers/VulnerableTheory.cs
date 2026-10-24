using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class VulnerableTheory : AbstractPowerTheoryMod<VulnerablePower>
{
    public static VulnerableTheory Create(int amount = 1)
    {
        var mod = ModelDb.CardModifier<VulnerableTheory>().TypedMutableClone();
        return (VulnerableTheory) CreateInternal(mod,
            TargetType.AnyEnemy,
            amount,
            PowerType.Debuff);
    }
}
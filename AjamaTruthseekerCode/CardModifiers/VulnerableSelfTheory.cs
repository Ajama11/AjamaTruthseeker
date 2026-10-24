using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class VulnerableSelfTheory : AbstractPowerTheoryMod<VulnerablePower>
{
    public static VulnerableSelfTheory Create(int amount = 1)
    {
        var mod = ModelDb.CardModifier<VulnerableSelfTheory>().TypedMutableClone();
        return (VulnerableSelfTheory) CreateInternal(mod,
            TargetType.Self,
            amount,
            PowerType.Debuff);
    }
}
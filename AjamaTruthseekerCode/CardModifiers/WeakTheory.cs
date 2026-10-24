using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class WeakTheory : AbstractPowerTheoryMod<WeakPower>
{
    public static WeakTheory Create(int amount = 1)
    {
        var mod = ModelDb.CardModifier<WeakTheory>().TypedMutableClone();
        return (WeakTheory) CreateInternal(mod,
            TargetType.AnyEnemy,
            amount,
            PowerType.Debuff);
    }
}
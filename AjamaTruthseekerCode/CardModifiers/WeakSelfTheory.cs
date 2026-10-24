using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class WeakSelfTheory : AbstractPowerTheoryMod<WeakPower>
{
    public static WeakSelfTheory Create(int amount = 1)
    {
        var mod = ModelDb.CardModifier<WeakSelfTheory>().TypedMutableClone();
        return (WeakSelfTheory) CreateInternal(mod,
            TargetType.Self,
            amount,
            PowerType.Debuff);
    }
}
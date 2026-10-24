using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AjamaTruthseeker.AjamaTruthseekerCode.CardModifiers;

public class WeakAoeTheory : AbstractPowerTheoryMod<WeakPower>
{
    public static WeakAoeTheory Create(int amount = 1)
    {
        var mod = ModelDb.CardModifier<WeakAoeTheory>().TypedMutableClone();
        return (WeakAoeTheory) CreateInternal(mod,
            TargetType.AllEnemies,
            amount,
            PowerType.Debuff);
    }
}
using MegaCrit.Sts2.Core.Entities.Powers;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Powers;

public class ShapedPowerAbstract : AjamaTruthseekerPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
}
using BaseLib.Abstracts;
using AjamaTruthseeker.AjamaTruthseekerCode.Extensions;
using Godot;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Character;

public class TruthseekerPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Truthseeker.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
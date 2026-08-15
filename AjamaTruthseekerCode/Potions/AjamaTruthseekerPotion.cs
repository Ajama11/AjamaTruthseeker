using BaseLib.Abstracts;
using BaseLib.Utils;
using AjamaTruthseeker.AjamaTruthseekerCode.Character;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Potions;

[Pool(typeof(TruthseekerPotionPool))]
public abstract class AjamaTruthseekerPotion : CustomPotionModel;
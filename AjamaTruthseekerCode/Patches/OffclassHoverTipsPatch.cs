using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using AjamaTruthseeker.AjamaTruthseekerCode.Powers;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
public static class OffclassHoverTipsPatch
{
    [HarmonyPostfix]
    public static IEnumerable<IHoverTip> Postfix(IEnumerable<IHoverTip> __result, CardModel __instance)
    {
        if (__instance is AjamaTruthseekerCard) return __result;

        List<IHoverTip> resultToReturn = __result.ToList();

        if (__instance.Keywords.Contains(MyEnums.Obscured) && !resultToReturn.Contains(HoverTipFactory.FromPower<ForesightPower>()))
        {
            resultToReturn.Insert(
                resultToReturn.IndexOf(HoverTipFactory.FromKeyword(MyEnums.Obscured)) + 1, 
                HoverTipFactory.FromPower<ForesightPower>());
        }

        return resultToReturn;
    }
}
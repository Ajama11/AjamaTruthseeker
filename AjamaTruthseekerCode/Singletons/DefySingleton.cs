using AjamaTruthseeker.AjamaTruthseekerCode.Hooks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Singletons;

public class DefySingleton() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        await MyHooks.OnPotentiallyDefy(card.CombatState!, choiceContext, card);
    }
}
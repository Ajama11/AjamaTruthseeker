using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Hooks;

public interface IOnDefyActivated
{
    public Task OnDefyActivated(CardModel card, PlayerChoiceContext choiceContext);
}
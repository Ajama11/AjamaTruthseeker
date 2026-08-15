using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Hooks;

public interface IOnForeseenActivated
{
    public Task OnForeseenActivated(CardModel card, PlayerChoiceContext choiceContext);
}
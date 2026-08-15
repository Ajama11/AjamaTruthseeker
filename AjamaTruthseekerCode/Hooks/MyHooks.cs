using AjamaTruthseeker.AjamaTruthseekerCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Hooks;

public static class MyHooks
{
    private static async Task Dispatch<T>(
        ICombatState combatState,
        PlayerChoiceContext choiceContext,
        Func<T, Task> action)
        where T : class
    {
        foreach (T listener in combatState.IterateHookListeners().OfType<T>())
        {
            AbstractModel model = (AbstractModel) (object) listener;

            choiceContext.PushModel(model);
            await action(listener);
            choiceContext.PopModel(model);
        }
    }

    /// <summary>
    /// If for some reason this is being called outside AjamaTruthseekerCard OnForeseenWrapper(), remember to call the OnForeseenActivated too if necessary!
    /// </summary>
    public static Task OnPotentiallyForeseen(ICombatState combatState, PlayerChoiceContext choiceContext,
        CardModel card)
    {
        return Dispatch<IOnPotentiallyForeseen>(combatState, choiceContext,
            listener => listener is AjamaTruthseekerCard listenerCard
                ? listenerCard.OnForeseenWrapper(card, choiceContext)
                : card is IOnPotentiallyForeseen
                    ? listener.OnForeseen(choiceContext)
                    : Task.CompletedTask
        );
    }
    
    public static Task OnForeseenActivated(ICombatState combatState, PlayerChoiceContext choiceContext,
        CardModel card)
    {
        return Dispatch<IOnForeseenActivated>(combatState, choiceContext,
            listener => listener.OnForeseenActivated(card, choiceContext)
        );
    }
    
    /// <summary>
    /// If for some reason this is being called outside AjamaTruthseekerCard OnDefyWrapper(), remember to call the OnDefyActivated too if necessary!
    /// </summary>
    public static Task OnPotentiallyDefy(ICombatState combatState, PlayerChoiceContext choiceContext,
        CardModel card)
    {
        return Dispatch<IOnPotentiallyDefy>(combatState, choiceContext,
            listener => listener is AjamaTruthseekerCard listenerCard
                ? listenerCard.OnDefyWrapper(card, choiceContext)
                : card is IOnPotentiallyDefy
                    ? listener.OnDefy(card, choiceContext)
                    : Task.CompletedTask
        );
    }
    
    public static Task OnDefyActivated(ICombatState combatState, PlayerChoiceContext choiceContext,
        CardModel card)
    {
        return Dispatch<IOnDefyActivated>(combatState, choiceContext,
            listener => listener.OnDefyActivated(card, choiceContext)
        );
    }
}
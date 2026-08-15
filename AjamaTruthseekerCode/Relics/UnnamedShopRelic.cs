using AjamaTruthseeker.AjamaTruthseekerCode.Relics;
using AjamaTruthseeker.AjamaTruthseekerCode.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Relics;

public class UnnamedShopRelic() : AjamaTruthseekerRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;

    public override bool HasUponPickupEffect => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs =
            new (SelectionScreenPrompt, 
                0, DynamicVars.Cards.IntValue)
            {
                Cancelable = false,
                RequireManualConfirmation = true
            };

        foreach (CardModel card in await CardSelectCmd.FromDeckGeneric(Owner, prefs, Proven.IsAbleToBeProven))
        {
            Proven.SetProven(card, true);
            CardCmd.Preview(card);
            MySounds.Prove.Play();
        }
    }
}
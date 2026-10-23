using AjamaTruthseeker.AjamaTruthseekerCode.Systems;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Extensions;

public static class CardModelExtensions
{
    extension(CardModel card)
    {
        public RunRngSet Rng => card.Owner.RunState.Rng;

        public bool IsProven()
        {
            return Proven.IsProven(card);
        }

        public bool IsFlagged()
        {
            return Proven.IsFlagged(card);
        }

        public bool IsAbleToBeProven()
        {
            return Proven.IsAbleToBeProven(card);
        }
        
        public bool IsAbleToBeCertified()
        {
            return Proven.IsAbleToBeCertified(card);
        }
    }
}
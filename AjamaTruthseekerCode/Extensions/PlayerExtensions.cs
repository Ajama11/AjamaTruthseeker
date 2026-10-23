using System.Diagnostics.CodeAnalysis;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Extensions;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public static class PlayerExtensions
{
    extension(Player player)
    {
        public IReadOnlyList<CardModel> Deck => PileType.Deck.GetPile(player).Cards;
        public IReadOnlyList<CardModel> Hand => PileType.Hand.GetPile(player).Cards;
        public IReadOnlyList<CardModel> DrawPile => PileType.Draw.GetPile(player).Cards;
        public IReadOnlyList<CardModel> DiscardPile => PileType.Discard.GetPile(player).Cards;
        public IReadOnlyList<CardModel> ExhaustPile => PileType.Exhaust.GetPile(player).Cards;
        public IEnumerable<CardModel> AllCards => player.PlayerCombatState?.AllCards ?? [];

        public List<Creature> Allies => player.Creature.CombatState?
            .GetTeammatesOf(player.Creature)
            .Where(c => c is
            {
                IsAlive: true, IsPlayer: true
            } && c.Player != player)
            .ToList() ?? [];

        public RunRngSet Rng => player.RunState.Rng;
    }
}
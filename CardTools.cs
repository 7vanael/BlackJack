using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarCardGame
{
    public static class CardTools
    {
        public static List<Card> Shuffle(List<Card> pile, Random randomNumberGenerator)
        {
            var shuffledDeck = new List<(Card Card, int RandomValue)>();

            foreach (var card in pile)
                shuffledDeck.Add((card, randomNumberGenerator.Next()));

            return shuffledDeck.OrderBy(tuple => tuple.RandomValue).Select(tuple => tuple.Card).ToList();
        }
        public static void TransferCard(Card card, List<Card> fromPile, List<Card> toPile)
        {
            fromPile.Remove(card);
            toPile.Add(card);
        }
        public static void TransferCardPile(List<Card> fromPile, List<Card> toPile)
        {
            toPile.AddRange(fromPile);
            fromPile.Clear();
        }
    }
}

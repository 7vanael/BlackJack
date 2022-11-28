using BlackJack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    public static class CardTools
    {
        public static void Shuffle(List<Card> pile, Random randomNumberGenerator)
        {
            var shuffledDeck = new List<(Card Card, int RandomValue)>();

            foreach (var card in pile)
            {
                shuffledDeck.Add((card, randomNumberGenerator.Next()));
            }
            pile.Clear();
            pile.AddRange(shuffledDeck.OrderBy(tuple => tuple.RandomValue).Select(tuple => tuple.Card));
                        
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
        // Create the deck
        public static List<Card> BuildADeck()
        {
            var deck = new List<Card>();

            foreach (var suit in Enum.GetValues<Suit>())
            {
                foreach (var rank in Enum.GetValues<Rank>())
                {
                    var card = new Card { Suit = suit, Rank = rank };
                    deck.Add(card);
                }
            }

            return deck;
        }
    }
}

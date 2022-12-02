using System.Runtime.CompilerServices;

namespace BlackJack
{
    internal class BlackJackRules
    {
        internal static bool IsPlayerBusted(Player player)
        {
            return ScoreHand(player) > 21;
        }

        internal static void PlayerDrawACard(Player player, Random rng)
        {
            if(!player.DrawPile.Any())
            {
                CardTools.TransferCardPile(player.DiscardPile, player.DrawPile);
                CardTools.Shuffle(player.DrawPile, rng);
                Console.WriteLine("The deck was shuffled.");
            }
            player.DrawACard();
        }

        internal static bool CanPlayerBet(Player player, float minimumBet)
        {
            return (player.Money >= minimumBet) ;
        }

        internal static void PlaceABet(Player player, float minimumBet)
        {
            player.Money = player.Money - minimumBet;
            player.Pool = player.Pool + minimumBet;
        }
        internal static void WinABet(Player player)
        {
            player.Money = player.Money + player.Pool * 2f;
            player.Pool = 0f;
        }
        
        internal static void WinABlackJack(Player player)
        {
            player.Money = player.Money + (player.Pool * 3f) / 2f;
            player.Pool = 0f;
        }
        internal static void Push(Player player)
        {
            player.Money = player.Money + player.Pool;
            player.Pool = 0f;
        }
        internal static int ScoreHand (Player player)
        {
            int totalScore = 0;

            foreach(Card card in player.Hand)
            {
                totalScore += card.Rank switch
                {
                    >= Rank.Ace and <= Rank.Nine => (int)card.Rank,
                    _  => 10
                };
            }

            foreach (Card card in player.Hand.Where(c => c.Rank == Rank.Ace))
            {
                if (totalScore <= 11)
                    totalScore += 10;
            }

            return totalScore;
        }
    }
}
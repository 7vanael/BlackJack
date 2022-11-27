namespace BlackJack
{
    internal class BlackJackRules
    {
        internal static bool IsPlayerBusted(Player player)
        {
            return ScoreHand(player) > 21;
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
namespace BlackJack
{
    public class Player
    {
        public List<Card> DrawPile { get; set; }
        public List<Card> DiscardPile { get; set; }
        public List<Card> Hand { get; set; }
        public required String Name { get; init; }
        public Dictionary<string, int> Stats { get; }
        public float Money { get; set; }
        public float Pool { get; set; }
        public Player()
        {
            DrawPile = new List<Card>();
            DiscardPile = new List<Card>();
            Hand = new List<Card>();
            Stats = new Dictionary<string, int>();
            Money = new float();
            Pool = new float();
        }

        public void DrawACard()
        {
            CardTools.TransferCard(card: DrawPile.First(), fromPile: DrawPile, toPile: Hand);
        }

        public void DiscardACard(Card card)
        {
            CardTools.TransferCard(card: card, fromPile: Hand, toPile: DiscardPile);
        }

        public void DiscardHand()
        {
            CardTools.TransferCardPile(fromPile: Hand, toPile: DiscardPile);
        }
    }

}

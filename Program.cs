namespace BlackJack
{
    internal class Program
    {
        const string ROUND_WINS = "Round wins";
        const string MONEY = "Money";
        static void Main(string[] args)
        {
            Console.WriteLine("Enter seed or enter to auto-generate one:");
            var seedText = Console.ReadLine();

            var seed = 0;
            if (!int.TryParse(seedText, out seed))
            {
                seed = new Random().Next(0, 10000);
            }

            Console.WriteLine($"seed = {seed}");

            var rng = new Random(seed);

            var allPlayers = new List<Player>();

            Console.WriteLine("Enter the names of players and just Enter to finish:");

            var done = false;
            while (!done)
            {
                string name = Console.ReadLine() ?? "";

                if (name != "")
                    allPlayers.Add(new Player() { Name = name });
                else
                    done = true;
            }

            if (!allPlayers.Any())
            {
                Console.WriteLine("No players! The House loses!");
                return;
            }

            foreach (var player in allPlayers)
            {
                player.Stats[ROUND_WINS] = 0;
                player.Stats[MONEY] = 0;
            }

            var activePlayers = new List<Player>(allPlayers);

            List<Card> deck = CardTools.BuildADeck();

            List<Card> drawPile = CardTools.Shuffle(deck, rng);
        }
    }
}
namespace BlackJack
{
    internal class Program
    {
        const string ROUND_WINS = "Round wins";
        const string MONEY = "Money";
        const string HAND_TOTAL = "Hand Total";
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
            allPlayers.Add(new Player() {Name = "Dealer"});

            foreach (var player in allPlayers)
            {
                player.Stats[ROUND_WINS] = 0;
                player.Stats[MONEY] = 0;
                player.Stats[HAND_TOTAL] = 0;
            }

            var activePlayers = new List<Player>(allPlayers);
            
            List<Card> deck = CardTools.BuildADeck();

            List<Card> drawPile = CardTools.Shuffle(deck, rng);
            List<Card> discardPile = new List<Card>();

            foreach (var player in allPlayers) 
            {
                player.DrawPile = drawPile;
                player.DiscardPile = discardPile;
            }

            var isGameOver = false;
            while (!isGameOver) 
            {
                for (int i = 0; i < 2; i++)
                {
                    foreach (Player player in allPlayers)
                    {
                        player.DrawACard();
                        Console.WriteLine($"{player.Name} drew {player.Hand.Last()}");
                    }
                }
                //Prompt each player to indicate if they want to STAY or HIT
                foreach(Player player in allPlayers)
                {
                    bool isLoopComplete = false;
                    while(!isLoopComplete)
                    { 
                        Console.WriteLine($"{player.Name}, your hand is worth {BlackJackRules.ScoreHand(player)}");
                        Console.WriteLine($"{player.Name}, do you (S)TAY or (H)IT?");
                        var answer = Console.ReadLine() ?? "";
                        switch (answer.ToUpper())
                        {
                            case "S":
                            case "STAY":
                                isLoopComplete = true;
                                break;
                            case "H":
                            case "HIT":
                                player.DrawACard();
                                Console.WriteLine($"{player.Name} drew {player.Hand.Last()}. " +
                                    $"Total score is {BlackJackRules.ScoreHand(player)}");
                                if (BlackJackRules.IsPlayerBusted(player))
                                {
                                    Console.WriteLine($"{player.Name} BUSTED!");
                                }
                                isLoopComplete = true;
                                break;
                            default:
                                Console.WriteLine("Please enter S to Stay, or H to Hit");
                                //return to top of foreach loop, but not for the nextg player
                                break;
                        }
                    }
                }   

            }
        }
    }
}
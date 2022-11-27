using System.Numerics;

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
            var dealer = new Player() { Name = "Dealer" };

            allPlayers.Add(dealer);

            foreach (var player in allPlayers)
            {
                player.Stats[ROUND_WINS] = 0;
                player.Stats[MONEY] = 0;
                player.Stats[HAND_TOTAL] = 0;
            }

            List<Card> deck = CardTools.BuildADeck();

            List<Card> drawPile = CardTools.Shuffle(deck, rng);
            List<Card> discardPile = new List<Card>();

            foreach (var player in allPlayers) 
            {
                player.DrawPile = drawPile;
                player.DiscardPile = discardPile;
            }

            var roundCounter = 0;

            var isGameOver = false;
            while (!isGameOver) 
            {
                Console.WriteLine($"------ ROUND {++roundCounter} ------");

                var activePlayers = new List<Player>(allPlayers);

                //TODO Check if players can afford to bet minimum bets
                //TODO Remove players who can't continue playing
                //TODO Declare winner? Or best loser.
                //TODO Initialize bets

                // Handle naturals

                //Deals initial two cards to each player
                for (int i = 0; i < 2; i++)
                {
                    foreach (Player player in allPlayers)
                    {
                        player.DrawACard();
                        if(player != dealer || i == 0)
                            Console.WriteLine($"{player.Name} drew {player.Hand.Last()}");
                        else
                            Console.WriteLine($"{player.Name} drew face down card.");
                    }
                }

                //Starts the round of play
                foreach (Player player in allPlayers.Where(p => p != dealer))
                {                   
                    bool isTurnComplete = false;
                    while(!isTurnComplete)
                    {
                        if (BlackJackRules.ScoreHand(player) == 21)
                        {
                            Console.WriteLine($"{player.Name} has BLACKJACK!");
                            isTurnComplete = true;
                            continue;
                        }

                        Console.WriteLine($"{player.Name}, your hand is worth {BlackJackRules.ScoreHand(player)}");
                        Console.WriteLine($"{player.Name}, do you (S)TAY or (H)IT?");
                        var answer = Console.ReadLine() ?? "";
                        switch (answer.ToUpper())
                        {
                            case "S":
                            case "STAY":
                                isTurnComplete = true;
                                break;
                            case "H":
                            case "HIT":
                                player.DrawACard();
                                Console.WriteLine($"{player.Name} drew {player.Hand.Last()}.");
                                if (BlackJackRules.IsPlayerBusted(player))
                                {
                                    Console.WriteLine($"Total score is {BlackJackRules.ScoreHand(player)}");
                                    Console.WriteLine($"{player.Name} BUSTED!");
                                    activePlayers.Remove(player);
                                    isTurnComplete = true;
                                }
                                break;
                            default:
                                Console.WriteLine("Please enter S to Stay, or H to Hit");                               
                                break;
                        }
                    }
                }

                // Automate dealer play
                Console.WriteLine($"The dealer's second card is {dealer.Hand.Last()}," +
                    $" dealer's score is {BlackJackRules.ScoreHand(dealer)}");
                if (BlackJackRules.ScoreHand(dealer) == 21)
                    Console.WriteLine("Dealer has BLACKJACK!");

                while(BlackJackRules.ScoreHand(dealer) <= 16)
                {
                    dealer.DrawACard();
                    Console.WriteLine($"{dealer.Name} drew {dealer.Hand.Last()}.");
                    Console.WriteLine($"Total score is {BlackJackRules.ScoreHand(dealer)}");
                    if (BlackJackRules.IsPlayerBusted(dealer))
                    {
                        Console.WriteLine($"Dealer BUSTED!");                        
                    }
                }

                // Round scoring
                foreach (var player in activePlayers.Where(p => p != dealer))
                {
                    if (BlackJackRules.IsPlayerBusted(dealer))
                    {
                        Console.WriteLine($"Dealer busted! {player.Name} is a winner!");
                    }
                    else if (BlackJackRules.ScoreHand(player) >= BlackJackRules.ScoreHand(dealer))
                    {
                        Console.WriteLine($"{player.Name} is a winner!");
                    }
                    else
                    {
                        Console.WriteLine($"{player.Name} loses!");
                    }
                }

                // Discard cards
                foreach (var player in allPlayers)
                {
                    player.DiscardHand();
                }
            }
        }
    }
}
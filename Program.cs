using System.ComponentModel;
using System.Numerics;

namespace BlackJack
{
    internal class Program
    {
        const string ROUND_WINS = "Round wins";
        const string BLACKJACK_WINS = "Blackjack wins";
        const string BUST_COUNT = "Bust Count";
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

            //Initializes starting cash/table buy in. Must be >1. 
            //Starting cash will be the same for every player.
            
            float startingCash = 50f;
            float minimumBet = 1f;
            Console.WriteLine("Enter the cash buy in for this table");
            var done = false;
            while (!done)
            {
                string number = Console.ReadLine() ?? "";
                if (float.TryParse(number, out var userEnteredCash) && userEnteredCash >= 1)
                {
                    startingCash = userEnteredCash;
                    Console.WriteLine($"Got it, the buy in for this game is {startingCash}.");
                    done = true;
                }
                else
                {
                    Console.WriteLine($"The house disagrees. The buy in for this game is {startingCash}.");
                    done = true;
                }
            }

            //Set minimum bet
            Console.WriteLine("Enter the standard bet for this table, or enter to accept $1");
            done = false;
            while (!done)
            {
                string number = Console.ReadLine() ?? "";

                if (float.TryParse(number, out float userEnteredBet) && (userEnteredBet > 0) && (userEnteredBet <= startingCash))
                {
                    minimumBet = userEnteredBet;
                    Console.WriteLine($"Got it, the bet for this game is {minimumBet}.");
                    done = true;
                }
                else
                {
                    Console.WriteLine($"The house disagrees. The standard bet for this game is {minimumBet}.");
                    done = true;
                }
            }

            Console.WriteLine("Enter the names of players and just Enter to finish:");

            done = false;
            while (!done)
            {
                string name = Console.ReadLine() ?? "";

                if (name != "")
                    allPlayers.Add(new Player() { Name = name, Money = startingCash});
                else
                    done = true;
            }

            if (!allPlayers.Any())
            {
                Console.WriteLine("No players! The House loses!");
                return;
            }
            var dealer = new Player() { Name = "Dealer", Money = 50000000 };

            allPlayers.Add(dealer);

            foreach (var player in allPlayers)
            {
                player.Stats[ROUND_WINS] = 0;
                player.Stats[BLACKJACK_WINS] = 0;
                player.Stats[BUST_COUNT] = 0;                              
            }

            List<Card> drawPile = CardTools.BuildADeck();
            CardTools.Shuffle(drawPile, rng);
            List<Card> discardPile = new List<Card>();
            List<Player> playersWithBlackJack = new List<Player>();

            foreach (var player in allPlayers) 
            {
                player.DrawPile = drawPile;
                player.DiscardPile = discardPile;
            }

            var roundCounter = 0;
            var isGameOver = false;

            var activePlayers = new List<Player>(allPlayers);

            while (!isGameOver) 
            {
                Console.WriteLine($"------ ROUND {++roundCounter} ------");
                playersWithBlackJack.Clear();
                
                //Check if players can afford to bet minimum bets
                //Remove players who can't continue playing
                foreach (Player player in new List<Player>(activePlayers))
                {
                    if(BlackJackRules.CanPlayerBet(player, minimumBet))
                    {
                        BlackJackRules.PlaceABet(player, minimumBet);
                        Console.WriteLine($"{player.Name} bets ${minimumBet}");
                    }
                    else
                    {
                        Console.WriteLine($"{player.Name} is unable to bet and has been removed from the table!");
                        activePlayers.Remove(player);
                    }
                }
              
                //Deals initial two cards to each player
                for (int i = 0; i < 2; i++)
                {
                    foreach (Player player in activePlayers)
                    {
                        BlackJackRules.PlayerDrawACard(player, rng);
                        if(player != dealer || i == 0)
                            Console.WriteLine($"{player.Name} drew {player.Hand.Last()}");
                        else
                            Console.WriteLine($"{player.Name} drew face down card.");
                    }
                }

                bool doesDealerHaveBlackJack = false;

                if (BlackJackRules.ScoreHand(dealer) == 21)
                {
                    Console.WriteLine($"Dealer's second card is {dealer.Hand.Last()}");
                    Console.WriteLine("Dealer has BLACKJACK!");
                    doesDealerHaveBlackJack = true;
                }
                foreach (Player player in activePlayers.Where(p => p != dealer))
                {
                    Console.WriteLine();                    
                    if (BlackJackRules.ScoreHand(player) == 21)
                    {
                        Console.WriteLine($"{player.Name} has BLACKJACK!");
                        player.Stats[BLACKJACK_WINS]++;
                        playersWithBlackJack.Add(player);                        
                    }
                }
                if (!doesDealerHaveBlackJack)
                {
                    //Starts the round of play
                    foreach (Player player in activePlayers.Where(p => p != dealer))
                    {
                        bool isTurnComplete = false;
                        if (BlackJackRules.ScoreHand(player) == 21)
                        {
                            isTurnComplete = true;
                        }

                        while (!isTurnComplete)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Dealer showing {dealer.Hand.First()}");
                            Console.WriteLine($"{player.Name} has {string.Join(", ", player.Hand.Select(c => c.Rank))}");
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
                                    BlackJackRules.PlayerDrawACard(player, rng);
                                    Console.WriteLine($"{player.Name} drew {player.Hand.Last()}.");
                                    if (BlackJackRules.IsPlayerBusted(player))
                                    {
                                        Console.WriteLine($"Total score is {BlackJackRules.ScoreHand(player)}");
                                        Console.WriteLine($"{player.Name} BUSTED!");
                                        player.Stats[BUST_COUNT]++;
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

                    while (BlackJackRules.ScoreHand(dealer) <= 16)
                    {
                        BlackJackRules.PlayerDrawACard(dealer, rng);
                        Console.WriteLine($"{dealer.Name} drew {dealer.Hand.Last()}.");
                        Console.WriteLine($"Total score is {BlackJackRules.ScoreHand(dealer)}");
                        if (BlackJackRules.IsPlayerBusted(dealer))
                        {
                            Console.WriteLine($"Dealer BUSTED!");
                        }
                    }
                }
                // Round scoring
                foreach (var player in activePlayers.Where(p => p != dealer))
                {
                    if(playersWithBlackJack.Contains(player) && !doesDealerHaveBlackJack)
                    {
                        BlackJackRules.WinABlackJack(player);
                        Console.WriteLine($"{player.Name} has won with a BlackJack! They now have {player.Money:0.00}");
                        player.Stats[BLACKJACK_WINS]++;
                        player.Stats[ROUND_WINS]++;
                    }
                    else if (!BlackJackRules.IsPlayerBusted(player) 
                        && (BlackJackRules.IsPlayerBusted(dealer) || (BlackJackRules.ScoreHand(player) > BlackJackRules.ScoreHand(dealer))))
                    {
                        BlackJackRules.WinABet(player);
                        Console.WriteLine($"{player.Name} is a winner! They now have {player.Money:0.00}");
                        player.Stats[ROUND_WINS]++;                        
                    } 
                    else if (!BlackJackRules.IsPlayerBusted(player) && (BlackJackRules.ScoreHand(player) == BlackJackRules.ScoreHand(dealer)))
                    {
                        BlackJackRules.Push(player);
                        Console.WriteLine($"{player.Name} pushed. Bet was returned. They now have {player.Money:0.00}.");
                    }
                    else
                    {
                        Console.WriteLine($"{player.Name} loses!");
                        Console.WriteLine($"{player.Name} now has ${player.Money:0.00}.");
                    }
                }

                // Discard cards & clears the pool.
                foreach (var player in allPlayers)
                {
                    player.DiscardHand();
                    player.Pool = 0;
                }

                // Do you want to play another round?  If not, game summary
                Console.WriteLine("Play another round? (Y)es or (N)o");
                var response = Console.ReadLine() ?? "";
                isGameOver = (response.ToUpper() == "N");
            }

            Console.WriteLine("-------- GAME SUMMARY --------");
            foreach(var player in allPlayers.Where(p => p != dealer))
            {
                Console.WriteLine($"{player.Name} won {player.Stats[ROUND_WINS]} rounds, busted {player.Stats[BUST_COUNT]}, and had {player.Stats[BLACKJACK_WINS]} blackjacks");
                Console.WriteLine($"{player.Name} ended with ${player.Money:0.00}.");
            }
        }
    }
}
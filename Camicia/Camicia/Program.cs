// the game state where the game finishes and doesn't look is correctly implemented
// Need to implement rounds (aka tricks) and then the game state where its a continuous loop

public static class Camicia
{
    public enum GameStatus
    {
        Finished, // Game ends because one person has all the cards
        Loop // Game is in an infinite loop because the non-number cards are identical to what they were earlier during the game
    }

    public record GameResult(GameStatus Status, int Tricks, int Cards);

    public static void Main(string[] args)
    {
        SimulateGame(["2", "A", "7", "8", "Q", "10"], ["3", "4", "5", "6", "K", "9", "J"]);
        Console.ReadLine();
    }

    public static GameResult SimulateGame(string[] playerA, string[] playerB)
    {
        bool playerATurn = true;
        bool playerBTurn = false;
        bool gameActive = true;
        int round = 1; // Round will increase every time there is a trick

        
        // 2 types of cards: Number card and payment card
        var paymentCardMapping = new Dictionary<string, int> {
            { "J", 1 },
            { "Q", 2 },
            { "K", 3 },
            { "A", 4 }
        };

        // Queue is FIFO - first in first out. Think of it as a line of people.
        Queue<string> playerAPile = new Queue<string>(playerA);
        Queue<string> playerBPile = new Queue<string>(playerB);
        Queue<string> centralPile = new Queue<string>();

        int playerAPenalty = 0;
        int playerBPenalty = 0;

        string lastPlayedCardByPlayerA = "";
        string lastPlayedCardByPlayerB = "";

        bool isPlayerAPayingPenalty = false;
        bool isPlayerBPayingPenalty = false;


        Console.WriteLine("Round |     Player A     |      Player B      |      Pile       |     Penalty Due      ");

        while (gameActive)
        {
            if (playerAPile.Count == 0 && isPlayerAPayingPenalty)
            {
                gameActive = false;
                while (centralPile.Count > 0)
                {
                    string lastCardRemovedFromCentralPile = centralPile.Dequeue();
                    playerBPile.Enqueue(lastCardRemovedFromCentralPile);
                }
                displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                break;
            }
            else if (playerBPile.Count == 0 && isPlayerBPayingPenalty)
            {
                gameActive = false;
                while (centralPile.Count > 0)
                {
                    string lastCardRemovedFromCentralPile = centralPile.Dequeue();
                    playerAPile.Enqueue(lastCardRemovedFromCentralPile);
                }
                displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                break;
            }

            displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);

            if (playerATurn)
            {
                if (playerAPile.Peek() == "J" || playerAPile.Peek() == "Q" ||
                    playerAPile.Peek() == "K" || playerAPile.Peek() == "A")
                {
                    playerBPenalty = paymentCardMapping[playerAPile.Peek()];
                    isPlayerBPayingPenalty = true;
                }

                if (isPlayerAPayingPenalty)
                {
                    while (playerAPenalty > 0)
                    {
                        if (playerAPile.Peek() == "J" || playerAPile.Peek() == "Q" ||
                            playerAPile.Peek() == "K" || playerAPile.Peek() == "A")
                        {
                            playerBPenalty = paymentCardMapping[playerAPile.Peek()];
                            isPlayerBPayingPenalty = true;
                            isPlayerAPayingPenalty = false;
                            lastPlayedCardByPlayerA = playerAPile.Dequeue(); // Remove from player A's deck
                            centralPile.Enqueue(lastPlayedCardByPlayerA); // Put the removed card in the central pile
                            playerATurn = false;
                            playerBTurn = true;
                            break;
                        }
                        playerATurn = true; // maybe not necessary
                        playerBTurn = false; // maybe not necessary
                        playerAPenalty -= 1;
                        //if (playerAPenalty <= 0)
                        //{
                        //    isPlayerAPayingPenalty = false;
                        //}
                        lastPlayedCardByPlayerA = playerAPile.Dequeue(); // Remove from player A's deck
                        centralPile.Enqueue(lastPlayedCardByPlayerA); // Put the removed card in the central pile
                        displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                    }
                }
                else if (!isPlayerAPayingPenalty && playerATurn)
                {
                    lastPlayedCardByPlayerA = playerAPile.Dequeue(); // Remove from player A's deck
                    centralPile.Enqueue(lastPlayedCardByPlayerA); // Put the removed card in the central pile
                    playerATurn = false;
                    playerBTurn = true;
                }
            }

            // REVERSE ABOVE LOGIC
            else if (playerBTurn)
            {
                if (playerBPile.Peek() == "J" || playerBPile.Peek() == "Q" ||
                    playerBPile.Peek() == "K" || playerBPile.Peek() == "A")
                {
                    playerAPenalty = paymentCardMapping[playerBPile.Peek()];
                    isPlayerAPayingPenalty = true;
                }

                if (isPlayerBPayingPenalty)
                {
                    while (playerBPenalty > 0)
                    {
                        
                        if (playerBPile.Peek() == "J" || playerBPile.Peek() == "Q" ||
                            playerBPile.Peek() == "K" || playerBPile.Peek() == "A")
                        {
                            playerAPenalty = paymentCardMapping[playerBPile.Peek()];
                            isPlayerAPayingPenalty = true;
                            isPlayerBPayingPenalty = false;
                            lastPlayedCardByPlayerB = playerBPile.Dequeue(); // Remove from player B's deck
                            centralPile.Enqueue(lastPlayedCardByPlayerB); // Put the removed card in the central pile
                            playerATurn = true;
                            playerBTurn = false;
                            break;
                        }
                        playerBTurn = true; // maybe not necessary
                        playerATurn = false; // maybe not necessary
                        playerBPenalty -= 1;
                        //if (playerBPenalty <= 0)
                        //{
                        //    isPlayerBPayingPenalty = false;
                        //}
                        lastPlayedCardByPlayerB = playerBPile.Dequeue(); // Remove from player B's deck
                        centralPile.Enqueue(lastPlayedCardByPlayerB); // Put the removed card in the central pile
                        displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                    
                    }
                }
                else if (!isPlayerBPayingPenalty && playerBTurn)
                {
                    lastPlayedCardByPlayerB = playerBPile.Dequeue(); // Remove from player B's deck
                    centralPile.Enqueue(lastPlayedCardByPlayerB); // Put the removed card in the central pile
                    playerBTurn = false;
                    playerATurn = true;
                }
            }
        }


        // If the player paying penalty reveals another payment card
        // Then that player stops paying penalty, and the other player must pay the penalty
        // Of the new payment card

        // If a penalty is paid in full without interruption, then the player who drew the payment 
        // card will collect the central pile (i.e. a 'trick') and place it at the bottom of their deck. This player
        // then starts the next round

        // If a player runs out of cards, the other player collects the central pile

        // If one player has all the cards in their hand after a trick, they win

        //The game enters a loop as soon as the decks are identical to what they were earlier 
        // during the game, not counting number cards

        return new GameResult(GameStatus.Finished, 0, 0);
    }

    public static void displayRow(int round, Queue<string> playerAPile, Queue<string> playerBPile, Queue<string> centralPile, int playerAPenalty, int playerBPenalty, bool isPlayerAPayingPenalty, bool isPlayerBPayingPenalty)
    {
        string penaltyToDisplay;
        string playerPayingPenalty;
        if (playerAPenalty > 0 && isPlayerAPayingPenalty)
        {
            penaltyToDisplay = playerAPenalty.ToString();
            playerPayingPenalty = "Player A: ";
        }
        else if (playerBPenalty > 0 && isPlayerBPayingPenalty)
        {
            penaltyToDisplay = playerBPenalty.ToString();
            playerPayingPenalty = "Player B: ";
        }
        else
        {
            penaltyToDisplay = "-";
            playerPayingPenalty = "";
        }

        Console.WriteLine("---------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine($"{round,3}   | {string.Join(" ", playerAPile),16} | {string.Join(" ", playerBPile),18} | {string.Join(" ", centralPile),10}       | {playerPayingPenalty}{penaltyToDisplay}");
        Console.WriteLine();
        Console.WriteLine("Press ENTER to continue");
        Console.ReadLine();
    }
}

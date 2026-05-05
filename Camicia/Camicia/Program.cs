using System;

// Payment cards are not working. It is never showing the correct value for playerAPenalty or playerBPenalty; it just shows 0.
// As a way to troubleshoot, you can simulate a game where the first card is a payment card, to get that functionality working.

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
            displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty);

            if (playerAPile.Count == 0)
            {
                gameActive = false;
            }
            else if (playerBPile.Count == 0)
            {
                gameActive = false;
            }

            if (playerATurn)
            {
                if (playerAPile.Peek() == "J" || playerAPile.Peek() == "Q" ||
                    playerAPile.Peek() == "K" || playerAPile.Peek() == "A")
                {
                    playerBPenalty = paymentCardMapping[playerAPile.Peek()];
                    isPlayerBPayingPenalty = true;
                }

                lastPlayedCardByPlayerA = playerAPile.Dequeue(); // Remove from player A's deck
                centralPile.Enqueue(lastPlayedCardByPlayerA); // Put the removed card in the central pile

                if (isPlayerAPayingPenalty)
                {
                    while (playerAPenalty > 0)
                    {
                        playerAPenalty -= 1;
                        if (playerAPenalty <= 0)
                        {
                            isPlayerAPayingPenalty = false;
                        }
                    }
                }

                playerATurn = false;
                playerBTurn = true;

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

                lastPlayedCardByPlayerB = playerBPile.Dequeue(); // Remove from player B's deck
                centralPile.Enqueue(lastPlayedCardByPlayerB); // Put the removed card in the central pile

                if (isPlayerBPayingPenalty)
                {
                    while (playerBPenalty > 0)
                    {
                        playerBPenalty -= 1;
                        if (playerBPenalty <= 0)
                        {
                            isPlayerBPayingPenalty = false;
                        }
                    }
                }

                playerBTurn = false;
                playerATurn = true;

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

    public static void displayRow(int round, Queue<string> playerAPile, Queue<string> playerBPile, Queue<string> centralPile, int playerAPenalty, int playerBPenalty)
    {
        string penaltyToDisplay = "-";
        if (playerAPenalty > 0)
        {
            penaltyToDisplay = playerAPenalty.ToString();
        }
        else if (playerBPenalty > 0)
        {
            penaltyToDisplay = playerBPenalty.ToString();
        }

        Console.WriteLine("---------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine($"{round,3}   | {string.Join(" ", playerAPile),16} | {string.Join(" ", playerBPile),18} | {string.Join(" ", centralPile),10}       | {playerAPenalty,10}");
        Console.WriteLine();
        Console.WriteLine("Press ENTER to continue");
        Console.ReadLine();
    }
}

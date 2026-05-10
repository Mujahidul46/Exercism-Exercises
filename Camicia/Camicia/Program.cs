// fix cards and tricks count
// fix bug in game 1

public static class Camicia
{
    public enum GameStatus
    {
        Finished, // Game ends because one person has all the cards
        Loop // Game is in an infinite loop because the non-number cards are identical to what they were earlier during the game
    }

    public record GameResult(GameStatus Status, int round, int Cards);

    public static void Main(string[] args)
    {
        //SimulateGame(["2", "A", "7", "8", "Q", "10"], ["3", "4", "5", "6", "K", "9", "J"]); // this game finished
        SimulateGame(["J", "2", "3"], ["4", "J", "5"]); // this game loops
        Console.ReadLine();
    }

    public static GameResult SimulateGame(string[] playerA, string[] playerB)
    {
        bool playerATurn = true;
        bool playerBTurn = false;
        bool gameActive = true;
        int round = 1; // round will increase every time there is a trick 

        
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

        int numOfCards = 0;

        int playerAPenalty = 0;
        int playerBPenalty = 0;

        string lastPlayedCardByPlayerA = "";
        string lastPlayedCardByPlayerB = "";

        bool isPlayerAPayingPenalty = false;
        bool isPlayerBPayingPenalty = false;

        bool playerAShouldTakeCentralPile = false;
        bool playerBShouldTakeCentralPile = false;


        GameStatus status = GameStatus.Finished;

        HashSet<string> seenGameStates = new HashSet<string>();

        Console.WriteLine("round |     Player A     |      Player B      |      Pile       |     Penalty Due      ");

        displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
        if (isLoopFound(playerAPile, playerBPile, seenGameStates))
        {
            gameActive = false;
        }

        while (gameActive)
        {
            if (playerAPile.Count == 0 && isPlayerAPayingPenalty) // code inside here can be its own method
            {
                gameActive = false;
                while (centralPile.Count > 0)
                {
                    string lastCardRemovedFromCentralPile = centralPile.Dequeue();
                    playerBPile.Enqueue(lastCardRemovedFromCentralPile);
                }
                round += 1;
                status = GameStatus.Finished;
                gameActive = false;
                displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                {
                    status = GameStatus.Loop;
                    gameActive = false;
                }
                break;
            }
            else if (playerBPile.Count == 0 && isPlayerBPayingPenalty) // code inside here can be its own method
            {
                gameActive = false;
                while (centralPile.Count > 0)
                {
                    string lastCardRemovedFromCentralPile = centralPile.Dequeue();
                    playerAPile.Enqueue(lastCardRemovedFromCentralPile);
                }
                round += 1;
                status = GameStatus.Finished;
                gameActive = false;
                displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                {
                    status = GameStatus.Loop;
                    gameActive = false;
                }
                break;
            }

            if (playerATurn)
            {
                if (playerAShouldTakeCentralPile)
                {
                    while (centralPile.Count > 0)
                    {
                        string lastCardFromCentralPile = centralPile.Dequeue();
                        playerAPile.Enqueue(lastCardFromCentralPile);
                    }
                    playerAShouldTakeCentralPile = false;
                    round++;
                    displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                    if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                    {
                        status = GameStatus.Loop;
                        gameActive = false;
                        break;
                    }
                }
                else if (playerAPile.Peek() == "J" || playerAPile.Peek() == "Q" ||
                    playerAPile.Peek() == "K" || playerAPile.Peek() == "A")
                {
                    playerBPenalty = paymentCardMapping[playerAPile.Peek()];
                    isPlayerBPayingPenalty = true;
                }
                // if one of the players is paying a penalty, and pay without interruption (i.e. they do not draw J/Q/K/A), then that's a trick, and the other player gets central pile.
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
                            numOfCards += 1;
                            centralPile.Enqueue(lastPlayedCardByPlayerA); // Put the removed card in the central pile
                            displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                            if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                            {
                                status = GameStatus.Loop;
                                gameActive = false;
                                break;
                            }
                            playerATurn = false;
                            playerBTurn = true;
                            break;
                        }
                        else if (playerAPenalty == 1) // this means the player is going to be able to pay their penalty without being interrupted (next card is number card). Therefore, the other player will get the center pile.
                        {
                            lastPlayedCardByPlayerA = playerAPile.Dequeue(); // Remove from player A's deck
                            numOfCards += 1;
                            centralPile.Enqueue(lastPlayedCardByPlayerA); // Put the removed card in the central pile
                            playerAPenalty--;
                            isPlayerAPayingPenalty = false;
                            displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                            if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                            {
                                status = GameStatus.Loop;
                                gameActive = false;
                                break;
                            }
                            playerATurn = false;
                            playerBTurn = true;
                            playerBShouldTakeCentralPile = true;
                        }
                        else // Player has a number card
                        {
                            playerAPenalty--;
                            lastPlayedCardByPlayerA = playerAPile.Dequeue(); // Remove from player A's deck
                            numOfCards += 1;
                            centralPile.Enqueue(lastPlayedCardByPlayerA); // Put the removed card in the central pile
                            displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                            if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                            {
                                status = GameStatus.Loop;
                                gameActive = false;
                                break;
                            }
                        }
                    }
                }
                else if (!isPlayerAPayingPenalty && playerATurn)
                {
                    lastPlayedCardByPlayerA = playerAPile.Dequeue(); // Remove from player A's deck
                    numOfCards += 1;
                    centralPile.Enqueue(lastPlayedCardByPlayerA); // Put the removed card in the central pile
                    displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                    if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                    {
                        status = GameStatus.Loop;
                        gameActive = false;
                        break;
                    }
                    playerATurn = false;
                    playerBTurn = true;
                }
            }
            // PLAYER B
            else if (playerBTurn)
            {
                if (playerBShouldTakeCentralPile)
                {
                    while (centralPile.Count > 0)
                    {
                        string lastCardFromCentralPile = centralPile.Dequeue();
                        playerBPile.Enqueue(lastCardFromCentralPile);
                    }
                    round++;
                    playerBShouldTakeCentralPile = false;
                    displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                    if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                    {
                        status = GameStatus.Loop;
                        gameActive = false;
                        break;
                    }
                }
                else if (playerBPile.Peek() == "J" || playerBPile.Peek() == "Q" ||
                    playerBPile.Peek() == "K" || playerBPile.Peek() == "A")
                {
                    playerAPenalty = paymentCardMapping[playerBPile.Peek()];
                    isPlayerAPayingPenalty = true;
                }
                // if one of the players is paying a penalty, and pay without interruption (i.e. they do not draw J/Q/K/A), then that's a trick, and the other player gets central pile.
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
                            numOfCards += 1;
                            centralPile.Enqueue(lastPlayedCardByPlayerB); // Put the removed card in the central pile
                            displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                            if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                            {
                                status = GameStatus.Loop;
                                gameActive = false;
                                break;
                            }
                            playerBTurn = false;
                            playerATurn = true;
                            break;
                        }
                        else if (playerBPenalty == 1) // this means the player is going to be able to pay their penalty without being interrupted (next card is number card)
                        {
                            lastPlayedCardByPlayerB = playerBPile.Dequeue(); // Remove from player B's deck
                            numOfCards += 1;
                            centralPile.Enqueue(lastPlayedCardByPlayerB); // Put the removed card in the central pile
                            playerBPenalty--;
                            isPlayerBPayingPenalty = false;
                            displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                            if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                            {
                                status = GameStatus.Loop;
                                gameActive = false;
                                break;
                            }
                            playerBTurn = false;
                            playerATurn = true;
                            playerAShouldTakeCentralPile = true;
                        }
                        else
                        {
                            playerBPenalty--;
                            //if (playerBPenalty <= 0)
                            //{
                            //    isPlayerBPayingPenalty = false;
                            //}
                            lastPlayedCardByPlayerB = playerBPile.Dequeue(); // Remove from player B's deck
                            numOfCards += 1;
                            centralPile.Enqueue(lastPlayedCardByPlayerB); // Put the removed card in the central pile
                            displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                            if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                            {
                                status = GameStatus.Loop;
                                gameActive = false;
                                break;
                            }
                        }
                    }
                }
                else if (!isPlayerBPayingPenalty && playerBTurn)
                {
                    lastPlayedCardByPlayerB = playerBPile.Dequeue(); // Remove from player B's deck
                    numOfCards += 1;
                    centralPile.Enqueue(lastPlayedCardByPlayerB); // Put the removed card in the central pile
                    displayRow(round, playerAPile, playerBPile, centralPile, playerAPenalty, playerBPenalty, isPlayerAPayingPenalty, isPlayerBPayingPenalty);
                    if (isLoopFound(playerAPile, playerBPile, seenGameStates))
                    {
                        status = GameStatus.Loop;
                        gameActive = false;
                        break;
                    }
                    playerBTurn = false;
                    playerATurn = true;
                }
            }
        }

        int tricks = round++;
        Console.WriteLine($"Status is {status} tricks: {tricks} Cards {numOfCards}");
        return new GameResult(status, tricks, numOfCards);
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

    public static bool isLoopFound(Queue<string> playerAPile, Queue<string> playerBPile, HashSet<string> seenGameStates)
    {
        string gameState = getGameState(playerAPile, playerBPile);
        if (!seenGameStates.Add(gameState))
        {
            return true;
        }
        return false;
    }

    public static string getGameState(Queue<string> playerAPile, Queue<string> playerBPile) // J23 4J5 will be JXX XJX
    {
        string gameState = "";

        foreach (string card in playerAPile)
        {
            gameState += card;
        }

        gameState += " ";

        foreach (string card in playerBPile)
        {
            gameState += card;
        }

        gameState = gameState.Replace("2", "X")
                             .Replace("3", "X")
                             .Replace("4", "X")
                             .Replace("5", "X")
                             .Replace("6", "X")
                             .Replace("7", "X")
                             .Replace("8", "X")
                             .Replace("9", "X")
                             .Replace("10", "X");

        return gameState;
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
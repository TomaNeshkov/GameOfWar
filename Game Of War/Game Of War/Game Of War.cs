using System.Security.Cryptography;
using System.Text;

namespace GameOfWar
{
    internal class GameOfWar
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.OutputEncoding = Encoding.UTF8;
            const string homeScreen = @"
================================================================================
||                       Welcome to the Game of War!                          ||
||                                                                            ||
|| HOW TO PLAY:                                                               ||
|| + Each of the two players are dealt one half of a shuffled deck of cards.  ||
|| + Each turn, each player draws one card from their deck.                   ||
|| + The player that drew the card with higher value gets both cards.         ||
|| + Both cards return to the winner&#39;s deck.                              ||
|| + If there is a draw, both players place the next three cards face down    ||
|| and then another card face-up. The owner of the higher face-up             ||
|| card gets all the cards on the table.                                      ||
||                                                                            ||
|| HOW TO WIN:                                                                ||
|| + The player who collects all the cards wins.                              ||
||                                                                            ||
|| CONTROLS:                                                                  ||
|| + Press [Enter] to draw a new card until we have a winner.                 ||
||                                                                            ||
||                               Have fun!                                    ||
================================================================================
";
            Console.WriteLine(homeScreen);
            List<Card> deck = GenerateDeck();
            ShuffleDeck(deck);
            Queue<Card> firstPlayerDeck = new Queue<Card>();
            Queue<Card> secondPlayerDeck = new Queue<Card>();
            DealCardsToPlayers();
            Card FirstPlayerDeck;
            Card SecondPlayerDeck;
            int totalMoves = 0;
            while (!GameHasWinner())
            {
                Console.ReadLine();
                DrawPlayersCards();
                Queue<Card> pool = new Queue<Card>();
                pool.Enqueue(FirstPlayerDeck);
                pool.Enqueue(SecondPlayerDeck);
                ProcessWar(pool);
                DetermineRoundWinner(pool);
                Console.WriteLine("================================================================================");
                Console.WriteLine($"First player currently has {firstPlayerDeck.Count} cards.");
                Console.WriteLine($"Second player currently has {secondPlayerDeck.Count} cards");
                totalMoves++;
            }

            List<Card> GenerateDeck()
            {
                List<Card> deck = new List<Card>();
                CardFace[] faces = (CardFace[])Enum.GetValues(typeof(CardFace));
                CardSuit[] suits = (CardSuit[])Enum.GetValues(typeof(CardSuit));
                for (int suite = 0; suite < suits.Length; suite++)
                {
                    for (int face = 0; face < faces.Length; face++)
                    {
                        CardFace currentFace = faces[face];
                        CardSuit currentSuit = suits[suite];
                        deck.Add(new Card
                        {
                            Face = currentFace,
                            Suite = currentSuit
                        });

                    }
                }
                return deck;

            }
            void ShuffleDeck(List<Card> deck)
            {
                Random random = new Random();
                for (int i = 0; i < deck.Count; i++)
                {
                    int firstCardIndex = random.Next(deck.Count);
                    Card tempCard = deck[firstCardIndex];
                    deck[firstCardIndex] = deck[i];
                    deck[i] = tempCard;
                }
            }
            void DealCardsToPlayers()
            {
                while (deck.Count > 0)
                {
                    Card[] firstTwoDrawnCards = deck.Take(2).ToArray();

                    deck.RemoveRange(0, 2);
                    firstPlayerDeck.Enqueue(firstTwoDrawnCards[0]);
                    secondPlayerDeck.Enqueue(firstTwoDrawnCards[1]);
                }
            }
            bool GameHasWinner()
            {
                if (!firstPlayerDeck.Any())
                {
                    Console.WriteLine($"After a total of {totalMoves} moves, the second player has won!");
                    return true;
                }
                if (!secondPlayerDeck.Any())
                {
                    Console.WriteLine($"After a total of {totalMoves} moves, the first player has won!");
                    return true;
                }
                return false;



            }
            void DrawPlayersCards()
            {
                FirstPlayerDeck = firstPlayerDeck.Dequeue();
                Console.WriteLine($"First player has drawn: {FirstPlayerDeck}");
                SecondPlayerDeck = secondPlayerDeck.Dequeue();
                Console.WriteLine($"Second player has drwan: {SecondPlayerDeck}");
            }
            void ProcessWar(Queue<Card> pool)
            {
                while ((int)FirstPlayerDeck.Face == (int)SecondPlayerDeck.Face)
                {
                    Console.WriteLine("WAR!");
                    if (firstPlayerDeck.Count < 4)
                    {
                        AddCardsToWinnerDeck(firstPlayerDeck, secondPlayerDeck);
                        Console.WriteLine($"First player does not have enough cards to continue playing...");
                        break;
                    }
                    if (secondPlayerDeck.Count < 4)
                    {
                        AddCardsToWinnerDeck(secondPlayerDeck, firstPlayerDeck);
                        Console.WriteLine($"Second player does not have enough cards to continue playing...");
                        break;
                    }
                    AddWarCardsToPool(pool);

                    FirstPlayerDeck = firstPlayerDeck.Dequeue();
                    Console.WriteLine($"First player has drawn: {FirstPlayerDeck}");
                    SecondPlayerDeck = secondPlayerDeck.Dequeue();
                    Console.WriteLine($"Second player has drawn: {SecondPlayerDeck}");
                    pool.Enqueue(FirstPlayerDeck);
                    pool.Enqueue(SecondPlayerDeck);
                }
            }

            void DetermineRoundWinner(Queue<Card> pool)
            {
                if ((int)FirstPlayerDeck.Face > (int)SecondPlayerDeck.Face)
                {
                    Console.WriteLine("The first player has won the cards!");
                    foreach (var card in pool)
                    {
                        firstPlayerDeck.Enqueue(card);
                    }
                }
                else
                {
                    Console.WriteLine("The second player has won the cards!");
                    foreach (var card in pool)
                    {
                        secondPlayerDeck.Enqueue(card);
                    }
                }
            }

            void AddWarCardsToPool(Queue<Card> pool)
            {
                for (int i = 0; i < 3; i++)
                {
                    pool.Enqueue(firstPlayerDeck.Dequeue());
                    pool.Enqueue(secondPlayerDeck.Dequeue());
                }
            }

            void AddCardsToWinnerDeck(Queue<Card> loserDeck, Queue<Card> winnerDeck)
            {
                while (loserDeck.Count > 0)
                {
                    winnerDeck.Enqueue(loserDeck.Dequeue());
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

class Game
{
    static List<Card> PlayerCards = new List<Card>(); //players cards
    static List<Card> DealerCards = new List<Card>(); //dealers cards
    static List<Card> deck = new List<Card>(); //card deck
    static Random random = new Random(); 
    static void Main()
    {
        Console.WriteLine("Dealer stands on soft 17 \nwelcome to blackjack type Play to start or Exit to quit");
        string Start = Console.ReadLine().ToLower();
        if (Start == "play")
        {
            CardsReset();
        }
        else if (Start == "exit")
        {
            System.Environment.Exit(0);
        }
        else
        {
            Console.WriteLine("no valid input given");
            Main();
        }

    }
    static void InitialiseDeck()
    {
        var cardTypes = new List<string> { "Spades", "Diamonds", "Hearts", "Clubs" };
        var cardValues = Enum.GetValues(typeof(Cards));

        for (int d = 0; d < 4; d++) // loops the deck creation 4 times to add 4 decks
        {
            for (int c = 0; c < cardValues.Length; c++) // creates a deck
            {
                foreach (var cardType in cardTypes)
                {
                    deck.Add(new Card
                    {
                        Name = Enum.GetName(typeof(Cards), cardValues.GetValue(c)),
                        Type = cardType,
                        Value = (int)cardValues.GetValue(c)
                    });
                }
            }
        }
    }
    static void Play()
    {
        Console.Clear();
        var handValues = HandValues(); // calls the hand values
        int playerHandValue = handValues.PlayerHandValue; // gets the player hand value
        int dealerHandValue = handValues.DealerHandValue; // gets the dealer hand value

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Player cards: {string.Join(", ", PlayerCards.Select(card => card.Name))}, total: {playerHandValue}"); // prints all card names only from player cards. using string.join to make it so there is a comma seperating each card name
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Dealer cards: {DealerCards[0].Name}, ?");
        Console.ResetColor();
        if (playerHandValue > 21) //cheks for player bust
        {
            Console.WriteLine("Bust! you lose \n\n");
            Main();
        } 
        else if (playerHandValue == 21 && dealerHandValue < 21 && PlayerCards.Count == 2 ) //checks for blackjack
        {
            Console.WriteLine("blackjack! you win \n\n");
            Main();
        }
        else if (playerHandValue == 21 && dealerHandValue < 21) //checks for if the player wins on 21 without blackjack
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Dealer cards: {string.Join(", ", DealerCards.Select(card => card.Name))}, total: {dealerHandValue}");
            Console.ResetColor();
            Console.WriteLine("21! you win \n\n");
            Main();
        }
        Console.WriteLine("type Hit to hit, type Stand to stand");
        string Action = Console.ReadLine().ToLower();
        if (Action == "hit") //gives the player another card
        {
            Card newCard = NextCard();
            if (newCard == null) return;
            PlayerCards.Add(newCard);
            Play();
        }
        else if(Action == "stand") //starts the dealers moves
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Player cards: {string.Join(", ", PlayerCards.Select(card => card.Name))}, total: {playerHandValue}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Dealer cards: {string.Join(", ", DealerCards.Select(card => card.Name))}, total: {dealerHandValue}");
            Console.ResetColor();
            Stand();
        }
        else
        {
            Console.WriteLine("no valid input given");
            Play();
        }
    }
    static Card NextCard()
    {
        if (deck.Count == 0) //checks to see if the deck is empty
        {
            Console.WriteLine("Deck is empty!");
            return null;
        }

        int index = random.Next(deck.Count); //Pick a random index
        Card drawnCard = deck[index]; //Get the card from that index
        deck.RemoveAt(index); //Remove the card from the deck
        return drawnCard;
    }
    static void Stand()
    {
        var handValues = HandValues(); //Get current hand values
        int playerHandValue = handValues.PlayerHandValue; //Player's hand value
        int dealerHandValue = handValues.DealerHandValue; //Dealer's hand value

        bool isSoft17 = dealerHandValue == 17 && DealerCards.Any(card => card.Name == "Ace" && card.Value == 11); //Check if the dealer has a soft 17 (hand value is 17 and includes an Ace counted as 11)

        while (dealerHandValue < 17 || (dealerHandValue == 17 && !isSoft17)) //Dealer stands on soft 17, hits on hard 17 or less
        {
            Card newCard = NextCard();
            if (newCard == null) return; 

            DealerCards.Add(newCard); //gives the dealer a new card
            handValues = HandValues(); // Recalculate hand values
            dealerHandValue = handValues.DealerHandValue;

            Console.WriteLine("dealer Hits");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Dealer cards: {string.Join(", ", DealerCards.Select(card => card.Name))}, total: {dealerHandValue}");
            Console.ResetColor();

            isSoft17 = dealerHandValue == 17 && DealerCards.Any(card => card.Name == "Ace" && card.Value == 11); //Recheck if the dealer now has a soft 17
        }

        //checks the game outcome
        if (playerHandValue > 21)
        {
            Console.WriteLine("Bust, player lost \n\n");
        }
        else if (dealerHandValue > 21)
        {
            Console.WriteLine("Dealer bust, player won \n\n");
        }
        else if (playerHandValue > dealerHandValue)
        {
            Console.WriteLine("Player won \n\n");
        }
        else if (playerHandValue == dealerHandValue)
        {
            Console.WriteLine("Push \n\n");
        }
        else
        {
            Console.WriteLine("Player lost! \n\n");
        }

        Main(); 
    }
    static void CardsReset()
    {
        deck.Clear(); //clears the deck
        InitialiseDeck(); //creates the deck
        PlayerCards.Clear(); //wipes players hand
        DealerCards.Clear(); //wipes dealers hand
        for (int i = 0; i < 2; i++) //gives the player and dealer 2 random cards from the deck
        {
            Card playerCard = NextCard();
            if (playerCard == null) return;
            Card dealerCard = NextCard();
            if (dealerCard == null) return;
            PlayerCards.Add(playerCard);
            DealerCards.Add(dealerCard);
        }
        Play();
    }
    static (int PlayerHandValue, int DealerHandValue) HandValues()
    {
        int CalculateHandValue(List<Card> hand) //calculates the total of the deck passed through
        {
            int totalValue = 0;
            int aceCount = 0;

            foreach (var card in hand)
            {
                if (card.Value == (int)Cards.Ace) //makes the ace worth 11 
                {
                    totalValue += 11;
                    aceCount++;
                }
                else
                {
                    totalValue += card.Value;
                }
            }
            while (totalValue > 21 && aceCount > 0) //makes the ace worth 1 if the total value is larger than 21
            {
                totalValue -= 10;
                aceCount--;
            }
            return totalValue;
        }
        //calculate hand values for Player and Dealer
        int PlayerHandValue = CalculateHandValue(PlayerCards);
        int DealerHandValue = CalculateHandValue(DealerCards);

        //return both values
        return (PlayerHandValue, DealerHandValue);
    }
}
enum Cards
{
    Ace = 1, Two, Three, Four, Five, Six,
    Seven, Eight, Nine, Ten, Jack = 10, Queen = 10, King = 10
}
class Card
{
    public string Name { get; set; }
    public string Type { get; set; }
    public int Value { get; set; }
}
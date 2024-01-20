using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace CardGames.Domain;

public class CardDeck : ICardDeck
{

    private List<ICard> Cards { get; }
    private static Random RandomNumber { get; } = new Random();

    public CardDeck()
    {
        Cards = new List<ICard>();
        //creates a card deck in a ordered way
        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
            {
                Card card = new Card(suit, rank);
                Cards.Add(card);
            }
        }

    }


    private CardDeck(List<ICard> cardsCollection)
    {
        Cards = cardsCollection;
    }

    public int RemainingCards { 
        get 
        {
             return Cards.Count;
        }
    }

    public ICard DealCard()
    {   
        if (Cards.Count <= 0 || Cards.Count > 52)
        {
            throw new InvalidOperationException();
        }
        ICard lastCard = Cards[Cards.Count - 1];
        Cards.RemoveAt(Cards.Count - 1);
        return lastCard;
    }

    public void Shuffle()
    {
        int[] usedNumbers = new int[0];

        for (int i = 0; i < Cards.Count; i++)
        {
            int selectedNumber = RandomNumber.Next(0, Cards.Count);

            if (!usedNumbers.Contains(selectedNumber))
            {
                ICard tempCard = Cards[i];
                Cards[i] = Cards[selectedNumber];
                Cards[selectedNumber] = tempCard;
            }
            usedNumbers.Append(i);
            usedNumbers.Append(selectedNumber);
        }
    }

    public IList<CardDeck> SplitBySuit()
    {
        // Groepeer de kaarten in de CardDeck op basis van hun 'Suit' (kleur)
        var suitGroups = Cards.GroupBy(card => card.Suit);

        // Maak een lijst (List) van CardDeck-objecten voor elke groep
        var suitDecks = suitGroups.Select(group =>
        {
            // Maak een nieuw CardDeck voor de huidige groep (suit)
            var suitDeck = new CardDeck(group.ToList());
            return suitDeck;
        }).ToList();

        // Geef de lijst van CardDeck-objecten terug
        return suitDecks;

    }

    public ICardDeck WithoutCardsRankingLowerThan(CardRank minimumRank)
    {
        //selecteer van de CardDeck enkel de kaarten die groter of gelijk zijn aan minimumRank en maak er een lijst van
        var biggerEqualThanCardDeck = Cards.Where(card => card.Rank >= minimumRank).ToList();
        // Maak een nieuwe ICardDeck aan met de g
        ICardDeck selectedCardDeck = new CardDeck(biggerEqualThanCardDeck); 
        return selectedCardDeck;
    }

}
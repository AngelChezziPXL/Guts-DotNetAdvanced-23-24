namespace CardGames.Domain;

public class HigherLowerGame
{
    private ICardDeck Cards { get; set; }       // ADDED PROPERTY
    private int RequiredNumberOfCorrectGuesses { get; set; }    // ADDED PROPERTY
    public ICard CurrentCard { get; private set; }
    public ICard? PreviousCard { get; private set; }

    public int NumberOfCorrectGuesses { get; private set; }

    public string? Motivation { get; private set; }

    public bool HasWon 
    { 
        get 
        {  
            return CompareCorrectGuesses(); 
        } 
    }

    public HigherLowerGame(ICardDeck standardDeck, int requiredNumberOfCorrectGuesses, CardRank minimumRank = CardRank.Ace)
    {
        Cards = standardDeck.WithoutCardsRankingLowerThan(minimumRank);
        Cards.Shuffle();
        CurrentCard = Cards.DealCard();
        RequiredNumberOfCorrectGuesses = requiredNumberOfCorrectGuesses;
        NumberOfCorrectGuesses = 0;

    }

    public void MakeGuess(bool higher)
    {
        DealNewCard();

        if (CurrentCard is not null && PreviousCard is not null)
            {
                if ((CurrentCard.Rank >= PreviousCard.Rank) && (higher == true))
                {
                    CorrectGuess();
                }
                else if ((CurrentCard.Rank <= PreviousCard.Rank) && (higher == false)) {
                    CorrectGuess();
                }
                else
                {
                WrongGuess();
                }
            }
    }

    //added methods
    
    private bool CompareCorrectGuesses() //added method
    {
        if (NumberOfCorrectGuesses == RequiredNumberOfCorrectGuesses)
        {
            NumberOfCorrectGuesses = 0;
            return true;
            
        }
        else 
        { 
            return false; 
        }
    }
    private void CorrectGuess()   //added method
    {
        NumberOfCorrectGuesses++;
        
        int NumberOfGuessesRemaining = RequiredNumberOfCorrectGuesses - NumberOfCorrectGuesses;
        
        if (NumberOfGuessesRemaining <= 3)
        {
            Motivation = $"Only {NumberOfGuessesRemaining.ToString()} card(s) to go!";
        }
        else
        {
            Motivation = null;
        }
        
    }

    private void WrongGuess()       //added method
    {
        NumberOfCorrectGuesses = 0;
        Motivation = null;
    }

    private void DealNewCard()      //added method
    {
        PreviousCard = CurrentCard;
        CurrentCard = Cards.DealCard();
    }
}
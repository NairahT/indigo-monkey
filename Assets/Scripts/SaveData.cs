using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int savedScore;
    public int savedStreak;
    public int savedCardCounter;
    public List<CardData> savedCards = new List<CardData>();

    // The cards the player had selected
    public Card savedFirstCard;
    public Card savedSecondCard;

    // Save the shuffled order of cards
    public List<int> savedShuffledOrder = new List<int>();
}

[Serializable]
public class CardData
{
    public int cardId; // Index of card in the scene hierarchy, which is its shuffled position
    public CardType cardType; //Which card type it was
    public bool isFlipped; // Track if the card is currently flipped
}
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int savedScore;
    public int savedStreak;
    public int savedCardCounter;
    public List<CardData> cards = new List<CardData>();
}

[Serializable]
public class CardData
{
    public int cardId; // Index of card in the scene hierarchy, which is its shuffled position
    public CardType cardType; //Which card type it was
    public bool isFlipped; // Track if the card is currently flipped
}
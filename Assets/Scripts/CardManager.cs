using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Transform gridContainer;

    private Dictionary<int, Card> cardsDictionary; // Stores the hierarchy index and the corresponding Card
    public Card[] cards;

    public List<int> currentShuffledOrder; // Stores the shuffled order during runtime
    public Card firstCard;
    public void InitializeCards()
    {
        // Initialize the dictionary with the card's index and card object
        cardsDictionary = new Dictionary<int, Card>();
        for (int i = 0; i < cards.Length; i++)
        {
            cardsDictionary.Add(i, cards[i]);
        }
    }

    public void ShuffleCards()
    {
        // Create a list of keys (indices) to shuffle
        List<int> keys = new List<int>(cardsDictionary.Keys);

        // Shuffle the keys list
        for (int i = keys.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            int temp = keys[i];
            keys[i] = keys[randomIndex];
            keys[randomIndex] = temp;
        }

        // Apply the shuffled order by updating the sibling indices of the cards
        for (int i = 0; i < keys.Count; i++)
        {
            int key = keys[i];
            cardsDictionary[key].transform.SetSiblingIndex(i);
        }

        // Store the current shuffled order to save later
        currentShuffledOrder = new List<int>(keys);
    }

    public void ApplyShuffledOrder(List<int> savedOrder)
    {
        if (savedOrder == null || savedOrder.Count != cards.Length)
        {
            Debug.LogWarning("Invalid shuffle order. Unable to apply.");
            return;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            cards[savedOrder[i]].transform.SetSiblingIndex(i);
        }
        currentShuffledOrder = savedOrder;

    }

    public Card GetFirstCard(int firstIndex)
    {
        int temp = currentShuffledOrder[firstIndex];
        firstCard = cards[temp];
        return firstCard;
    }
}

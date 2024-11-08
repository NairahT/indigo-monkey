using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    public Transform gridContainer;

    private Dictionary<int, Card> cardsDictionary; // Stores the hierarchy index and the corresponding Card
    public Card[] cards;

    [SerializeField]
    private List<int> currentShuffledOrder; // Stores the shuffled order during runtime

    [SerializeField]
    private Card firstCard;
    [SerializeField]
    private Card secondCard;

    private int cardCounter; //To keep track of the current pair of selected cards

    private int score;
    private int streak;
    private int scoreToWin;

    // To update the score and streak score text
    public TMP_Text scoreText;
    public TMP_Text streakText;

    private string saveFilePath;

    private bool didWin;

    void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savedData.json");

        // Calculate score needed to win based on maximum amount of matches that can be made * 10 for the score value
        scoreToWin = cards.Length / 2;
        scoreToWin *= 10;


        if (File.Exists(saveFilePath))
        {
            LoadGame();
        } else
        {
            InitializeGame();
        }
    }

    private void InitializeGame()
    {
        cardCounter = 0;
        score = 0;
        streak = 0;

        UpdateScoreAndStreakUI();

        // Initialize the dictionary with the card's index and card object
        cardsDictionary = new Dictionary<int, Card>();
        for (int i = 0; i < cards.Length; i++)
        {
            cardsDictionary.Add(i, cards[i]);
        }

        ShuffleCards();

        StartCoroutine(ShowCardsAtStart());
    }

    void ShuffleCards()
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

    private void OnApplicationQuit()
    {
        // Only save if the player hasn't won the round yet
        if(!didWin)
        {
            SaveGame();
        }
    }

    private void WinGame()
    {
        Debug.Log("Game won!");
        didWin = true;

        // Check if the save file exists, and delete it as we will start a fresh game with no need to load an old save
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }

    private void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            savedScore = score,
            savedStreak = streak,
            savedCardCounter = cardCounter,
            savedFirstCard = firstCard,
            savedSecondCard = secondCard
        };

        foreach (Card card in cards)
        {
            CardData cardData = new CardData
            {
                cardId = card.transform.GetSiblingIndex(), // Use the sibling index as the ID
                cardType = card.cardType,
                isFlipped = card.cardState == CardState.Flipped 
            };
            saveData.savedCards.Add(cardData);
        }

        // Save the shuffled order of cards
        saveData.savedShuffledOrder = currentShuffledOrder;

        // Convert saveData to JSON
        string json = JsonUtility.ToJson(saveData, true);

        // Write JSON to file
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved: " + saveFilePath);
    }

    private void LoadGame()
    {
        // Read JSON from file
        string json = File.ReadAllText(saveFilePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        // Load score and streak
        score = saveData.savedScore;
        streak = saveData.savedStreak;
        cardCounter = saveData.savedCardCounter;
        currentShuffledOrder = saveData.savedShuffledOrder;

        if (saveData.savedFirstCard != null)
        {
            firstCard = saveData.savedFirstCard;
        }

        if (saveData.savedSecondCard != null)
        {
            secondCard = saveData.savedSecondCard;
        }

        UpdateScoreAndStreakUI();

        for (int i = 0; i < cards.Length; i++)
        {
            cards[currentShuffledOrder[i]].transform.SetSiblingIndex(i);
        }

        // Load card states
        foreach (CardData cardData in saveData.savedCards)
        {
            Card card = gridContainer.GetChild(cardData.cardId).GetComponent<Card>();

            if (card != null)
            {
                card.cardType = cardData.cardType;
                if (cardData.isFlipped)
                {
                    card.FlipOpen();
                }
            }
        }

        Debug.Log("Game Loaded");
    }


    public void RestartGame()
    {
        InitializeGame();
    }

    private IEnumerator ShowCardsAtStart()
    {

        for(int i =0; i <cards.Length; i++)
        {
            cards[i].FlipOpen();
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].FlipClose();
        }
    }

    void OnEnable()
    {
        Card.OnCardSelected += HandleCardSelection;
    }

    void OnDisable()
    {
        Card.OnCardSelected -= HandleCardSelection;
    }

    private void HandleCardSelection(Card card)
    {
        if (cardCounter == 0) // Player selected the first card
        {
            firstCard = card;
            cardCounter++;
        } 
        else if (cardCounter == 1) // Player selected the second card
        {
            secondCard = card;
            CompareCards(); // Player has now chosen two cards, compare them
            cardCounter = 0;
        }
    }

    private void CompareCards()
    {
        if(firstCard.cardType == secondCard.cardType)
        {
            Debug.Log($"Match made successfully between {firstCard.cardType} and {secondCard.cardType}");
            
            firstCard.animator.SetTrigger("Win");
            secondCard.animator.SetTrigger("Win");

            firstCard.DisableClicking();
            secondCard.DisableClicking();

            score += 10;
            streak++;

            if (score == scoreToWin) // Score has reached its maximum. Player wins!
            {
                WinGame();
            }
        } 
        else
        {
            Debug.Log($"Unsuccessful card match between {firstCard.cardType} and {secondCard.cardType}");
            firstCard.animator.SetTrigger("Close");
            secondCard.animator.SetTrigger("Close");


            streak = 0;
            firstCard.cardState = CardState.Unflipped;
            secondCard.cardState = CardState.Unflipped;
        }

        firstCard = null;
        secondCard = null;

        UpdateScoreAndStreakUI();
    }

    private void UpdateScoreAndStreakUI()
    {
        scoreText.text = score.ToString();
        streakText.text = streak.ToString();
    }
}

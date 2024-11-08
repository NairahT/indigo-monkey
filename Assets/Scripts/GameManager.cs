using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    public Transform gridContainer;
    public Card[] cards;

    public Card firstCard;
    public Card secondCard;

    public int cardCounter; //To keep track of the current pair of selected cards

    private int score;
    private int streak;
    private int scoreToWin;

    public TMP_Text scoreText;
    public TMP_Text streakText;

    private string saveFilePath;
    void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savedData.json");
        Debug.Log(saveFilePath);

        InitializeGame();
      //  LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void WinGame()
    {
        Debug.Log("Game won!");
    }

    private void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            savedScore = score,
            savedStreak = streak,
            savedCardCounter = cardCounter
        };

        foreach (Card card in cards)
        {
            CardData cardData = new CardData
            {
                cardId = card.transform.GetSiblingIndex(), // Use the sibling index as the ID
                cardType = card.cardType,
                isFlipped = card.cardState == CardState.Flipped
                
            };
            saveData.cards.Add(cardData);
        }

        // Convert saveData to JSON
        string json = JsonUtility.ToJson(saveData, true);

        // Write JSON to file
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved: " + saveFilePath);
    }

    private void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            // Read JSON from file
            string json = File.ReadAllText(saveFilePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            // Load score and streak
            score = saveData.savedScore;
            streak = saveData.savedStreak;
            cardCounter = saveData.savedCardCounter;

            UpdateScoreAndStreakUI();

            // Load card states
            foreach (CardData cardData in saveData.cards)
            {
                Card card = gridContainer.GetChild(cardData.cardId).GetComponent<Card>();
                if (card != null)
                {
                    card.cardType = cardData.cardType;
                    if (cardData.isFlipped)
                    {
                        card.FlipOpen();
                    }
                    else
                    {
                        card.FlipClose();
                    }
                }
            }

            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No save file found. Starting new game.");
        }
    }

    void Shuffle(Card[] cards)
    {
        for (int i = cards.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Card temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    private void InitializeGame()
    {
        cardCounter = 0;
        score = 0;
        streak = 0;


        UpdateScoreAndStreakUI();

        cards = FindObjectsOfType<Card>();

        // Calculate score needed to win based on maximum amount of matches that can be made * 10 for the score value
        scoreToWin = cards.Length / 2;
        scoreToWin *= 10;
        
        Shuffle(cards);

        // Update the hierarchy based on the shuffled array
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].transform.SetSiblingIndex(i);
        }

        StartCoroutine(ShowCardsAtStart());
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

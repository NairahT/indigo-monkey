using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CardManager cardManager;
    public SaveLoadManager saveLoadManager;

    public Card firstCard;
    public Card secondCard;

    public int cardCounter; //To keep track of the current pair of selected cards

    public int score;
    public int streak;
    public int scoreToWin;

    // To update the score and streak score text
    public TMP_Text scoreText;
    public TMP_Text streakText;

    public GameObject GameOverMenu;

    private bool didWin;

    void Start()
    {
        didWin = false;

        // Calculate score needed to win based on maximum amount of matches that can be made * 10 for the score value
        scoreToWin = cardManager.cards.Length / 2;
        scoreToWin *= 10;

        // Check if there's save data to be loaded, else initialize the game as new
        SaveData saveData = saveLoadManager.LoadGame();

        if(saveData!=null)
        {
            LoadGame(saveData);
        } 
        else
        {
            InitializeGame();
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
    private void InitializeGame()
    {
        cardCounter = 0;
        score = 0;
        streak = 0;
        didWin = false;

        UpdateScoreAndStreakUI();

        cardManager.InitializeCards();
        cardManager.ShuffleCards();

        StartCoroutine(ShowCardsAtStart());
    }

    // Show players the cards at the start for them to memorize
    private IEnumerator ShowCardsAtStart()
    {
        foreach (Card card in cardManager.cards)
        {
            card.FlipOpen();
        }

        yield return new WaitForSeconds(2f);

        foreach (Card card in cardManager.cards)
        {
            card.FlipClose();
        }
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
        if (firstCard.cardType == secondCard.cardType)
        {
            Debug.Log($"Match made successfully between {firstCard.cardType} and {secondCard.cardType}");

            AudioManager.instance.PlayMatchAudio();

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

            AudioManager.instance.PlayWrongAudio();

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

    private void OnApplicationQuit()
    {
        // Only save data if the player hasn't won the round yet
        if(!didWin)
        {
            saveLoadManager.SaveGame(this, cardManager);
        }
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause() // Using OnApplicationPause for Android as OnApplicationQuit is not always called when apps are closed 
    {
        Debug.Log("Called on android");

        if (!didWin)
        {
            saveLoadManager.SaveGame(this, cardManager);
        }
    }
#endif

    private void WinGame()
    {
        Debug.Log("Game won!");
        didWin = true;

        // Delete save file as we will start a fresh game 
        saveLoadManager.DeleteSaveFile();

        // Turn Game Over menu after a short delay
        StartCoroutine(DelayedGameMenuPopUp());
    }

    private IEnumerator DelayedGameMenuPopUp()
    {
        yield return new WaitForSeconds(2f);

        //Enable the game over menu and play winning audio clip
        GameOverMenu.SetActive(true);
        AudioManager.instance.PlayWinAudio();
    }

    private void LoadGame(SaveData saveData)
    {
        score = saveData.savedScore;
        streak = saveData.savedStreak;
        cardCounter = saveData.savedCardCounter;

        if (saveData.savedFirstCard != null)
        {
            firstCard = saveData.savedFirstCard;
        }

        if (saveData.savedSecondCard != null)
        {
            secondCard = saveData.savedSecondCard;
        }

        UpdateScoreAndStreakUI();

        cardManager.ApplyShuffledOrder(saveData.savedShuffledOrder);

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

    private void UpdateScoreAndStreakUI()
    {
        scoreText.text = score.ToString();
        streakText.text = streak.ToString();
    }

    public void RestartGame()
    {
        saveLoadManager.DeleteSaveFile();
        SceneManager.LoadScene(0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


public class GameManager : MonoBehaviour
{
    public Card[] cards;

    public Card firstCard;
    public Card secondCard;

    public int cardCounter; //To keep track of the current pair of selected cards

    private int score;
    private int streak;

    public TMP_Text scoreText;
    public TMP_Text streakText;

    void Start()
    {
        InitializeGame();
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

            score += 10;
            streak++;
            
        } 
        else
        {
            

            Debug.Log($"Unsuccessful card match between {firstCard.cardType} and {secondCard.cardType}");
            firstCard.animator.SetTrigger("Close");
            secondCard.animator.SetTrigger("Close");


            streak = 0;
        }

        firstCard.cardState = CardState.Unflipped;
        secondCard.cardState = CardState.Unflipped;

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

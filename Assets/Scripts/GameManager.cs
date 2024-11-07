using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


public class GameManager : MonoBehaviour
{
    public Card firstCard;
    public Card secondCard;

    private int cardCounter; //To keep track of the current pair of selected cards

    private int score;
    private int streak;

    public TMP_Text scoreText;
    public TMP_Text streakText;

    void Start()
    {
        cardCounter = 0;
        score = 0;
        streak = 0;

        UpdateScoreAndStreakUI();
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

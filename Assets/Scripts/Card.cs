using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Card : MonoBehaviour
{
    public static event Action<Card> OnCardSelected;

    public CardType cardType;
    public CardState cardState;

    public Animator animator;

    private void OnMouseDown()
    {
        Debug.Log($"Clicked card of type {cardType}");

        if(cardState == CardState.Unflipped)
        {
            OnCardSelected?.Invoke(this);
            animator.SetTrigger("Open");

            cardState = CardState.Flipped;
        }
    }

    public void FlipOpen()
    {
        if (cardState == CardState.Unflipped)
        {
            animator.SetTrigger("Open");
            cardState = CardState.Flipped;
        }
    }

    public void FlipClose()
    {
        animator.SetTrigger("Close");
        cardState = CardState.Unflipped;
    }
}

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
            animator.SetTrigger("Open");
            cardState = CardState.Flipped;
            OnCardSelected?.Invoke(this);
           
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

    public void DisableClicking()
    {
        BoxCollider2D cardCollider = GetComponent<BoxCollider2D>();
        if(cardCollider!=null)
        {
            cardCollider.enabled = false;
        }
    
    }
}

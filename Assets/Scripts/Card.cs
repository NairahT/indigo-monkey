using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Card : MonoBehaviour
{
    public static event Action<Card> OnCardSelected;

    public CardType cardType;
    public CardState cardState;

    private void OnMouseDown()
    {
        Debug.Log($"Clicked card of type {cardType}");

        if(cardState == CardState.Unflipped)
        {
            OnCardSelected?.Invoke(this);
        }
    }
}

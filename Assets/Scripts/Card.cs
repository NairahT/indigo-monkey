using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    private CardType cardType;

    [SerializeField]
    private CardState cardState;

    private void OnMouseDown()
    {
        Debug.Log($"Clicked card of type {cardType}");
    }
}

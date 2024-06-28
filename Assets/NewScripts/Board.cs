using System;
using System.Collections;
using System.Collections.Generic;
using Console;
using UnityEngine;

public class Board
{
    public delegate void NoSelectedDeck();
    public event NoSelectedDeck noSelectedDeck;
    Player player1;
    Player player2;
    Deck selectedDeck;

    public void StartGame()
    {
        if (selectedDeck == null)
        {
            noSelectedDeck();
        }
        Debug.Log("Juego listo para empezar");
    }

    public void SetSelectedDeck(Guid id)
    {
        selectedDeck = Store.GetDeck(id);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Console;
using UnityEngine;

public class Board
{
    System.Random random = new System.Random();
    public delegate void NoSelectedDeck();
    public delegate void InstantiateHands(List<Cards> player1Hand, List<Cards> player2Hand);
    public event NoSelectedDeck noSelectedDeck;
    public event InstantiateHands instantiateHands;
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
        GeneratePlayers();
    }

    public void SetSelectedDeck(Guid id)
    {
        selectedDeck = Store.GetDeck(id);
        Debug.Log($"{selectedDeck.cards[0].name}");
    }

    public void GeneratePlayers()
    {
        int index = random.Next(0, Store.decks.Count);
        player1 = new Player(selectedDeck);
        player2 = new Player(Store.decks[index]);
        GenerateHands();
    }

    public void GenerateHands()
    {
        player1.GenerateHand();
        player2.GenerateHand();
        Debug.Log($"{player1.hand[0].name} y {player2.hand[0].name}");
        instantiateHands(player1.hand, player2.hand);
    }
}

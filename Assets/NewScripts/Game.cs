using System;
using System.Collections;
using System.Collections.Generic;
using Console;
using UnityEngine;

public class Game
{

    System.Random random = new System.Random();
    public delegate void NoSelectedDeck();
    public delegate void InstantiateHands(List<Cards> player1Hand, List<Cards> player2Hand);
    public event NoSelectedDeck noSelectedDeck;
    public event InstantiateHands instantiateHands;
    public Player player1;
    public Player player2;
    public Player activePlayer;
    Deck selectedDeck;

    public void StartGame()
    {
        if (selectedDeck == null)
        {
            noSelectedDeck();
        }
        Debug.Log("Juego listo para empezar");
        GeneratePlayers();
        activePlayer = player1;
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
        Debug.Log($"{player1.hand.cards[0].name} y {player2.hand.cards[0].name}");
        instantiateHands(player1.hand.cards, player2.hand.cards);
    }

    public void PlayCard(Cards card, GameObject panel)
    {
        /*switch(panel)
        {
            case
        }*/
        //activePlayer.field.AddCard(card, );
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Console;
using UnityEngine;

public class Game
{
    int player2Points;
    int player1Points;
    Context context;
    System.Random random = new System.Random();
    public delegate void NoSelectedDeck();
    public delegate void InstantiateHands(List<Cards> player1Hand, List<Cards> player2Hand);
    public delegate void UpdatePlayersPoints(int player1Points, int player2Points);
    public delegate void PassTurn();
    public event NoSelectedDeck noSelectedDeck;
    public event InstantiateHands instantiateHands;
    public event PassTurn passTurn;
    public event UpdatePlayersPoints updatePoints;
    public Player player1;
    public Player player2;
    public Player activePlayer;
    Deck selectedDeck;

    public void StartGame()
    {
        player1Points = 0;
        player2Points = 0;
        if (selectedDeck == null)
        {
            noSelectedDeck();
        }
        Debug.Log("Juego listo para empezar");
        GeneratePlayers();
        activePlayer = player1;
        context = new Context(new List<Cards>(), activePlayer.id);
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
        activePlayer.field.SendButtom(card);
        context.board.Add(card);
        ActiveEffect();
        ChangesActivePlayer();
        UpdatePoints();
        passTurn();
    }

    private void ChangesActivePlayer()
    {
        if (IsPlayer1Playing())
        {
            activePlayer = player2;
            context.triggerPlayer = player2.id;
        }
        else
        {
            activePlayer = player1;
            context.triggerPlayer = player1.id;
        }
    }

    public bool IsPlayer1Playing()
    {
        return activePlayer == player1 ? true : false;
    }

    private void ActiveEffect()
    {

    }

    public void UpdatePoints()
    {
        player1Points = player1.field.GetPoints();
        player2Points = player2.field.GetPoints();
        updatePoints(player1Points, player2Points);
    }
}

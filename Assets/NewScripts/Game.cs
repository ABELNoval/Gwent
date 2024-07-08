using System;
using System.Collections;
using System.Collections.Generic;
using Console;
using Unity.VisualScripting;
using UnityEngine;

public class Game
{
    public bool player1IsPlaying;
    public bool player2IsPlaying;
    public int player2Points;
    public int player1Points;
    Context context;
    System.Random random = new System.Random();
    public delegate void NoSelectedDeck();
    public delegate void InstantiateHands(List<Cards> player1Hand, List<Cards> player2Hand);
    public delegate void Draw(List<Cards> player1Cards, List<Cards> player2cards);
    public delegate void UpdatePlayersPoints(int player1Points, int player2Points);
    public delegate void PassTurn();
    public event NoSelectedDeck noSelectedDeck;
    public event InstantiateHands instantiateHands;
    public event PassTurn passTurn;
    public event Draw draw;
    public event UpdatePlayersPoints updatePoints;
    public Player player1;
    public Player player2;
    public Player activePlayer;
    public int player1Wons;
    public int player2Wons;
    Deck selectedDeck;

    public void StartGame()
    {
        player1IsPlaying = true;
        player2IsPlaying = true;
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
        UpdatePoints();
        passTurn();
    }

    public void ChangesActivePlayer()
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

    public void FinishRound()
    {

    }

    public void DrawCards(int cant)
    {
        List<Cards> player1Cards = new List<Cards>();
        List<Cards> player2Cards = new List<Cards>();
        for (int i = 0; i < cant; i++)
        {
            player1.deck.DrawCard();
            player1Cards.Add(player1.deck.cards[player1.deck.cards.Count - 1]);
            player2.deck.DrawCard();
            player2Cards.Add(player2.deck.cards[player2.deck.cards.Count - 1]);
        }
        draw(player1Cards, player2Cards);
    }
}

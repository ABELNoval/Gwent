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
    public int player1Wins;
    public int player2Wins;
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
        context.findPlayer += FindPlayer;
    }

    public void SetSelectedDeck(Guid id)
    {
        selectedDeck = Store.GetDeck(id);
        Debug.Log($"{selectedDeck.cards[0].name}");
    }

    public void GeneratePlayers()
    {
        int index = random.Next(0, Store.decks.Count);
        Deck player1Deck = new Deck(selectedDeck.id, selectedDeck.name);
        foreach (Cards cards in selectedDeck.cards)
        {
            player1Deck.SendButtom(cards);
        }

        Deck player2Deck = new Deck(Store.decks[index].id, Store.decks[index].name);
        foreach (Cards cards in Store.decks[index].cards)
        {
            player2Deck.SendButtom(cards);
        }
        player1 = new Player(player1Deck);
        player2 = new Player(player2Deck);
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
        activePlayer.hand.Remove(card);
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

    public void StopPlaying()
    {
        if (IsPlayer1Playing())
        {
            player2.isPlaying = false;
            return;
        }
        player1.isPlaying = false;
    }

    public void RefrechBoard()
    {
        ClearCards(null);
        UpdatePoints();
        DrawCards(2);
    }

    public void FinishRound()
    {

    }

    private void ClearCards(Predicate<Cards> predicate)
    {
        int i = 0;
        int j = 0;
        if (predicate == null)
        {
            while (i < player1.field.cards.Count)
            {
                Cards card = player1.field.Pop();
                Debug.Log($"{card.name}");
                player1.graveyard.Push(card);
            }

            while (j < player2.field.cards.Count)
            {
                Cards card = player2.field.Pop();
                player2.graveyard.Push(card);
            }
        }
        else
        {
            List<Cards> cards1 = player1.field.Find(predicate);
            foreach (Cards card in cards1)
            {
                player1.field.Remove(card);
                player1.graveyard.Push(card);
            }

            List<Cards> cards2 = player2.field.Find(predicate);
            foreach (Cards card in cards2)
            {
                player2.field.Remove(card);
                player2.graveyard.Push(card);
            }
        }
    }

    public void DrawCards(int cant)
    {
        List<Cards> player1Cards = new List<Cards>();
        List<Cards> player2Cards = new List<Cards>();
        for (int i = 0; i < cant; i++)
        {
            Cards card1 = player1.deck.DrawCard();
            Cards card2 = player2.deck.DrawCard();
            if (player1.hand.cards.Count < 10)
            {
                player1Cards.Add(card1);
                player1.hand.SendButtom(card1);
            }
            else
            {
                player1.graveyard.SendButtom(card1);
            }

            if (player2.hand.cards.Count < 10)
            {
                player2Cards.Add(card2);
                player2.hand.SendButtom(card2);
            }
            else
            {
                player2.graveyard.SendButtom(card2);
            }

        }
        draw(player1Cards, player2Cards);
    }

    private Player FindPlayer(Guid id)
    {
        if (player1.id == id)
        {
            return player1;
        }
        return player2;
    }
}

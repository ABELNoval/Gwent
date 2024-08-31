using System;
using System.Collections.Generic;
using Console;
using UnityEngine;

public class Game
{
    public bool player1IsPlaying;
    public bool player2IsPlaying;
    public int player2Points;
    public int player1Points;
    System.Random random = new System.Random();
    public delegate void InvalidDeck();
    public delegate void InstantiateHands(List<Cards> player1Hand, List<Cards> player2Hand);
    public delegate void Start();
    public delegate void Draw(List<Cards> player1Cards, List<Cards> player2cards);
    public delegate void UpdatePlayersPoints(int player1Points, int player2Points);
    public delegate void PassTurn();
    public delegate void UpdateHand(Player player);
    public delegate void RemoveCard(Player player, Cards card);
    public event UpdateHand updateHand;
    public event RemoveCard removeCard;

    public event InvalidDeck invalidDeck;
    public event InstantiateHands instantiateHands;
    public event PassTurn passTurn;
    public event Draw draw;
    public event Start start;
    public event UpdatePlayersPoints updatePoints;
    public Player player1;
    public Player player2;
    public Player activePlayer;
    public int player1Wins;
    public int player2Wins;
    public Deck selectedDeck { get; private set; }


    public void StartGame()
    {
        if (!IsAValidDeck(selectedDeck))
        {
            invalidDeck();
        }
        else
        {
            player1IsPlaying = true;
            player2IsPlaying = true;
            player1Points = 0;
            player2Points = 0;
            Debug.Log("Juego listo para empezar");
            GeneratePlayers();
            activePlayer = player1;
            Context.board = new Board();
            Context.triggerPlayer = activePlayer.id;
            Context.findPlayer += FindPlayer;
            Context.findPlayerWithString += FindPlayerWithString;
            start();
        }
    }


    public void SetSelectedDeck(Guid id)
    {
        selectedDeck = Store.GetDeck(id);
    }

    private bool IsAValidDeck(Deck deck)
    {
        if (deck != null)
        {
            if (deck.cards.Count >= 25 && GoldCardCant(deck) == 1 && SilverCardCant(deck) <= 8)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public int GoldCardCant(Deck deck)
    {
        List<Cards> goldCards = new List<Cards>();
        goldCards = deck.Find(card => card.type == "Gold");
        return goldCards.Count;
    }

    public int SilverCardCant(Deck deck)
    {
        List<Cards> goldCards = new List<Cards>();
        goldCards = deck.Find(card => card.type == "Silver");
        return goldCards.Count;
    }

    public void GeneratePlayers()
    {
        int index = random.Next(0, Store.decks.Count);
        if (!IsAValidDeck(Store.decks[index]))
        {
            int i = index;
            while (i == index)
            {
                i = random.Next(0, Store.decks.Count);
            }
            index = i;
        }
        Deck player1Deck = new Deck(selectedDeck.id, selectedDeck.Name);
        foreach (Cards cards in selectedDeck.cards)
        {
            player1Deck.SendBottom(cards);
        }

        Deck player2Deck = new Deck(Store.decks[index].id, Store.decks[index].Name);
        foreach (Cards cards in Store.decks[index].cards)
        {
            player2Deck.SendBottom(cards);
        }
        player1 = new Player(player1Deck);
        player1.deck.owner = player1;
        player1.graveyard.owner = player1;
        player1.field.owner = player1;

        player2 = new Player(player2Deck);
        player2.deck.owner = player2;
        player2.graveyard.owner = player2;
        player2.field.owner = player2;
        GenerateHands();
        player1.removeCard += RemoveCards;
        player1.updateHand += UpdateHands;
        player2.removeCard += RemoveCards;
        player2.updateHand += UpdateHands;
    }

    public void GenerateHands()
    {
        player1.GenerateHand();
        player1.hand.owner = player1;
        player2.GenerateHand();
        player2.hand.owner = player2;
        Debug.Log($"{player1.hand.cards[0].name} y {player2.hand.cards[0].name}");
        instantiateHands(player1.hand.cards, player2.hand.cards);
    }

    public void PlayCard(Cards card, GameObject panel)
    {
        activePlayer.hand.cards.Remove(card);
        activePlayer.field.SendBottom(card);
        Context.board.SendBottom(card);
        ActiveEffect(card);
        UpdatePoints();
        passTurn();
    }

    private void UpdateHands(Player player)
    {
        updateHand(player);
    }

    private void RemoveCards(Player player, Cards card)
    {
        removeCard(player, card);
    }

    public void ChangesActivePlayer()
    {
        if (IsPlayer1Playing())
        {
            activePlayer = player2;
            Context.triggerPlayer = player2.id;
        }
        else
        {
            activePlayer = player1;
            Context.triggerPlayer = player1.id;
        }
    }

    public bool IsPlayer1Playing()
    {
        return activePlayer == player1 ? true : false;
    }

    private void ActiveEffect(Cards card)
    {
        if (card.onActivation != null)
        {
            for (int i = 0; i < card.onActivation.Count; i++)
            {
                card.onActivation[i].GenerateEffect();
            }
        }
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
                player1.hand.SendBottom(card1);
            }
            else
            {
                player1.graveyard.SendBottom(card1);
            }

            if (player2.hand.cards.Count < 10)
            {
                player2Cards.Add(card2);
                player2.hand.SendBottom(card2);
            }
            else
            {
                player2.graveyard.SendBottom(card2);
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

    private Player FindPlayerWithString(string name)
    {
        if (name == "player1")
        {
            return player1;
        }
        return player2;
    }
}

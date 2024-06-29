using Console;
using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject emptyCard;
    public GameObject player1CardsPrefab;
    public GameObject player2CardsPrefab;
    public GameObject player1HandPanel;
    public GameObject player2HandPanel;
    public GameObject player1SecundaryHand;
    public GameObject player2SecundaryHand;
    Board board;
    public GameObject optionsPanel;

    void Start()
    {
        board = new Board();
        board.noSelectedDeck += ShowOptioptionsPanel;
        board.instantiateHands += InstantiateHands;
        Debug.Log("GameStart");
    }

    public void ShowOptioptionsPanel()
    {
        optionsPanel.SetActive(true);
    }

    public void CreateDeck()
    {
        Guid guid = Guid.NewGuid();
        Store.AddDeck(new Deck(guid, "Nuevo"));
    }

    public void Beginning()
    {
        board.StartGame();
    }

    public void ChangeSelectedDeck(Guid id)
    {
        board.SetSelectedDeck(id);
    }

    public void InstantiateHands(List<Cards> player1Hand, List<Cards> player2Hand)
    {
        for (int i = 0; i < player1Hand.Count; i++)
        {
            GameObject cardObj = Instantiate(player1CardsPrefab, player1HandPanel.transform);
            CardUi cardUi = cardObj.GetComponent<CardUi>();
            cardUi.SetupCard(player1Hand[i]);
            GameObject cardObjEnemy = Instantiate(player2CardsPrefab, player2HandPanel.transform);
            CardUi cardUiEnemy = cardObjEnemy.GetComponent<CardUi>();
            cardUiEnemy.SetupCard(player2Hand[i]);
            GameObject player1SecundaryCard = Instantiate(emptyCard, player1SecundaryHand.transform);
            GameObject player2SecundaryCard = Instantiate(emptyCard, player2SecundaryHand.transform);
        }
    }
}

using Console;
using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    Board board;
    public GameObject optionsPanel;
    #region Vars
    private System.Random random = new System.Random();
    #endregion

    //PLayers
    #region Players
    Player player1;
    Player player2;
    #endregion


    //Delegados
    #region Delegates
    public delegate void SelectDeck(int number);

    #endregion

    //Eventos
    #region Events

    public event SelectDeck selectDeck;


    #endregion

    void Start()
    {
        board = new Board();
        board.noSelectedDeck += ShowOptioptionsPanel;
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
}

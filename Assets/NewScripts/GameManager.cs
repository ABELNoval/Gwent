using Console;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    #region Vars
    private System.Random random = new System.Random();
    #endregion

    //PLayers
    #region Players
    Player player1;
    Player player2;
    #endregion

    //Almacen
    #region Store

    Store store;

    #endregion

    //Delegados
    #region Delegates
    public delegate void Save(Store store);
    public delegate Store Load();
    public delegate void StartGame();
    public delegate void SelectDeck(int number);

    #endregion

    //Eventos
    #region Events

    public event Save save;
    public event Load load;
    public event StartGame start;
    public event SelectDeck selectDeck;

    #endregion

    void Start()
    {
        Debug.Log("GameStart");
        store = load();
        start += GenerateCards;
        selectDeck += AssignedDecks;
    }

    //Salvar el juego
    public void SaveGame()
    {
        save(store);
    }

    public void Beginning()
    {
        player1.GenerateHand();
        player2.GenerateHand();
        Debug.Log(player1.hand[0].name);
        Debug.Log(player2.hand[0].name);
    }

    public void DropDwon(int index)
    {
        selectDeck(index);
    }

    public void AssignedDecks(int number)
    {
        if (number < 0 || number > store.decks.Count)
        {
            throw new System.Exception("Error, tu deck no existe");
        }
        else
        {
            int a = random.Next(0, store.decks.Count);
            player1 = new Player(store.decks[number]);
            player2 = new Player(store.decks[a]);
        }
    }
    public void GenerateCards()
    {
        //Generar prefabs de cartas y ponerlos en las manos
    }
}

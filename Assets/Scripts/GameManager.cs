using Console;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Debug = UnityEngine.Debug;
using UnityEngine.Video;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI player1Points;
    public TextMeshProUGUI player2Points;
    public GameObject finalVideo;
    public GameObject boardObject;
    public GameObject declaration;
    public GameObject player1Camera;
    public GameObject player2Camera;
    public GameObject selectedCard;
    public List<GameObject> boardPanels;
    public GameObject cardsInfoPanel;
    public GameObject emptyCard;
    public GameObject player1CardsPrefab;
    public GameObject player2CardsPrefab;
    public GameObject player1HandPanel;
    public GameObject player2HandPanel;
    public GameObject player1SecundaryHand;
    public GameObject player2SecundaryHand;
    public Game game;
    public GameObject optionsPanel;
    public GameObject errorNotification;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject board;
    public GameObject videoObj;
    public new AudioSource audio;
    [SerializeField] private AudioMixer audioMixer;

    void Start()
    {
        selectedCard = new GameObject();
        game = new Game();
        game.noSelectedDeck += ShowOptioptionsPanel;
        game.instantiateHands += InstantiateHands;
        game.passTurn += PassTurn;
        game.updatePoints += UpdatePoints;
        game.draw += InstantiateHands;
        Debug.Log("GameStart");
    }

    #region MainMenu

    //Cambiar al menu de opciones( OptionsButton )
    public void ChangesToOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    //Comenzar el juego( StartButton )
    public void ChangesToGame()
    {
        videoObj.SetActive(false);
        videoObj.GetComponent<VideoPlayer>().Stop();
        audio.Stop();
        mainMenu.SetActive(false);
        board.SetActive(true);
    }

    //Salir del juego( QuitButton )
    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion

    #region OptionsMenu

    //Ajuste de pantalla( Fullscreen )
    public void FullscreenBool()
    {
        if (Screen.fullScreen == false)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }

    //Ajuste del volumen de la musica
    public void VolumMusic(float volum)
    {
        audioMixer.SetFloat("Volum(music)", volum);
    }

    //Volver al menu principal( BackButton )
    public void ChangesToMainMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    #endregion

    public void ShowOptioptionsPanel()
    {
        ChangesToOptions();
    }

    public void CreateDeck()
    {
        Guid guid = Guid.NewGuid();
        Store.AddDeck(new Deck(guid, "Nuevo"));
    }

    public void StartGame()
    {
        if (game.invalidDeck)
        {
            mainMenu.SetActive(false);
            errorNotification.SetActive(true);
            TextMeshProUGUI text = errorNotification.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Tu deck no cumple con los requisitos para ser usado. Seleccione otro o ajuste el ya seleccionado";
        }
        else
        {
            ChangesToGame();
            game.StartGame();
        }
    }


    public void ChangeSelectedDeck(Guid id)
    {
        game.SetSelectedDeck(id);
    }

    public void InstantiateHands(List<Cards> player1Hand, List<Cards> player2Hand)
    {
        for (int i = 0; i < player1Hand.Count || i < player2Hand.Count; i++)
        {
            if (player1Hand.Count > i)
            {
                GameObject cardObj = Instantiate(player1CardsPrefab, player1HandPanel.transform);
                CardUi cardUi = cardObj.GetComponent<CardUi>();
                cardUi.SetupCard(player1Hand[i]);
                GameObject player1SecundaryCard = Instantiate(emptyCard, player1SecundaryHand.transform);
            }

            if (player2Hand.Count > i)
            {
                GameObject cardObjEnemy = Instantiate(player2CardsPrefab, player2HandPanel.transform);
                CardUi cardUiEnemy = cardObjEnemy.GetComponent<CardUi>();
                cardUiEnemy.SetupCard(player2Hand[i]);
                GameObject player2SecundaryCard = Instantiate(emptyCard, player2SecundaryHand.transform);
            }
        }
    }

    public void GenerateInfo(CardUi cardUi)
    {
        PanelCardInformation panelCardInformation = cardsInfoPanel.GetComponent<PanelCardInformation>();
        panelCardInformation.CreateCardPanelInfo(cardUi.card);
        cardsInfoPanel.SetActive(true);
    }

    public void ClickCard(GameObject card)
    {
        selectedCard = card;
        CardUi cardUi = card.GetComponent<CardUi>();
        if (cardUi.card.range[0] == "Climate")
        {
            Image img = boardPanels[boardPanels.Count - 1].GetComponent<Image>();
            img.sprite = Resources.Load<Sprite>("Art/Images/Panel");
            boardPanels[boardPanels.Count - 1].AddComponent<PanelController>();
        }
        else
        {
            if (game.activePlayer == game.player1)
            {
                for (int i = 0; i < cardUi.card.range.Count; i++)
                {
                    SelectPanels("player1" + cardUi.card.range[i]);
                }
            }
            else
            {
                for (int i = 0; i < cardUi.card.range.Count; i++)
                {
                    SelectPanels("player2" + cardUi.card.range[i]);
                }
            }
        }
    }

    private void SelectPanels(string panelTag)
    {
        foreach (GameObject panel in boardPanels)
        {
            if (panel.tag == panelTag)
            {
                panel.AddComponent<PanelController>();
                Image img = panel.GetComponent<Image>();
                img.sprite = Resources.Load<Sprite>("Art/Images/Panel");
            }
        }
    }

    public void PlayCard(GameObject panel)
    {
        RemoveEmptyCard();
        game.PlayCard(selectedCard.GetComponent<CardUi>().card, panel);
        ChangesCardsConfig(panel);
        selectedCard.transform.SetParent(panel.transform, false);
        DeselectPanels();
    }

    private void DeselectPanels()
    {
        foreach (GameObject panel in boardPanels)
        {
            Destroy(panel.GetComponent<PanelController>());
            Image img = panel.GetComponent<Image>();
            img.sprite = null;
        }
    }

    private void RemoveEmptyCard()
    {
        if (game.IsPlayer1Playing())
        {
            Destroy(player1SecundaryHand.transform.GetChild(0).gameObject);
        }
        else
        {
            Destroy(player2SecundaryHand.transform.GetChild(0).gameObject);
        }
    }

    public void ChangesCardsConfig(GameObject panel)
    {
        int cantCard = 1;
        if (panel.tag != "player1Buff" && panel.tag != "player2Buff")
        {
            cantCard = (panel.tag == "Climate") ? 4 : 11;
        }
        Image[] images = selectedCard.GetComponentsInChildren<Image>();
        TextMeshProUGUI name = selectedCard.GetComponentInChildren<TextMeshProUGUI>();
        RectTransform rectTransformLayout = panel.GetComponent<RectTransform>();
        RectTransform rectTransform = selectedCard.GetComponent<RectTransform>();
        BoxCollider boxCollider = selectedCard.GetComponent<BoxCollider>();
        float width = rectTransformLayout.rect.width / cantCard;
        float height = rectTransformLayout.rect.height;
        Vector2 cardSize = rectTransform.sizeDelta;
        foreach (Image image in images)
        {
            image.enabled = false;
        }
        name.enabled = false;
        cardSize.x = width;
        cardSize.y = height;
        rectTransform.sizeDelta = cardSize;
        boxCollider.size = new Vector3(cardSize.x, cardSize.y, boxCollider.size.z);
    }

    private void PassTurn()
    {
        //selectedCard = null;
        if (game.player1.isPlaying && game.player2.isPlaying)
        {
            game.ChangesActivePlayer();
            Debug.Log("PassTurn");
            player1Camera.SetActive(game.activePlayer == game.player1);
            player2Camera.SetActive(game.activePlayer == game.player2);
            player1HandPanel.SetActive(game.activePlayer == game.player1);
            player2HandPanel.SetActive(game.activePlayer == game.player2);
            player1SecundaryHand.SetActive(game.activePlayer == game.player2);
            player2SecundaryHand.SetActive(game.activePlayer == game.player1);
        }
    }

    private void UpdatePoints(int player1Points, int player2Points)
    {
        this.player1Points.text = "Player1: " + player1Points.ToString();
        this.player2Points.text = "Player2: " + player2Points.ToString();
    }

    public void StopPlaying()
    {
        if (!game.player1.isPlaying || !game.player2.isPlaying)
        {
            game.ChangesActivePlayer();
            game.StopPlaying();
            FinishRound();
            if (game.player1Wins < 2 && game.player2Wins < 2)
            {
                StartNewRound();
                return;
            }
            FinishGame();
            return;
        }
        PassTurn();
        game.StopPlaying();
    }

    private void FinishRound()
    {
        StartCoroutine(GetTheWinnerOfTheRound());
        game.RefrechBoard();
        ClearBoard();
    }

    private void FinishGame()
    {
        ClearBoard();
        StartCoroutine(GetTheWinnerOfTheGame());
        boardObject.SetActive(false);
        finalVideo.SetActive(true);
    }

    private void ClearBoard()
    {
        foreach (GameObject panel in boardPanels)
        {
            for (int i = panel.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(panel.transform.GetChild(i).gameObject);
            }
        }
    }

    private IEnumerator GetTheWinnerOfTheRound()
    {
        declaration.SetActive(true);
        if (game.player1Points > game.player2Points)
        {
            declaration.GetComponentInChildren<TextMeshProUGUI>().text = "Player1 won the round";
            game.player1Wins++;
            game.activePlayer = game.player2;
        }
        else
        {
            if (game.player1Points != game.player2Points)
            {
                declaration.GetComponentInChildren<TextMeshProUGUI>().text = "Player2 won the round";
                game.player2Wins++;
                game.activePlayer = game.player1;
            }
            else
            {
                declaration.GetComponentInChildren<TextMeshProUGUI>().text = "Draw";
                game.player1Wins++;
                game.player2Wins++;
            }
        }
        yield return new WaitForSeconds(2f);
        declaration.SetActive(false);
    }

    private IEnumerator GetTheWinnerOfTheGame()
    {
        declaration.SetActive(true);
        if (game.player1Wins > game.player2Wins)
        {
            declaration.GetComponentInChildren<TextMeshProUGUI>().text = "Player1 won the game";
        }
        else
        {
            declaration.GetComponentInChildren<TextMeshProUGUI>().text = "Player2 won the game";
        }
        yield return new WaitForSeconds(2f);
        declaration.SetActive(false);
    }

    public void StartNewRound()
    {
        game.player1.isPlaying = true;
        game.player2.isPlaying = true;
        //game.DrawCards(2);
        PassTurn();
    }
}

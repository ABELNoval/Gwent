using System.Diagnostics;
using Console;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI player1Points;
    public TextMeshProUGUI player2Points;
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

    void Start()
    {
        selectedCard = new GameObject();
        game = new Game();
        game.noSelectedDeck += ShowOptioptionsPanel;
        game.instantiateHands += InstantiateHands;
        game.passTurn += PassTurn;
        game.updatePoints += UpdatePoints;
        UnityEngine.Debug.Log("GameStart");
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
        game.StartGame();
    }

    public void ChangeSelectedDeck(Guid id)
    {
        game.SetSelectedDeck(id);
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
        if (!game.IsPlayer1Playing())
        {
            player1Camera.SetActive(false);
            player1HandPanel.SetActive(false);
            player1SecundaryHand.SetActive(true);
            player2Camera.SetActive(true);
            player2HandPanel.SetActive(true);
            player2SecundaryHand.SetActive(false);
        }
        else
        {
            player1Camera.SetActive(true);
            player1HandPanel.SetActive(true);
            player1SecundaryHand.SetActive(false);
            player2Camera.SetActive(false);
            player2HandPanel.SetActive(false);
            player2SecundaryHand.SetActive(true);
        }
    }

    private void UpdatePoints(int player1Points, int player2Points)
    {
        this.player1Points.text = player1Points.ToString();
        this.player2Points.text = player2Points.ToString();
    }
}

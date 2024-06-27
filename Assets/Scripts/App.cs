using UnityEngine;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Video;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class App : MonoBehaviour
{
    public GameObject audioCards;
    public GameObject player1BossPanel;
    public GameObject player2BossPanel;
    public GameObject player1BossCard;
    public GameObject player2BossCard;
    public GameObject passPanel;
    public GameObject video;
    public GameObject audioButton;
    public GameObject panelAfterTheGame;
    public List<GameObject> gamePanelsInBoard;
    public List<GameObject> gamePanelsInHands;
    public GameObject deselectCard;
    private GameObject selectedCard;
    public GameObject panelInfo;
    public GameObject bossCardPanelInfo;
    public GameObject player1HandSecundaryPanel;
    public GameObject player2HandSecundaryPanel;
    public GameObject player1MelePanel;
    public GameObject player1RangePanel;
    public GameObject player1SiegePanel;
    public GameObject player1Buff1Panel;
    public GameObject player1Buff2Panel;
    public GameObject player1Buff3Panel;
    public GameObject player1CementaryPanel;
    public GameObject player2MelePanel;
    public GameObject player2RangePanel;
    public GameObject player2SiegePanel;
    public GameObject player2Buff1Panel;
    public GameObject player2Buff2Panel;
    public GameObject player2Buff3Panel;
    public GameObject player2CementaryPanel;
    public GameObject expansionPanel;
    public GameObject player1Camera;
    public GameObject player2Camera;
    public GameObject player1Cards;
    public GameObject player2Cards;
    public GameObject player1HandPanel;
    public GameObject player2HandPanel;
    public GameObject emptyCard;
    public TextMeshProUGUI player1Points;
    public TextMeshProUGUI player2Points;
    public GameObject winnerPanel;
    public TextMeshProUGUI winnerText;
    public List<GameObject> activePanels;
    public Game game;

    //Ejecutando el juego
    public void Start()
    {
        game = new Game();
        Debug.Log("GameStart");
        gamePanelsInBoard.Add(player1MelePanel);
        gamePanelsInBoard.Add(player1RangePanel);
        gamePanelsInBoard.Add(player1SiegePanel);
        gamePanelsInBoard.Add(player1Buff1Panel);
        gamePanelsInBoard.Add(player1Buff2Panel);
        gamePanelsInBoard.Add(player1Buff3Panel);
        gamePanelsInBoard.Add(player2MelePanel);
        gamePanelsInBoard.Add(player2RangePanel);
        gamePanelsInBoard.Add(player2SiegePanel);
        gamePanelsInBoard.Add(player2Buff1Panel);
        gamePanelsInBoard.Add(player2Buff2Panel);
        gamePanelsInBoard.Add(player2Buff3Panel);
        gamePanelsInBoard.Add(expansionPanel);
        gamePanelsInHands.Add(player1HandPanel);
        gamePanelsInHands.Add(player2HandPanel);
        gamePanelsInHands.Add(player1HandSecundaryPanel);
        gamePanelsInHands.Add(player2HandSecundaryPanel);
    }

    //Cambio de deck en opciones(Pediente)
    public void ChangeDeck()
    {
        game.ChangePlayerDeck();
    }

    //Comienza el juego
    public void Play()
    {
        player1Camera.SetActive(game.activePlayer == game.player1);
        player2Camera.SetActive(game.activePlayer == game.player2);
        GenerateBossCard();
        StartCoroutine(GenerateHands(10));
    }

    //Generar las cartas
    public IEnumerator GenerateHands(int cantCard)
    {
        RectTransform player1PanelTransform = player1HandPanel.GetComponent<RectTransform>();
        RectTransform player2PanelTransform = player2HandPanel.GetComponent<RectTransform>();
        RectTransform player1RectTransform = player1HandSecundaryPanel.GetComponent<RectTransform>();
        RectTransform player2RectTransform = player2HandSecundaryPanel.GetComponent<RectTransform>();
        for (int i = 0; i < cantCard; i++)
        {
            GameObject cardObj = Instantiate(player1Cards, player1PanelTransform);
            CardUi player1Card = cardObj.GetComponent<CardUi>();
            player1Card.SetupCard(game.player1.hand[i]);
            GameObject cardObjEnemy = Instantiate(player2Cards, player2PanelTransform);
            CardUi player2Card = cardObjEnemy.GetComponent<CardUi>();
            player2Card.SetupCard(game.player2.hand[i]);
            GameObject player1SecundaryCard = Instantiate(emptyCard, player1RectTransform);
            GameObject player2SecundaryCard = Instantiate(emptyCard, player2RectTransform);
            //AudioSource audio = audioCards.GetComponent<AudioSource>();
            //audio.Play();
            yield return new WaitForSeconds(0.5f);
        }
    }

    //Generar la carta lider
    private void GenerateBossCard()
    {
        GameObject player1CardObj = Instantiate(player1BossCard, player1BossPanel.transform);
        GameObject player2CardObj = Instantiate(player2BossCard, player2BossPanel.transform);
        player1CardObj.GetComponent<BossCardmanager>().SetupCard(game.sorcererDeck.CreateBossCard());
        player2CardObj.GetComponent<BossCardmanager>().SetupCard(game.sorcererDeck.CreateBossCard());
    }

    //Hay alguna carta seleccionada?
    public bool IsCardSelected()
    {
        return selectedCard != null;
    }

    //Seleccionar la carta que se va a jugar
    public void SelectCardToPlay(GameObject cardObj)
    {
        selectedCard = cardObj;
        deselectCard.SetActive(true);
        CardUi cardUi = selectedCard.GetComponent<CardUi>();
        SelectPanelToPlay(cardUi.card);
    }

    //Marcar los paneles donde se puede jugar la carta
    public void SelectPanelToPlay(Card card)
    {
        SelectMelePaneltoPlay(card);
        SelectRangePaneltoPlay(card);
        SelectSiegePaneltoPlay(card);
        SelectExpansionPaneltoPlay(card);
        SelectBuffPaneltoPlay(card);
    }

    //Marcar el panel mele
    public void SelectMelePaneltoPlay(Card card)
    {
        if (card.IsMele())
        {
            GameObject melePanel = (game.activePlayer == game.player1) ? player1MelePanel : player2MelePanel;
            GenerateImageToPanel(melePanel);
            activePanels.Add(melePanel);
        }
    }

    //Marcar el panel range
    public void SelectRangePaneltoPlay(Card card)
    {
        if (card.IsRange())
        {
            GameObject rangePanel = (game.activePlayer == game.player1) ? player1RangePanel : player2RangePanel;
            GenerateImageToPanel(rangePanel);
            activePanels.Add(rangePanel);
        }
    }

    //Marcar el panel siege
    public void SelectSiegePaneltoPlay(Card card)
    {
        if (card.IsSiege())
        {
            GameObject siegePanel = (game.activePlayer == game.player1) ? player1SiegePanel : player2SiegePanel;
            GenerateImageToPanel(siegePanel);
            activePanels.Add(siegePanel);
        }
    }

    //Marcar el panel expansion
    public void SelectExpansionPaneltoPlay(Card card)
    {
        if (card.IsExpansion() && expansionPanel.transform.childCount < 3)
        {
            GenerateImageToPanel(expansionPanel);
            activePanels.Add(expansionPanel);
        }
    }

    //Marcar los paneles buff
    public void SelectBuffPaneltoPlay(Card card)
    {
        if (card.IsBuff())
        {
            GameObject buffPanel1 = (game.activePlayer == game.player1) ? player1Buff1Panel : player2Buff1Panel;
            GameObject buffPanel2 = (game.activePlayer == game.player1) ? player1Buff2Panel : player2Buff2Panel;
            GameObject buffPanel3 = (game.activePlayer == game.player1) ? player1Buff3Panel : player2Buff3Panel;
            if (buffPanel1.transform.childCount == 0)
            {
                GenerateImageToPanel(buffPanel1);
                activePanels.Add(buffPanel1);
            }

            if (buffPanel2.transform.childCount == 0)
            {
                GenerateImageToPanel(buffPanel2);
                activePanels.Add(buffPanel2);
            }

            if (buffPanel3.transform.childCount == 0)
            {
                GenerateImageToPanel(buffPanel3);
                activePanels.Add(buffPanel3);
            }
        }
    }

    //Generar imagen del panel en que se jugara
    public void GenerateImageToPanel(GameObject panel)
    {
        Image image = panel.GetComponentInChildren<Image>();
        image.sprite = Resources.Load<Sprite>("Art/Images/Panel");
        panel.AddComponent<PanelController>();
    }

    //Deseleccionar los paneles
    public void DeselectPanels()
    {
        while (activePanels.Count > 0)
        {
            Image image = activePanels[0].GetComponentInChildren<Image>();
            Destroy(activePanels[0].GetComponent<PanelController>());
            image.sprite = null;
            activePanels.Remove(activePanels[0]);
        }
    }

    //Deseleccionar el panel
    public void DeselectCard()
    {
        selectedCard = null;
        deselectCard.SetActive(false);
        DeselectPanels();
    }

    //Jugar una carta
    public void PlayCardUI(GameObject panel)
    {
        Debug.Log("PlayCardUI");
        CardUi cardUi = selectedCard.GetComponent<CardUi>();
        if (Enum.TryParse(panel.tag, out BoardPosition boardPosition))
        {
            RectTransform panelTransform = panel.GetComponent<RectTransform>();
            selectedCard.transform.SetParent(panelTransform, false);
            ChangesCardsConfig(panel, selectedCard);
            DeselectPanels();
            deselectCard.SetActive(false);
            game.Play(cardUi.id, boardPosition);
            if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects().Count > 0)
            {
                ActiveEffect(selectedCard, panel);
                return;
            }
            UpdatePoints();
            PassTurn();
        }
    }

    //Remover una carta de la mano rival durante tu turno
    private void RemoveEmptyCard()
    {
        if (game.activePlayer != game.player1)
        {
            Destroy(player1HandSecundaryPanel.transform.GetChild(0).gameObject);

            return;
        }
        Destroy(player2HandSecundaryPanel.transform.GetChild(0).gameObject);
    }

    //Adaptar carta a los paneles
    public void ChangesCardsConfig(GameObject panel, GameObject card)
    {
        int cantCard = 1;
        if (panel.tag != "BuffMele" && panel.tag != "BuffRange" && panel.tag != "BuffSiege")
        {
            cantCard = (panel.tag == "Expansion") ? 4 : 11;
        }
        //HorizontalLayoutGroup panelLayout = panel.GetComponent<HorizontalLayoutGroup>();
        Image[] images = card.GetComponentsInChildren<Image>();
        TextMeshProUGUI name = card.GetComponentInChildren<TextMeshProUGUI>();
        RectTransform rectTransformLayout = panel.GetComponent<RectTransform>();
        RectTransform rectTransform = card.GetComponent<RectTransform>();
        BoxCollider boxCollider = card.GetComponent<BoxCollider>();
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

    //Mostrar panel con la informacion de las cartas
    public void ViewCardInfo(Card card)
    {
        PanelCardInformation panelCardInfo = panelInfo.GetComponent<PanelCardInformation>();
        List<string> cardEffects = card.GetEffects();
        int power = game.GetCardPower(card.id);
        panelCardInfo.CreateCardPanelInfo(card.nameBase, card.artworkBase, card.descriptionBase, card.cardPosBase, power, card.typeOfCard, cardEffects);
        panelInfo.SetActive(true);
    }

    //Cambio de camara y de jugador actual
    public void PassTurn()
    {
        selectedCard = null;
        if (game.player1.isPlaying && game.player2.isPlaying)
        {
            game.ChangesActivePLayer();
            RemoveEmptyCard();
            Debug.Log("PassTurn");
            player1Camera.SetActive(game.activePlayer == game.player1);
            player2Camera.SetActive(game.activePlayer == game.player2);
            player1HandPanel.SetActive(game.activePlayer == game.player1);
            player2HandPanel.SetActive(game.activePlayer == game.player2);
            player1HandSecundaryPanel.SetActive(game.activePlayer == game.player2);
            player2HandSecundaryPanel.SetActive(game.activePlayer == game.player1);
        }
    }

    //Dejar de jugar en esa ronda
    public void StopPlayForThisRound()
    {
        DeselectCard();
        if (!game.player1.isPlaying || !game.player2.isPlaying)
        {
            game.StopPlaying();
            StartCoroutine(FinishRound());
            if (game.player1Wins < 2 && game.player2Wins < 2)
            {
                StartNewRound();
                return;
            }
            StartCoroutine(FinishGame());
            return;
        }
        PassTurn();
        game.StopPlaying();
    }

    //Finalizar la ronda
    public IEnumerator FinishRound()
    {
        winnerPanel.SetActive(true);
        winnerText.text = game.GetTheWinnerOfTheRound();
        RefrechBoard();
        player1Points.text = game.GetPointsInPlayer1Board().ToString();
        player2Points.text = game.GetPointsInPlayer2Board().ToString();
        yield return new WaitForSeconds(2f);
        winnerPanel.SetActive(false);
    }

    //Actualizar el tablero para una nueva ronda
    public void RefrechBoard()
    {
        game.RefrechBoard();
        RemoveAllCardUi();
    }

    //Remover las cartas del campo
    public void RemoveAllCardUi()
    {
        foreach (GameObject panel in gamePanelsInBoard)
        {
            for (int i = panel.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(panel.transform.GetChild(i).gameObject);
            }
        }
    }

    //Remover las cartas de las manos
    public void RemoveHandCards()
    {
        foreach (GameObject panel in gamePanelsInHands)
        {
            for (int i = panel.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(panel.transform.GetChild(i).gameObject);
            }
        }
    }

    //Empezar la nueva ronda
    public void StartNewRound()
    {
        game.player1.isPlaying = true;
        game.player2.isPlaying = true;
        game.activePlayer = game.winnerOfTheCurrentRound == game.player1 ? game.player2 : game.player1;
        PassTurn();
        GenerateTwoCards();
    }

    //Robar dos cartas
    public void GenerateTwoCards()
    {
        StartCoroutine(GenerateHands(2));
        StartCoroutine(DeleteInecesaryCards());
    }

    //Eliminar cartas sobrantes en las manos
    public IEnumerator DeleteInecesaryCards()
    {
        int i = 1;
        yield return new WaitForSeconds(1.0f);
        while (game.player1.hand.Count > 10)
        {
            game.player1Board.AddCementeryCard(game.player1.hand[0]);
            game.player1.hand.RemoveAt(0);
            Destroy(player1HandPanel.transform.GetChild(player1HandPanel.transform.childCount - i).gameObject);
            Destroy(player1HandSecundaryPanel.transform.GetChild(i).gameObject);
            i++;
        }
        while (game.player2.hand.Count > 10)
        {
            game.player2Board.AddCementeryCard(game.player2.hand[0]);
            Destroy(player2HandPanel.transform.GetChild(player2HandPanel.transform.childCount - i).gameObject);
            Destroy(player2HandSecundaryPanel.transform.GetChild(i - 1).gameObject);
            game.player2.hand.RemoveAt(0);
            i++;
        }
    }

    //Terminar el juego
    public IEnumerator FinishGame()
    {
        winnerPanel.SetActive(true);
        winnerText.text = game.player1Wins > game.player2Wins ? "Player1 wins the game" : "Player2 wins the game";
        RemoveAllCardUi();
        RemoveHandCards();
        yield return new WaitForSeconds(3f);
        winnerPanel.SetActive(false);
        panelInfo.SetActive(false);
        passPanel.SetActive(false);
        player1Camera.SetActive(true);
        video.SetActive(true);
        VideoPlayer videoPlayer = video.GetComponent<VideoPlayer>();
        videoPlayer.Play();
        yield return new WaitForSeconds(31f);
        panelAfterTheGame.SetActive(true);
    }

    //Actualizar los puntos
    public void UpdatePoints()
    {
        player1Points.text = "Player1:" + game.player1Board.GetBoardPoints();
        player2Points.text = "Player2:" + game.player2Board.GetBoardPoints();
    }

    //Activar efectos
    public void ActiveEffect(GameObject cardUi, GameObject panel)
    {
        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Gojo)
        {
            UpdatePoints();
            PassTurn();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Mahito)
        {
            Card card = game.ActiveMahitoEffect();
            if (game.activePlayer == game.player1)
            {
                GameObject player1Card = Instantiate(player1Cards, player1HandPanel.transform);
                CardUi player1CardUi = player1Card.GetComponent<CardUi>();
                player1CardUi.SetupCard(card);
                Instantiate(emptyCard, player1HandSecundaryPanel.transform);
                return;
            }
            GameObject player2Card = Instantiate(player2Cards, player2HandPanel.transform);
            CardUi player2CardUi = player2Card.GetComponent<CardUi>();
            player2CardUi.SetupCard(card);
            Instantiate(emptyCard, player2HandSecundaryPanel.transform);
            UpdatePoints();
            PassTurn();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Sukuna)
        {
            UpdatePoints();
            PassTurn();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Higuruma)
        {
            Card afectedCard = game.ActiveHigurumaEffect();
            RemoveCardUi(afectedCard);
            UpdatePoints();
            PassTurn();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Hakari)
        {
            System.Random a = new();
            game.ActiveHakariEffect(a.Next(1, 6), cardUi.GetComponent<CardUi>().card);
            UpdatePoints();
            PassTurn();
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Kashimo)
        {
            List<Card> afectedCards = game.ActiveKashimoEffect();
            foreach (Card card in afectedCards)
            {
                RemoveCardUi(card);
            }
            UpdatePoints();
            PassTurn();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Yuta)
        {
            EffectPosition effectPosition = GetEffectPosition(panel);
            game.ActiveYutaEffect(effectPosition);
            UpdatePoints();
            PassTurn();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Kenjaku)
        {
            game.ActiveKenjakuEffect(cardUi.GetComponent<CardUi>().card);
            UpdatePoints();
            PassTurn();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionGojo)
        {
            UpdatePoints();
            PassTurn();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionHakari)
        {
            SelectPanelsToApplyEffect();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionJogo)
        {
            SelectPanelsToApplyEffect();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionYuta)
        {
            SelectPanelsToApplyEffect();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionSukuna)
        {
            SelectPanelsToApplyEffect();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionMahito)
        {
            SelectPanelsToApplyEffect();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Buff)
        {
            game.ActiveBuffEffect(GetEffectPosition(panel));
            UpdatePoints();
            PassTurn();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Despeje)
        {
            game.DesactiveExpansionEffect();
            DestroyExpansionsCards();
            UpdatePoints();
            PassTurn();
            return;
        }

        if (cardUi.GetComponent<CardUi>().card.GetTypeOfEffects()[0] == TypeOfEffects.Lure)
        {
            int i = 0;
            while (i < panel.transform.childCount)
            {
                CardUi cardObj = panel.transform.GetChild(i).GetComponent<CardUi>();
                if (cardObj.card.typeOfCard != TypeOfCard.GoldCard)
                {
                    panel.transform.GetChild(i).AddComponent<CardIsSelectedWithAnEffect>();
                }
                i++;
            }
        }
    }

    public void ActiveBossEffect(Card card)
    {
        game.AddEffect(card.GetTypeOfEffects()[0]);
        PassTurn();
    }

    public void ApplyLureEffect(GameObject cardOnBoard)
    {
        Debug.Log("Lure");
        Card card = cardOnBoard.GetComponent<CardUi>().card;
        card.currentPower = card.powerBase;
        game.ActiveBuggiBuggiEffect(card);
        RemoveTheCardIsSelectedWithAnEffect();
        GameObject panel = game.activePlayer == game.player1 ? player1HandPanel : player2HandPanel;
        cardOnBoard.transform.SetParent(panel.transform, false);
        ChangesCardsConfig(panel, cardOnBoard);
        Instantiate(emptyCard, player1HandSecundaryPanel.transform);
        UpdatePoints();
        PassTurn();

    }

    //Marcar los paneles rivales que se pueden afectar
    public void GenerateImageInRivalPanel(GameObject panel)
    {
        Image image = panel.GetComponentInChildren<Image>();
        image.sprite = Resources.Load<Sprite>("Art/Images/Panel");
        //panel.AddComponent<PanelRivalController>();
    }

    public void AudioPlay()
    {
        AudioSource audioSource = audioButton.GetComponent<AudioSource>();
        audioSource.Play();
    }

    private void RemoveTheCardIsSelectedWithAnEffect()
    {
        RemoveTheCardIsSelectedWithAnEffectInPanel(player1MelePanel);
        RemoveTheCardIsSelectedWithAnEffectInPanel(player1RangePanel);
        RemoveTheCardIsSelectedWithAnEffectInPanel(player1SiegePanel);
        RemoveTheCardIsSelectedWithAnEffectInPanel(player2MelePanel);
        RemoveTheCardIsSelectedWithAnEffectInPanel(player2RangePanel);
        RemoveTheCardIsSelectedWithAnEffectInPanel(player2SiegePanel);
    }

    private void RemoveTheCardIsSelectedWithAnEffectInPanel(GameObject panel)
    {
        int i = 0;
        while (i < panel.transform.childCount)
        {
            CardIsSelectedWithAnEffect cardIsSelectedWithAnEffect = panel.transform.GetChild(i).GetComponent<CardIsSelectedWithAnEffect>();
            if (cardIsSelectedWithAnEffect != null)
            {
                Destroy(cardIsSelectedWithAnEffect);
            }
            i++;
        }
    }

    private EffectPosition GetEffectPosition(GameObject panel)
    {
        if (panel.CompareTag("BuffMele") || panel.CompareTag("Mele"))
        {
            return EffectPosition.Mele;
        }

        if (panel.CompareTag("BuffRange") || panel.CompareTag("Range"))
        {
            return EffectPosition.Range;
        }
        return EffectPosition.Siege;
    }

    private void RemoveCardUi(Card card)
    {
        RemoveCardUiInPanel(card, player1MelePanel);
        RemoveCardUiInPanel(card, player1RangePanel);
        RemoveCardUiInPanel(card, player1SiegePanel);
        RemoveCardUiInPanel(card, player2MelePanel);
        RemoveCardUiInPanel(card, player2RangePanel);
        RemoveCardUiInPanel(card, player2SiegePanel);
    }

    private void RemoveCardUiInPanel(Card card, GameObject panel)
    {
        if (card != null)
        {
            foreach (Transform child in panel.transform)
            {
                CardUi cardUi = child.GetComponent<CardUi>();
                if (cardUi.card.id == card.id)
                {
                    Destroy(child.gameObject);
                    Destroy(cardUi.gameObject);
                    return;
                }
            }
        }

    }

    private void SelectPanelsToApplyEffect()
    {
        Image player1MeleImage = player1MelePanel.GetComponentInChildren<Image>();
        player1MeleImage.sprite = Resources.Load<Sprite>("Art/Images/Panel");
        Image player1RangeImage = player1RangePanel.GetComponentInChildren<Image>();
        player1RangeImage.sprite = Resources.Load<Sprite>("Art/Images/Panel");
        Image player1SiegeImage = player1SiegePanel.GetComponentInChildren<Image>();
        player1SiegeImage.sprite = Resources.Load<Sprite>("Art/Images/Panel");
        Image player2MeleImage = player2MelePanel.GetComponentInChildren<Image>();
        player2MeleImage.sprite = Resources.Load<Sprite>("Art/Images/Panel");
        Image player2RangeImage = player2RangePanel.GetComponentInChildren<Image>();
        player2RangeImage.sprite = Resources.Load<Sprite>("Art/Images/Panel");
        Image player2SiegeImage = player2SiegePanel.GetComponentInChildren<Image>();
        player2SiegeImage.sprite = Resources.Load<Sprite>("Art/Images/Panel");

        player1MelePanel.AddComponent<ExpansionEffect>();
        player1RangePanel.AddComponent<ExpansionEffect>();
        player1SiegePanel.AddComponent<ExpansionEffect>();
        player2MelePanel.AddComponent<ExpansionEffect>();
        player2RangePanel.AddComponent<ExpansionEffect>();
        player2SiegePanel.AddComponent<ExpansionEffect>();
    }

    public void ActiveExpansionEffect(GameObject panel)
    {
        Image player1MeleImage = player1MelePanel.GetComponentInChildren<Image>();
        player1MeleImage.sprite = null;
        Image player1RangeImage = player1RangePanel.GetComponentInChildren<Image>();
        player1RangeImage.sprite = null;
        Image player1SiegeImage = player1SiegePanel.GetComponentInChildren<Image>();
        player1SiegeImage.sprite = null;
        Image player2MeleImage = player2MelePanel.GetComponentInChildren<Image>();
        player2MeleImage.sprite = null;
        Image player2RangeImage = player2RangePanel.GetComponentInChildren<Image>();
        player2RangeImage.sprite = null;
        Image player2SiegeImage = player2SiegePanel.GetComponentInChildren<Image>();
        player2SiegeImage.sprite = null;

        Destroy(player1MelePanel.GetComponent<ExpansionEffect>());
        Destroy(player1RangePanel.GetComponent<ExpansionEffect>());
        Destroy(player1SiegePanel.GetComponent<ExpansionEffect>());
        Destroy(player2MelePanel.GetComponent<ExpansionEffect>());
        Destroy(player2RangePanel.GetComponent<ExpansionEffect>());
        Destroy(player2SiegePanel.GetComponent<ExpansionEffect>());

        EffectPosition effectPosition = GetEffectPosition(panel);
        if (selectedCard == null)
        {
            Debug.Log("Hola");
        }
        CardUi cardUi = selectedCard.GetComponent<CardUi>();
        if (cardUi.card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionHakari)
        {
            game.ActiveExpansionHakariEffect(effectPosition);
            UpdatePoints();
            PassTurn();
            return;
        }
        if (cardUi.card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionMahito)
        {
            game.ActiveExpansionMahitoEffect(effectPosition);
            UpdatePoints();
            PassTurn();
            return;
        }
        if (cardUi.card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionYuta)
        {
            game.ActiveExpansionYutaEffect(effectPosition);
            UpdatePoints();
            PassTurn();
            return;
        }
        if (cardUi.card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionSukuna)
        {
            game.ActiveExpansionSukunaEffect(effectPosition);
            UpdatePoints();
            PassTurn();
            return;
        }
        if (cardUi.card.GetTypeOfEffects()[0] == TypeOfEffects.ExpansionJogo)
        {
            game.ActiveExpansionJogoEffect(effectPosition);
            UpdatePoints();
            PassTurn();
            return;
        }
    }

    private void DestroyExpansionsCards()
    {
        foreach (Transform child in expansionPanel.transform)
        {
            // Destruir cada hijo
            Destroy(child.gameObject);
        }
    }
}
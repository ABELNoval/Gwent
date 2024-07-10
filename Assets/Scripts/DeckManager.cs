using TMPro;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    TextMeshProUGUI cantCards = new();
    TextMeshProUGUI cantGoldCards = new();
    TextMeshProUGUI cantSilverCards = new();

    public DeckManager(int cantCards, int cantGoldCards, int cantSilverCards, GameObject createDeck)
    {
        this.cantCards = createDeck.transform.GetChild(0).Find("Cards").GetComponent<TextMeshProUGUI>();
        this.cantGoldCards = createDeck.transform.GetChild(0).Find("GoldCards").GetComponent<TextMeshProUGUI>();
        this.cantSilverCards = createDeck.transform.GetChild(0).Find("SilverCards").GetComponent<TextMeshProUGUI>();

        this.cantCards.text = "Cards: " + cantCards.ToString() + " / 25";
        this.cantGoldCards.text = "GoldCards: " + cantGoldCards.ToString() + " / 1";
        this.cantSilverCards.text = "SilverCards: " + cantSilverCards.ToString() + " / 8";
    }
}

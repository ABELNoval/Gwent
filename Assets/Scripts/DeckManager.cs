using TMPro;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    TextMeshProUGUI cantCards = new();
    TextMeshProUGUI cantGoldCards = new();
    TextMeshProUGUI cantSilverCards = new();
    TextMeshProUGUI deckName = new();

    public DeckManager(int cantCards, int cantGoldCards, int cantSilverCards, GameObject createDeck, string deckName)
    {
        this.deckName = createDeck.transform.GetChild(0).Find("InputField").transform.GetChild(0).Find("Placeholder").GetComponent<TextMeshProUGUI>();
        this.cantCards = createDeck.transform.GetChild(0).Find("Cards").GetComponent<TextMeshProUGUI>();
        this.cantGoldCards = createDeck.transform.GetChild(0).Find("GoldCards").GetComponent<TextMeshProUGUI>();
        this.cantSilverCards = createDeck.transform.GetChild(0).Find("SilverCards").GetComponent<TextMeshProUGUI>();

        this.cantCards.text = "Cards: " + cantCards.ToString() + " / 25";
        this.cantGoldCards.text = "GoldCards: " + cantGoldCards.ToString() + " / 1";
        this.cantSilverCards.text = "SilverCards: " + cantSilverCards.ToString() + " / 8";
        this.deckName.text = deckName;
    }
}

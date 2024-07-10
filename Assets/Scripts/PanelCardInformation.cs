using Console;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelCardInformation : MonoBehaviour
{
    public TextMeshProUGUI cardName;
    public Image cardImage;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI cardPos;
    public TextMeshProUGUI cardPower;
    public TextMeshProUGUI effect;
    public Cards card;

    public void CreateCardPanelInfo(Cards card)
    {
        this.card = card;
        cardPos.text = Pos();
        cardName.text = card.name;
        cardImage.sprite = Resources.Load<Sprite>(card.img);
        cardDescription.text = card.description;
        cardPower.text = card.attack.ToString();
        effect.text = Effect();
    }

    private string Pos()
    {
        string position = "Position: ";
        foreach (string pos in card.range)
        {
            position += pos + " ";
        }
        return position;
    }

    private string Effect()
    {
        string effects = "Effects: ";
        /*foreach (string eff in card.effectsName)
        {
            effects += eff + " ";
        }*/
        return effects;
    }
}
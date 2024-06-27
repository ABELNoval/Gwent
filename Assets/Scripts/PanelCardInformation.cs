using System.Collections.Generic;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;
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

    public void CreateCardPanelInfo(string cardName, Sprite cardImage, string cardDescription, CardPos cardPos, int cardPower, TypeOfCard typeOfCard, List<string> effects)
    {
        this.cardPos.text = cardPos.ToString();
        this.cardName.text = cardName;
        this.cardImage.sprite = cardImage;
        this.cardDescription.text = cardDescription;
        this.cardPower.text = cardPower.ToString();
        this.effect.text = effects.Count > 0 ? effects[0] : null;
    }
}
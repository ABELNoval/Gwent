using System;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUi : MonoBehaviour
{
    public Guid id;
    public TextMeshProUGUI title;
    public RawImage image;
    public Card card;

    public void SetupCard(Card card)
    {
        this.card = card;
        this.id = card.id;
        this.title.text = card.nameBase;
        this.image.texture = card.artworkBase.texture;
    }
}

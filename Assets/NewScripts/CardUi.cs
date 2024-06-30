using System;
using Console;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUi : MonoBehaviour
{
    public Guid id;
    public TextMeshProUGUI title;
    public RawImage image;
    public Cards card;

    public void SetupCard(Cards card)
    {
        this.card = card;
        title.text = card.name;
        image.texture = Resources.Load<Sprite>(card.img).texture;
    }
}

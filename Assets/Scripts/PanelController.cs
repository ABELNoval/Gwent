using UnityEngine;

public class PanelController : MonoBehaviour
{
    private GameManager gameManager;

    //Inicia el juego
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    //Click sobre un panel
    private void OnMouseDown()
    {
        gameManager.PlayCard(gameObject);
    }
}

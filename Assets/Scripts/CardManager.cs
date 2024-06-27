using UnityEngine;

public class CardManager : MonoBehaviour
{
    private App app;


    //Inicio del juego
    private void Start()
    {
        app = FindFirstObjectByType<App>();
    }

    //Click sobre la carta
    private void OnMouseDown()
    {
        if (!app.IsCardSelected())
        {
            app.SelectCardToPlay(gameObject);
        }
    }

    //Mover el mouse por encima de la carta
    private void OnMouseOver()
    {
        CardUi cardUi = GetComponent<CardUi>();
        app.ViewCardInfo(cardUi.card);
    }

    void OnMouseEnter()
    {
        if (app.game.activePlayer == app.game.player1)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, gameObject.transform.position.z + 1f);
            return;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, gameObject.transform.position.z - 1f);
    }

    void OnMouseExit()
    {
        if (app.game.activePlayer == app.game.player1)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, gameObject.transform.position.z - 1f);
            return;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, gameObject.transform.position.z + 1f);
    }

}

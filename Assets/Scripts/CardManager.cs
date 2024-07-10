using UnityEngine;

public class CardManager : MonoBehaviour
{
    GameManager gameManager;
    //Inicio del juego
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    //Click sobre la carta
    private void OnMouseDown()
    {
        Debug.Log("click");
        gameManager.ClickCard(gameObject);
    }

    //Mover el mouse por encima de la carta
    private void OnMouseOver()
    {
        CardUi cardUi = GetComponent<CardUi>();
        gameManager.GenerateInfo(cardUi);
    }

    void OnMouseEnter()
    {
        if (gameManager.game.IsPlayer1Playing())
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, gameObject.transform.position.z + 1f);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, gameObject.transform.position.z - 1f);
        }
    }

    void OnMouseExit()
    {
        if (!gameManager.game.IsPlayer1Playing())
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, gameObject.transform.position.z + 1f);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, gameObject.transform.position.z - 1f);
        }
    }

}

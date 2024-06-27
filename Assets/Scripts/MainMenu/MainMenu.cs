using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject game;

    //Cambiar al menu de opciones( OptionsButton )
    public void ChangesToOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    
    //Comenzar el juego( StartButton )
    public void ChangesToGame()
    {
        mainMenu.SetActive(false);
        game.SetActive(true);
    }

    //Salir del juego( QuitButton )
    public void QuitGame()
    {
        Application.Quit();
    }

}

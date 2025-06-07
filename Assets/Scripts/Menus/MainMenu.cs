using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnclickPlayGame()
    {
        // Aqu� puedes cargar la escena del juego
        SceneManager.LoadScene("Game");
    }
    public void OnclickExitGame()
    {
        // Aqu� puedes salir del juego
        Application.Quit();
    }
    public void OnclickMainMenu()
    {
        // Aqu� puedes cargar la escena del juego
        SceneManager.LoadScene("MenuPrincipal");
    }
}

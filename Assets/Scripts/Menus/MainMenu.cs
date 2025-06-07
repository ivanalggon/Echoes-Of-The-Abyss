using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnclickPlayGame()
    {
        // Aquí puedes cargar la escena del juego
        SceneManager.LoadScene("Game");
    }
    public void OnclickExitGame()
    {
        // Aquí puedes salir del juego
        Application.Quit();
    }
    public void OnclickMainMenu()
    {
        // Aquí puedes cargar la escena del juego
        SceneManager.LoadScene("MenuPrincipal");
    }
}

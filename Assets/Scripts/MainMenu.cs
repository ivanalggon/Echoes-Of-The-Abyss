using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Aqu� puedes cargar la escena del juego
        SceneManager.LoadScene("Game");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // Ensure the game starts unpaused
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
    public void OnclickMainMenu()
    {
        // Aquí puedes cargar la escena del juego
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void ResumeGame()
    {
        TogglePause();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

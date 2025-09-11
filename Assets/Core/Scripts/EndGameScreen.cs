using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScreen : MonoBehaviour
{
    public Button restartButton;
    public Button mainMenuButton;

    private void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
    }

    public void RestartGame()
    {
        GameManager.instance.RestartGame();
    }

    public void ReturnToMainMenu()
    {
        GameManager.instance.ReturnToMainMenu();
    }
}
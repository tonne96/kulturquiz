using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject howToPlayPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("I QUIT!");
        Application.Quit();
    }

    public void ShowHowToPlay()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(true);
    }

    public void Back()
    {
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }
}
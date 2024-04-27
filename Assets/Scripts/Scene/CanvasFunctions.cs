using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasFunctions : MonoBehaviour
{
    private bool isPaused = false;
    [SerializeField] private Sprite pause1;
    [SerializeField] private Sprite pause2;
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingMenu;
    public void RestartButton()
    {
        SceneManager.LoadScene(ArchiveSystem.SceneIndex);
        ArchiveSystem.LoadArchive(ArchiveSystem.LevelIndex, FindObjectOfType<Player>().transform);
    }
    public void PauseButton()
    {
        if (!isPaused)
        {
            pauseButton.image.sprite = pause2;
            pauseMenu.SetActive(true);
            isPaused = true;
            Time.timeScale = 0.05f;
            StateSystem.Instance.canInput = false;
        }
        else
        {
            pauseButton.image.sprite = pause1;
            pauseMenu.SetActive(false);
            isPaused = false;
            Time.timeScale = 1f;
            StateSystem.Instance.canInput = true;
        }

    }
    public void ReturnMenuButton()
    {
        isPaused = false;
        Time.timeScale = 1f;
        ArchiveSystem.StoreLocalArchive();
        SceneManager.LoadScene(0);
    }
    public void ToSettings()
    {
        settingMenu.SetActive(true);
    }
    public void QuitGame()
    {
        ArchiveSystem.StoreLocalArchive();
        Application.Quit();
    }
}

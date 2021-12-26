using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour
{
    public AudioSource audioPlayer;
    public AudioClip clickSound;
    public GameObject exitCanvas;
    public GameObject resumeButton;
    public GameObject pauseButton;

    public void PlayButtonSound()
    {
        audioPlayer.PlayOneShot(clickSound);
    }

    public void LoadGameScene()
    {
        // this will be changed back in the future
        //SceneManager.LoadScene("GameScene");
        SceneManager.LoadScene("DemoScene");
    }

    public void ReturnToMenu()
    {
        exitCanvas.SetActive(false);
    }

    public void PrepareToExit()
    {
        exitCanvas.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        resumeButton.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        resumeButton.SetActive(false);
        pauseButton.SetActive(true);
    }
}

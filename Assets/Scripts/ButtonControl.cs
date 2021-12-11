using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour
{
    public AudioSource audioPlayer;
    public AudioClip clickSound;
    public GameObject exitCanvas;

    public void PlayButtonSound()
    {
        audioPlayer.PlayOneShot(clickSound);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
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
}

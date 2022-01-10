using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    public AudioSource audioPlayer;
    public AudioClip clickSound;
    public AudioSource backgroundMusic;
    public GameObject exitCanvas;
    public GameObject settingCanvas;
    public GameObject resumeButton;
    public GameObject pauseButton;
    public Text volumeText;
    public Slider volumeSlider;
    public Dropdown levelDropdown;
    public Dropdown player1Dropdown;
    public Dropdown player2Dropdown;
    public GameObject settingGameObject;

    public void PlayButtonSound()
    {
        audioPlayer.PlayOneShot(clickSound);
    }

    public void LoadGameScene()
    {
        // this will be changed back in the future
        //SceneManager.LoadScene("GameScene");
        Time.timeScale = 1;
        SceneManager.LoadScene("DemoScene2");
    }

    public void ModifySetting()
    {
        settingCanvas.SetActive(true);
    }

    public void ModifyLevel()
    {
        settingGameObject.GetComponent<SettingControl>().level = levelDropdown.value;
    }

    public void ModifyVolume()
    {
        // change music volume
        backgroundMusic.volume = volumeSlider.value / 100;
        // show the volume value
        volumeText.text = volumeSlider.value.ToString();
        // set the music volume for playscene too
        settingGameObject.GetComponent<SettingControl>().volume = volumeSlider.value / 100;
    }

    public void ModifyPlayer1()
    {
        settingGameObject.GetComponent<SettingControl>().player1 = player1Dropdown.value;
    }

    public void ModifyPlayer2()
    {
        settingGameObject.GetComponent<SettingControl>().player2 = player2Dropdown.value;
    }


    public void ReturnToMenu()
    {
        exitCanvas.SetActive(false);
        settingCanvas.SetActive(false);
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
        // destroy setting gameobject
        Destroy(GameObject.Find("SettingGameObject"));
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

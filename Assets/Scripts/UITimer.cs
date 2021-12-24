using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    float timerFloat = 0f;
    //string timerText;
    public Text timerText;
    public Text timerEndingText;


    public GameObject badgeCanvas;
    public GameObject gamingCanvas;
    public GameObject unicorn;
    public Text badgeTitle;
    public Image badgeImage;
    public Sprite bronzeBadge;
    public Sprite silverBadge;
    public Sprite goldBadge;
    public Sprite platinumBadge;
    public Sprite diamondBadge;
    public Sprite eliteBadge;
    private bool hasWon = false;

    AudioSource audioPlayer;
    public AudioClip succuessSE;


    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasWon)
        {
            timerFloat += Time.deltaTime;
            timerText.text = timerFloat.ToString("F2") + " seconds";
        }

        // if unicorn went dead then show the canvas
        if (unicorn.GetComponent<RoleController>().currentHealth <= 0)
        {
            if (!hasWon)
            {
                audioPlayer.PlayOneShot(succuessSE);
            }
            hasWon = true;
            badgeCanvas.SetActive(true);
            gamingCanvas.SetActive(false);
            timerEndingText.text = "You've protected the unicorn for " + timerFloat.ToString("F2") + " seconds!";
            if (10.0f > timerFloat && timerFloat >= 0.0f)
            {
                badgeTitle.text = "Bronze BodyGuard";
                badgeImage.sprite = bronzeBadge;
            }
            else if (20.0f > timerFloat && timerFloat >= 10.0f)
            {
                badgeTitle.text = "Silver BodyGuard";
                badgeTitle.color = new Color(0.8490566f, 0.8241089f, 0.756942f, 1);
                badgeImage.sprite = silverBadge;
            }
            else if (30.0f > timerFloat && timerFloat >= 20.0f)
            {
                badgeTitle.text = "Gold BodyGuard";
                badgeTitle.color = new Color(1, 0.9105341f, 0, 1);
                badgeImage.sprite = goldBadge;
            }
            else if (40.0f > timerFloat && timerFloat >= 30.0f)
            {
                badgeTitle.text = "Platinum BodyGuard";
                badgeTitle.color = new Color(0.6650944f, 0.9793268f, 1, 1);
                badgeImage.sprite = platinumBadge;
            }
            else if (50.0f > timerFloat && timerFloat >= 40.0f)
            {
                badgeTitle.text = "Diamond BodyGuard";
                badgeTitle.color = new Color(0.6666667f, 0.8777453f, 1, 1);
                badgeImage.sprite = diamondBadge;
            }
            else
            {
                badgeTitle.text = "Elite BodyGuard";
                badgeTitle.color = new Color(1, 1, 1, 1);
                badgeImage.sprite = eliteBadge;
            }
        }
    }

}

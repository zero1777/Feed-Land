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

    private AudioSource audioPlayer;
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
            timerEndingText.text = timerFloat.ToString("F2");
            if (10.0f > timerFloat && timerFloat >= 0.0f)
            {
                badgeTitle.text = "Bronze";
                badgeTitle.color = new Color(0.9056604f, 0.5277233f, 0.1153435f, 1);
                badgeImage.sprite = bronzeBadge;
            }
            else if (20.0f > timerFloat && timerFloat >= 10.0f)
            {
                badgeTitle.text = "Silver";
                badgeTitle.color = new Color(0.745283f, 0.7093292f, 0.6925507f, 1);
                badgeImage.sprite = silverBadge;
            }
            else if (30.0f > timerFloat && timerFloat >= 20.0f)
            {
                badgeTitle.text = "Gold";
                badgeTitle.color = new Color(1, 0.7716983f, 0.0235849f, 1);
                badgeImage.sprite = goldBadge;
            }
            else if (40.0f > timerFloat && timerFloat >= 30.0f)
            {
                badgeTitle.text = "Platinum";
                badgeTitle.color = new Color(0.6650944f, 0.9793268f, 1, 1);
                badgeImage.sprite = platinumBadge;
            }
            else if (50.0f > timerFloat && timerFloat >= 40.0f)
            {
                badgeTitle.text = "Diamond";
                badgeTitle.color = new Color(0.0764062f, 0.6203933f, 1, 1);
                badgeImage.sprite = diamondBadge;
            }
            else
            {
                badgeTitle.text = "Elite";
                badgeTitle.color = new Color(1, 1, 1, 1);
                badgeImage.sprite = eliteBadge;
            }
        }
    }

}

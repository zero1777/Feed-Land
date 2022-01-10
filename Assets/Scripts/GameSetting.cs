using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public GameObject unicorn;
    private GameObject settingGameObject;
    private SettingControl initialSettings;

    private int level;
    private int player1;
    private int player2;

    // Start is called before the first frame update
    void Start()
    {
        settingGameObject = GameObject.Find("SettingGameObject");
        initialSettings = settingGameObject.GetComponent<SettingControl>();

        // set the game difficulty level
        level = initialSettings.level;
        if (level == 0) {
            unicorn.GetComponent<RoleController>().maxHealth = 30;
        } else if (level == 1) {
            unicorn.GetComponent<RoleController>().maxHealth = 20;
        } else if (level == 2) {
            unicorn.GetComponent<RoleController>().maxHealth = 3;
        }

        // set the background music volume
        backgroundMusic.volume = initialSettings.volume;

        // set the player1 avatar

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

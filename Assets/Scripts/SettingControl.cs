using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingControl : MonoBehaviour
{
    public int level = 0;
    public float volume = 0.2f;
    public int player1 = 0;
    public int player2 = 0;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        GetLevel();
    }

    public int GetLevel() {
        print("Now level is:" + level);
        return level;
    }

    public float GetVolume() {
        print(volume);
        return volume;
    }

    public int GetPlayer1() {
        print(player1);
        return player1;
    }

    public int GetPlayer2() {
        print(player2);
        return player2;
    }
}

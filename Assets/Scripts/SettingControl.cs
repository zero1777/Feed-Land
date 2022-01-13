using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingControl : MonoBehaviour
{
    public int level = 0;
    public float volume = 0.2f;
    public int players = 2;
    public int player1 = 0;
    public int player2 = 1;
    public int player3 = 1;
    public int player4 = 1;



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
    }

}

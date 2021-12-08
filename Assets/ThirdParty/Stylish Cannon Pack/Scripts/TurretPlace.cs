using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPlace : MonoBehaviour
{
    public GameObject turret;
    public GameObject minesPlace;
    public GameObject mine;
    // public GameObject turretPlace;
    public int demandMines;

    private int currentMines;
    private int places;
    // Start is called before the first frame update
    void Start()
    {
        places = 4;
        currentMines = 0;
        InitialMines();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMinesStatus();
    }

    public void GetMines() {
        if (currentMines < 4) currentMines++;
        if (currentMines >= demandMines) ConstructTurret();
    }

    void InitialMines() {
        Vector3 offsetY = new Vector3(0f, 0.5f, 0f);
        for (int i=0; i<demandMines; i++) {
            GameObject m = Instantiate(mine, minesPlace.transform);
            m.transform.localPosition = offsetY * i;
        }
    }

    void ConstructTurret() {
        // destroy the floor
        for (int i=0; i<places; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        // destroy the floating mines
        for (int i=0; i<demandMines; i++) {
            Destroy(minesPlace.transform.GetChild(i).gameObject);
        }
        Destroy(minesPlace);

        // construct the turret on the current turret place
        Instantiate(turret, gameObject.transform);
        turret.transform.localPosition = new Vector3(-0.5f, 1.0f, -0.5f);
    }

    void UpdateMinesStatus() {
        if (currentMines == demandMines) return;
        SetMinesAlphaVal(1f);
    }

    void SetMinesAlphaVal(float alphaVal) {
        for (int i=0; i<currentMines; i++) {
            Material mat = minesPlace.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().materials[0];
            Color newColor = new Color(mat.color.r, mat.color.g, mat.color.b, alphaVal);
            mat.color = newColor;
        }
    }
}

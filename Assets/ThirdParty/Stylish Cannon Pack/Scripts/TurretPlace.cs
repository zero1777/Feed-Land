using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPlace : MonoBehaviour
{
    public GameObject turret;
    public GameObject mines;
    // public GameObject turretPlace;
    public int demandMines;

    private int currentMines;
    // Start is called before the first frame update
    void Start()
    {
        currentMines = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlaceStatus();
    }

    public void GetMines() {
        if (currentMines < 4) currentMines++;
        if (currentMines >= demandMines) ConstructTurret();
    }

    void ConstructTurret() {
        Vector3 offsetY = new Vector3(0f, 1f, 0f);
        Instantiate(turret, gameObject.transform);
        turret.transform.localPosition = new Vector3(-0.5f, 1.0f, -0.5f);
    }

    void UpdatePlaceStatus() {
        SetAlphaVal(0.5f);
    }

    void SetAlphaVal(float alphaVal) {
        for (int i=0; i<currentMines; i++) {
            Material mat = transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().materials[0];
            Color newColor = new Color(mat.color.r, mat.color.g, mat.color.b, alphaVal);
            mat.color = newColor;
        }
    }
}

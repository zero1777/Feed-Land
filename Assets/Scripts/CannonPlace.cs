using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonPlace : MonoBehaviour
{
    public GameObject cannonPrefab;
    public GameObject minesPlace;
    public GameObject minePrefab;
    // public GameObject cannonPlace;
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

    public void GetMine()
    {
        if (currentMines < demandMines) currentMines++;
        if (currentMines >= demandMines) ConstructCannon();
    }

    private void InitialMines()
    {
        Vector3 baseY = new Vector3(0f, 0.7f, 0f);
        Vector3 offsetY = new Vector3(0f, 0.5f, 0f);
        for (int i = 0; i < demandMines; i++)
        {
            GameObject m = Instantiate(minePrefab, minesPlace.transform);
            m.transform.localPosition = offsetY * i + baseY;
        }
    }

    private void ConstructCannon()
    {
        // destroy the floor
        for (int i = places - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // destroy the floating mines
        // for (int i=0; i<demandMines; i++) {
        //     Destroy(minesPlace.transform.GetChild(i).gameObject);
        // }
        Destroy(minesPlace);

        // construct the cannon on the current cannon place
        Instantiate(cannonPrefab, gameObject.transform);
        cannonPrefab.transform.localPosition = new Vector3(-0.5f, 1.0f, -0.5f);
    }

    private void UpdateMinesStatus()
    {
        if (currentMines == demandMines) return;
        SetMinesAlphaVal(1f);
    }

    private void SetMinesAlphaVal(float alphaVal)
    {
        for (int i = 0; i < currentMines; i++)
        {
            Material mat = minesPlace.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().materials[0];
            Color newColor = new Color(mat.color.r, mat.color.g, mat.color.b, alphaVal);
            mat.color = newColor;
        }
    }
}

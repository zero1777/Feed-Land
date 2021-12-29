using UnityEngine;

public class CannonPlace : MonoBehaviour
{
    public GameObject cannonPrefab;
    public GameObject minePrefab;
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
        // testing
        // if (Input.GetKeyDown(KeyCode.A)) {
        //     ConstructCannon();
        // }
        UpdateMinesStatus();
    }

    public bool GetMine()
    {
        Debug.Log("[CannonPlace.GetMine]: get mine successfully");

        currentMines++;

        return currentMines == demandMines;
    }

    public void ConstructCannon()
    {
        // construct the cannon on the current cannon place
        Debug.Log("[CannonPlace.ConstructCannon]: construct cannon successfully");
        GameObject cannon = Instantiate(cannonPrefab, gameObject.transform.position, Quaternion.identity);
        cannon.transform.Rotate(0f, 180f, 0f);
    }

    private void InitialMines()
    {
        Vector3 baseY = new Vector3(0f, 1f, 0f);
        Vector3 offsetY = new Vector3(0f, 0.5f, 0f);
        for (int i = 0; i < demandMines; i++)
        {
            GameObject mine = Instantiate(minePrefab, gameObject.transform);
            mine.transform.localPosition = offsetY * i + baseY;
            mine.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            Material mat = mine.GetComponent<MeshRenderer>().materials[0];
            Color newColor = new Color(mat.color.r, mat.color.g, mat.color.b, 0.5f);
            mat.color = newColor;
        }
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
            Material mat = gameObject.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().materials[0];
            Color newColor = new Color(mat.color.r, mat.color.g, mat.color.b, alphaVal);
            mat.color = newColor;
        }
    }
}

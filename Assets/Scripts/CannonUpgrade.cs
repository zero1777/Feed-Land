using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CannonUpgrade : MonoBehaviour
{
    public GameObject upgradeCannonPrefab;
    public int demandUpgradeMaterials;
    public Image progressBar;
    private int currentUpgradeMaterials;
    // Start is called before the first frame update
    void Start()
    {
        currentUpgradeMaterials = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown) {
            GetUpgradeMaterial();
        }
        UpdateProgressBar((float)currentUpgradeMaterials / demandUpgradeMaterials);
    }

    private void UpdateProgressBar(float ratio) {
        // Debug.Log(currentUpgradeMaterials);
        progressBar.fillAmount = ratio;
    }

    private void UpgradeCannon() {
        Instantiate(upgradeCannonPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void GetUpgradeMaterial() {
        if (currentUpgradeMaterials < demandUpgradeMaterials) 
            currentUpgradeMaterials++;
        if (currentUpgradeMaterials >= demandUpgradeMaterials) UpgradeCannon();
    }
}

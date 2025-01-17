﻿using UnityEngine;
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
        UpdateProgressBar((float)currentUpgradeMaterials / demandUpgradeMaterials);
    }

    // Update is called once per frame
    void Update()
    {
        // for testing
        // if (Input.GetKeyDown(KeyCode.Tab)) {
        //     // GetUpgradeMaterial();
        //     UpgradeCannon();
        // }
        UpdateProgressBar((float)currentUpgradeMaterials / demandUpgradeMaterials);
    }

    private void UpdateProgressBar(float ratio)
    {
        // Debug.Log(currentUpgradeMaterials);
        progressBar.fillAmount = ratio;
    }

    public void UpgradeCannon()
    {
        Debug.Log("[CannonUpgrade.UpgradeCannon]: Cannon Upgrade successfully");
        GameObject upgradeCannon = Instantiate(upgradeCannonPrefab, transform.position, transform.rotation);
    }

    public bool GetUpgradeMaterial()
    {
        Debug.Log("[CannonUpgrade.GetUpgradeMaterial]: get upgrade material successfully");
        currentUpgradeMaterials++;

        return currentUpgradeMaterials == demandUpgradeMaterials;
    }
}

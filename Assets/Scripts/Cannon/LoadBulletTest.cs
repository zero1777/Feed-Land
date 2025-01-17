﻿using UnityEngine;

public class LoadBulletTest : MonoBehaviour
{
    // public GameObject cannonObject;
    public GameObject cannonObject2;
    public GameObject redFoodPrefab;
    public GameObject blueFoodPrefab;
    public GameObject greenFoodPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // cannonObject.GetComponent<CannonSenseShoot>().LoadBullet(redFoodPrefab);
            cannonObject2.GetComponent<CannonSenseShoot>().LoadBullet(redFoodPrefab);
        }

        if (Input.GetMouseButtonDown(1))
        {
            // cannonObject.GetComponent<CannonSenseShoot>().LoadBullet(blueFoodPrefab);
            cannonObject2.GetComponent<CannonSenseShoot>().LoadBullet(blueFoodPrefab);
        }

        /* Debug only
        if (Input.GetKeyDown("space"))
        {
            cannonObject.GetComponent<CannonSenseShoot>().LoadBullet(greenFoodPrefab);
            cannonObject2.GetComponent<CannonSenseShoot>().LoadBullet(greenFoodPrefab);
        }
        */
    }
}

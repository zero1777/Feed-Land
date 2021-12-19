using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBulletTest : MonoBehaviour
{
    public GameObject cannonObject;
    public GameObject redFoodPrefab;
    public GameObject blueFoodPrefab;
    public GameObject greenFoodPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cannonObject.GetComponent<CannonSenseShoot>().LoadBullet(redFoodPrefab);
        }

        if (Input.GetMouseButtonDown(1))
        {
            cannonObject.GetComponent<CannonSenseShoot>().LoadBullet(blueFoodPrefab);
        }

        if (Input.GetKeyDown("space"))
        {
            cannonObject.GetComponent<CannonSenseShoot>().LoadBullet(greenFoodPrefab);
        }
    }
}

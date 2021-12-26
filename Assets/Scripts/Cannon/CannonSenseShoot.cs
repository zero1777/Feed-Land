using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CannonSenseShoot : MonoBehaviour
{
    // tag definitions
    public const string foodTagSuffix = "_food";

    // nearest enemy related
    public float checkRadius;
    public LayerMask checkLayers; // Layer = Enemy

    // shooting bullet related
    public Transform shootPoint;
    public int maxBulletNum = 3;
    public GameObject redBulletPrefab;
    public GameObject blueBulletPrefab;
    public GameObject greenBulletPrefab;
    public Transform stagePoint;  // the bullet used for display 
    public GameObject redDisplayPrefab;
    public GameObject blueDisplayPrefab;
    public GameObject greenDisplayPrefab;
    private float coolDownTime = 3f; // shooting CD
    private float shootTimer = 0f;
    private bool vacant = true;
    private Dictionary<string, GameObject> bulletPrefabs;
    private Dictionary<string, GameObject> displayPrefabs;
    private Queue<GameObject> waitBullets = new Queue<GameObject>();

    void Start()
    {
        bulletPrefabs = new Dictionary<string, GameObject>(){
            {"red", redBulletPrefab},
            {"green", greenBulletPrefab},
            {"blue", blueBulletPrefab},
        };

        displayPrefabs = new Dictionary<string, GameObject>(){
            {"red", redDisplayPrefab},
            {"green", greenDisplayPrefab},
            {"blue", blueDisplayPrefab},
        };
    }
    void Update()
    {
        shootTimer += Time.deltaTime;
        int waitBulletsCount = waitBullets.Count;
        // build the bullet in runtime
        if (shootTimer > coolDownTime && waitBulletsCount > 0 && vacant)
        {
            shootTimer = 0f;
            GameObject nextBullet = waitBullets.Dequeue();

            // the bullet used for display will be destroyed if that bullet is fired
            Destroy(stagePoint.GetChild(0).gameObject);

            // create next bullet
            CreateBullet(nextBullet.tag.Split('_')[0]);
            vacant = false;
        }

        if (shootPoint.childCount == 0)
        {
            vacant = true;
        }
    }

    // return a nearest enemy with correct tag or null
    public Collider FindNearestEnemyWithTag(GameObject food)
    {
        // find the near enemies
        Collider[] nearEnemies = Physics.OverlapSphere(transform.position, checkRadius, checkLayers);

        // compare enemies distance, array[0] is the nearest one
        Array.Sort(nearEnemies, new DistanceComparer(transform));

        String foodColor = food.tag.Split('_')[0];

        // find the same tag between food and monster
        foreach (Collider nearEnemy in nearEnemies)
        {
            String monsterColor = nearEnemy.tag.Split('_')[0];
            if (foodColor == monsterColor)
            {
                return nearEnemy;
            }
        }

        return null;
    }

    // draw the checking radius
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }

    // load bullet
    public bool LoadBullet(GameObject newBullet)
    {
        if (waitBullets.Count >= maxBulletNum)
        {
            return false;
        }

        if (newBullet.tag.EndsWith(foodTagSuffix))
        {
            waitBullets.Enqueue(newBullet);
            Debug.Log("load bullet");

            // display on the top of cannon
            CreateDisplayBullet(newBullet.tag.Split('_')[0]);
        }

        return true;
    }

    private void CreateBullet(string color = "")
    {
        Instantiate(bulletPrefabs[color], shootPoint.position, shootPoint.rotation, shootPoint);
    }

    private void CreateDisplayBullet(string color = "")
    {
        GameObject ammoFood = Instantiate(displayPrefabs[color], stagePoint);
        ammoFood.transform.localPosition = new Vector3(0.3f, 0, 4.0f);
    }
}

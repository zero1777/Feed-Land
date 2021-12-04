using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CannonSenseShoot : MonoBehaviour
{
    public float checkRadius;
    public LayerMask checkLayers; // Layer = Enemy
    public GameObject bulletPrefab;
    public int bulletNum = 1;
    public Transform shootPoint;
    private float coolDownTime = 3f; // shooting CD
    private float shootTimer = 0f;
    private Collider nearestEnemy;
    private bool vacant = true;

    void Update()
    {
        shootTimer += Time.deltaTime;

        // build the bullet in runtime
        if (shootTimer > coolDownTime && bulletNum > 0 && vacant)
        {
            shootTimer = 0f;
            bulletNum -= 1;
            Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity, shootPoint);
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
}

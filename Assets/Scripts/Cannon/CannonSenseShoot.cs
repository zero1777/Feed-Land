using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class CannonSenseShoot : MonoBehaviour
{
    public float checkRadius;
    public LayerMask checkLayers; // Layer = Enemy
    public GameObject bulletPrefab;
    public int bulletNum = 1;
    public Transform shootPoint;
    private float coolDownTime = 3f; // shooting CD
    private float _shootTimer = 0f;
    private Collider nearestEnemy;
    private bool vacant = true;

    void FixedUpdate()
    {
        _shootTimer += Time.deltaTime;

        // build the bullet in runtime
        if (_shootTimer > coolDownTime && bulletNum > 0 && vacant)
        {
            _shootTimer = 0f;
            bulletNum -= 1;
            // Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            Instantiate(bulletPrefab, shootPoint).transform.position = shootPoint.position;
            // vacant = false;
        }
    }

    // return a nearest enemy with correct tag or null
    public Collider FindNearestEnemyWithTag(GameObject food)
    {
        // find the near enemies
        Collider[] nearEnemies = Physics.OverlapSphere(transform.position, checkRadius, checkLayers);

        // compare enemies distance, array[0] is the nearest one
        Array.Sort(nearEnemies, new DistanceComparer(transform));

        String foodColor = Regex.Match(food.tag, @"^[^_]*").Value;

        // find the same tag between food and monster
        foreach (Collider nearEnemy in nearEnemies)
        {
            String monsterColor = Regex.Match(nearEnemy.tag, @"^[^_]*").Value;
            if (foodColor == monsterColor)
            {
                return nearEnemy;
            }
        }

        return null;
    }
    // draw the checking radius
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}

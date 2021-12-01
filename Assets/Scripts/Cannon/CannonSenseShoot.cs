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
    public AnimationCurve lerpCurve;
    public Vector3 lerpOffset;  // the curve height and shape
    public float lerpTime = 3f; // how long it takes from A to B
    
    private float coolDownTime = 3f; // shooting CD
    private float _timer, _shootTimer = 0f;
    private GameObject bullet;
    private Collider nearestEnemy;

    void FixedUpdate()
    {
        _shootTimer += Time.deltaTime;

        // find all the enemies as colliders
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, checkLayers);

        // compare enemies distance, array[0] is the nearest one
        Array.Sort(colliders, new DistanceComparer(transform));

        // build the bullet in runtime
        if (_shootTimer > coolDownTime && bulletNum > 0 && colliders.Length != 0)
        {
            _shootTimer = 0f;
            _timer = 0f;
            bulletNum -= 1;
            Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            bullet = GameObject.Find("Sphere(Clone)"); ;
        }

        // throw bullet
        if (bullet != null)
        {
            _timer += Time.deltaTime;
            if (_timer > lerpTime)
            {

                _timer = lerpTime;
            }
            
            nearestEnemy = colliders[0];
            
            // lerpRatio: 0-1
            float lerpRatio = _timer / lerpTime;

            // become curve otherwise it will be a straight line
            Vector3 positionOffset = lerpCurve.Evaluate(lerpRatio) * lerpOffset;
            // bullet will follow the curve and be reached the destination in [lerpTime]
            bullet.transform.position = Vector3.Lerp(shootPoint.position, nearestEnemy.transform.position, lerpRatio) + positionOffset;
        }
    }

    // draw the checking radius
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}

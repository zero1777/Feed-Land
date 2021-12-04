using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletController : MonoBehaviour
{
    public float checkRadius;
    public LayerMask checkLayers; // Layer = Enemy
    public AnimationCurve lerpCurve;
    public Vector3 lerpOffset;
    public float lerpTime = 3f;
    public Rigidbody rb;
    private float _timer = 0f;
    private Collider nearestEnemy;
    private Vector3 startPoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPoint = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // find all the enemies as colliders
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, checkLayers);

        // compare enemies distance, array[0] is the nearest one
        Array.Sort(colliders, new DistanceComparer(transform));

        // return a nearest enemy with correct tag or null
        nearestEnemy = transform.parent.parent.parent.GetComponent<CannonSenseShoot>().FindNearestEnemyWithTag(this.gameObject);

        // if there is an enemy, throw bullet to the nearest enemy
        if (nearestEnemy != null)
        {
            ActivateRb();
            _timer += Time.deltaTime;
            if (_timer > lerpTime)
            {
                _timer = lerpTime;
            }

            // lerpRatio: 0-1
            float lerpRatio = _timer / lerpTime;

            // become curve otherwise it will be a straight line
            Vector3 positionOffset = lerpCurve.Evaluate(lerpRatio) * lerpOffset;

            // bullet will follow the curve and be reached the destination in [lerpTime]
            transform.position = Vector3.Lerp(startPoint, nearestEnemy.transform.position, lerpRatio) + positionOffset;

        }
        else
        {
            DesactivateRb();
        }

    }

    // Destroy bullet when it hit on dragon
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag.Contains("monster"))
        {
            Debug.Log("hit monster");
            Destroy(gameObject);
        }
    }

    public void DesactivateRb()
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;
    }

    public void ActivateRb()
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
    }
}

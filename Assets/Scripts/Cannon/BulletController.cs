using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletController : MonoBehaviour
{
    public AnimationCurve lerpCurve;
    public Vector3 lerpOffset;
    public float lerpTime = 3f;
    private Rigidbody rb;
    private float timer = 0f;
    private Vector3 startPoint;
    private Transform cannonTransform;
    private Animation m_animations;
    private bool isShoot = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPoint = transform.position;

        m_animations = gameObject.GetComponentInParent<Animation>();
        cannonTransform = this.transform.parent.parent;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // return a nearest enemy with correct tag or null
        Collider nearestEnemy = gameObject.GetComponentInParent<CannonSenseShoot>().FindNearestEnemyWithTag(gameObject);

        // if there is an enemy, throw bullet to the nearest enemy
        if (nearestEnemy != null)
        {
            ActivateRb();
            timer += Time.deltaTime;
            if (timer > lerpTime)
            {
                timer = lerpTime;
            }

            // Cannon turn to nearest enemy
            FaceTarget(nearestEnemy.transform, cannonTransform);

            if (!isShoot && m_animations != null)
            {
                isShoot = true;
                m_animations.Play("CannonShoot");
            }

            // lerpRatio: 0-1
            float lerpRatio = timer / lerpTime;

            // become curve otherwise it will be a straight line
            Vector3 positionOffset = lerpCurve.Evaluate(lerpRatio) * lerpOffset;

            // bullet will follow the curve and be reached the destination in [lerpTime]
            transform.position = Vector3.Lerp(startPoint, nearestEnemy.transform.position, lerpRatio) + positionOffset;
        }
        else
        {
            DeactivateRb();
        }

    }

    // Destroy bullet when it hit on dragon
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag.Contains("monster"))
        {
            Debug.Log("hit monster");
            Destroy(gameObject);
            isShoot = false;
        }
    }

    public void DeactivateRb()
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;
    }

    public void ActivateRb()
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
    }

    private void FaceTarget(Transform target, Transform cannon)
    {
        Vector3 direction = (target.position - cannon.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        cannon.rotation = Quaternion.Slerp(cannon.rotation, lookRotation, Time.deltaTime * 5f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitDragon : MonoBehaviour
{
    // Destroy bullet when it hit on dragon
    void OnCollisionEnter(Collision other) 
    {
        if (other.collider.tag.Contains("monster"))
        {
            Destroy(gameObject);
        }
    }
}

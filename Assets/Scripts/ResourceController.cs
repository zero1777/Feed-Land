using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //for NavMeshAgent

public class ResourceController : MonoBehaviour
{

    public int maxHealth = 3;
    public int currentHealth;

    public enum Ingredient
    {
        // ...
    }

    void Start()
    {
        currentHealth = maxHealth;
    }


    void Update()
    {
        // debug health X to -hp
        if (Input.GetKeyDown(KeyCode.X))
        {
            getHit(1);
        }
    }

    public bool getHit(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            DestroyResource();
            return true;
        }
        return false;
    }

    public void DestroyResource()
    {
        Destroy(gameObject);
    }
}

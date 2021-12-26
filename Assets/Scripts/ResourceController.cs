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
            GetHit(1);
        }
    }

    public bool GetHit(int damage)
    {
        Debug.Log("[Resource.GetHit]: get hit successfully");

        currentHealth -= damage;
        return currentHealth <= 0;
    }
}

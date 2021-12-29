using UnityEngine;

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
        /* Debug only
        if (Input.GetKeyDown(KeyCode.X))
        {
            GetHit(1);
        }
        */
    }

    public bool GetHit(int damage)
    {
        Debug.Log("[Resource.GetHit]: get hit successfully");

        currentHealth -= damage;
        return currentHealth <= 0;
    }
}

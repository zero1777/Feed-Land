using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI; //for NavMeshAgent
using System;

public class RoleController : MonoBehaviour
{

    public int maxHealth = 5;
    public int currentHealth;
    public HealthBar healthBar;


    public enum Direction
    {
        UP ,
        Down,
        Left,
        Right
    };

    public List<Vector3> storedPath;

    private NavMeshAgent nma = null;
    
    // use to decide route
    private Vector3 initDestination;
    private Vector3 newDirection;
    private Vector3 newDestination;

    private bool canMove;

    private Animator animator;

    Rigidbody rb;

	void Start()
    {
        // init
        rb = GetComponent<Rigidbody>();
        canMove = true;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        nma = this.GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();


    }

	void FixedUpdate()
	{
        // check role is moving or not
        if (nma.hasPath == false && canMove == true)
        {
            // decide new route
            initDestination = new Vector3(Mathf.Round(this.gameObject.transform.position.x), this.gameObject.transform.position.y, Mathf.Round(this.gameObject.transform.position.z));
            newDirection = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 2));
            newDestination = initDestination + newDirection;
            nma.SetDestination(newDestination);
            animator.SetFloat("speed", 0.0f);
            // store path
            storedPath.Add(newDirection);
        }
        else
        {
            animator.SetFloat("speed", 1.0f);
            
        }
        // check if dead
        if (currentHealth <= 0)
        {
            canMove = false;
            nma.isStopped = true;
            animator.SetInteger("animation", 10);
        }
        else
        {
            canMove = true;
            nma.isStopped = false;
            animator.SetInteger("animation", 1);
        }
        // debug health Z to -hp
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }
    public void appendPath(Vector3 newPath)
    {
        storedPath.Add(newPath);
    }
}

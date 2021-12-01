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

    // store path
    public List<Vector3> storedPath;
    public int bufferSize = 15;

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
        // init para
        rb = GetComponent<Rigidbody>();
        canMove = true;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        nma = this.GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // init route;
        initDestination = new Vector3(Mathf.Round(this.gameObject.transform.position.x), this.gameObject.transform.position.y, Mathf.Round(this.gameObject.transform.position.z));
        newDirection = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 2));
        newDestination = initDestination + newDirection;
        InitFirst15Path();

    }

	void FixedUpdate()
	{

        // dynamic generate route list
        if (storedPath.Count < bufferSize && canMove == true)
        {
            newDirection = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 2));
            newDestination += newDirection;
            // store path
            storedPath.Add(newDestination);
        }
        // check role is moving or not
        else if(nma.hasPath == false && canMove == true)
        {

            nma.SetDestination(storedPath[0]);
            animator.SetFloat("speed", 0.0f);
            storedPath.RemoveAt(0);

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
        // check if out of bound
        if (this.gameObject.transform.position.x <=-8)
        {
            canMove = false;
        }
        // debug health Z to -hp
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TakeDamage(1);
        }
        Debug.Log(storedPath[0]);
    }

    // insert fixed first 15 path
    private void InitFirst15Path()
    {
        storedPath.Add(new Vector3(6.0f, 0.7f, 6.0f));
        storedPath.Add(new Vector3(5.0f, 0.7f, 7.0f));
        storedPath.Add(new Vector3(5.0f, 0.7f, 6.0f));
        storedPath.Add(new Vector3(4.0f, 0.7f, 7.0f));
        storedPath.Add(new Vector3(4.0f, 0.7f, 8.0f));
        storedPath.Add(new Vector3(4.0f, 0.7f, 7.0f));
        storedPath.Add(new Vector3(3.0f, 0.7f, 6.0f));
        storedPath.Add(new Vector3(2.0f, 0.7f, 5.0f));
        storedPath.Add(new Vector3(1.0f, 0.7f, 5.0f));
        storedPath.Add(new Vector3(1.0f, 0.7f, 4.0f));
        storedPath.Add(new Vector3(0.0f, 0.7f, 4.0f));
        storedPath.Add(new Vector3(-1.0f, 0.7f, 4.0f));
        storedPath.Add(new Vector3(-2.0f, 0.7f, 5.0f));
        storedPath.Add(new Vector3(-3.0f, 0.7f, 5.0f));
        storedPath.Add(new Vector3(-4.0f, 0.7f, 4.0f));
        initDestination = new Vector3(-4.0f, 0.7f, 4.0f);
        newDirection = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 2));
        newDestination = initDestination + newDirection;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }
    public void AppendPath(Vector3 newPath)
    {
        storedPath.Add(newPath);
    }
}

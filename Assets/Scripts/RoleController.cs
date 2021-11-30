using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI; //for NavMeshAgent
using System;

public class RoleController : MonoBehaviour
{
    public float speed;
    public float rotateSpeed;
    public float jumpForce;

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
    private GameObject[] randomPoint;
    private int currentRandom;


    private Vector3 initPos;
    private Vector3 initDestination;
    private Vector3 newDirection;
    private Vector3 newDestination;

    private bool canJump;
    private bool canMove;

    private Animator animator;

    Rigidbody rb;

	void Start()
    {
        rb = GetComponent<Rigidbody>();
		initPos = this.gameObject.transform.position;
		canJump = true;
        canMove = true;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        nma = this.GetComponent<NavMeshAgent>();
        randomPoint = GameObject.FindGameObjectsWithTag("random_point");
        Debug.Log("random_point = " + randomPoint.Length.ToString());

        animator = GetComponent<Animator>();


    }

    void Update()
    {

    }

	void FixedUpdate()
	{

        if (nma.hasPath == false && canMove == true)
        {
            /*currentRandom = UnityEngine.Random.Range(0, randomPoint.Length + 1);
            nma.SetDestination(randomPoint[currentRandom].transform.position);
            Debug.Log("Moving to RandomPoint " + currentRandom.ToString());*/
            initDestination = new Vector3(Mathf.Round(this.gameObject.transform.position.x), this.gameObject.transform.position.y, Mathf.Round(this.gameObject.transform.position.z));
            newDirection = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 2));
            newDestination = initDestination + newDirection;
            Debug.Log("Moving to newDestination " + newDestination.ToString());
            nma.SetDestination(newDestination);
            animator.SetFloat("speed", 0.0f);
            storedPath.Add(newDirection);
        }
        else
        {
            animator.SetFloat("speed", 1.0f);
            
        }
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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TakeDamage(1);
        }
    }


	private void OnTriggerEnter(Collider other)
	{

    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }
    public void appendPath()
    {

    }
}

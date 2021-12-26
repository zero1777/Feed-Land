using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleController : MonoBehaviour
{
    public int maxHealth = 5;
    public float roleMovingSpeed = 2.0f;
    public float roleRotSpeed = 0.1f;
    public int currentHealth;

    public HealthBar healthBar;
    public MapGenerator mapGenerator;

    public enum Direction
    {
        UP,
        Down,
        Left,
        Right
    };

    public List<GameObject> enemies = new List<GameObject>();
    public List<Vector3> storedPath;

    // use to decide route
    private Vector3 targetPosition;
    private Vector3 lookAtTarget;
    private Quaternion roleRot;
    private bool canMove;
    private bool isMoving = false;
    private Animator animator;
    private Rigidbody rb;
    // camera & light follow
    private GameObject mainCamera;
    private GameObject light;

    IEnumerator Start()
    {
        // init para
        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        rb = GetComponent<Rigidbody>();
        canMove = true;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        animator = GetComponent<Animator>();

        mainCamera = GameObject.Find("Main Camera");
        light = GameObject.Find("Directional Light");

        // generate Enemy after 1 second, every 10 second generate another monster

        InvokeRepeating("GenerateEnemy", 1.0f, 10.0f);
        // wait mapGenerator has terminated own "Start" life cycle
        yield return new WaitUntil(() => mapGenerator.isInitialized);
        GetMapPath();
    }

    void FixedUpdate()
    {
        // check is moving 
        if (isMoving && canMove == true)
            MoveRole();
        // check role is  not
        if (isMoving == false && canMove == true)
        {
            if (storedPath.Count > 0)
            {
                targetPosition = storedPath[0];
                SetTargetPosition();
                animator.SetFloat("speed", 1.0f);
                storedPath.RemoveAt(0);
            }
        }
        else
        {
            animator.SetFloat("speed", 0.0f);
        }

        // check if dead
        if (currentHealth <= 0)
        {
            canMove = false;
            animator.SetInteger("animation", 10);
        }
        else
        {
            canMove = true;
            animator.SetInteger("animation", 1);
        }
    }

    void LateUpdate()
    {
        Vector3 mainCameraPosition = new Vector3(transform.position.x - 5.0f, mainCamera.transform.position.y, mainCamera.transform.position.z);
        mainCamera.transform.position = mainCameraPosition;
        Vector3 lightPosition = new Vector3(transform.position.x - 5.0f, light.transform.position.y, light.transform.position.z);
        light.transform.position = lightPosition;
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

    private void GenerateEnemy()
    {
        int idx = Random.Range(0, enemies.Count);
        Instantiate(enemies[idx]);
    }

    private void GetMapPath()
    {
        for (int i = 0; i < mapGenerator.mapNum; i++)
        {
            for (int j = 0; j < mapGenerator.GetPath(i).Count; j++)
            {
                storedPath.Add(mapGenerator.GetPath(i)[j]);
                //Debug.Log(mapGenerator.GetPath(i)[j]);
            }
        }
    }

    private void SetTargetPosition()
    {
        transform.LookAt(targetPosition);
        lookAtTarget = new Vector3(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y, targetPosition.z - transform.position.z);
        roleRot = Quaternion.LookRotation(lookAtTarget);
        isMoving = true;
    }

    private void MoveRole()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, roleRot, roleRotSpeed * Time.fixedDeltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, roleMovingSpeed * Time.fixedDeltaTime);
        if (transform.position == targetPosition)
        {
            isMoving = false;
        }
    }
}

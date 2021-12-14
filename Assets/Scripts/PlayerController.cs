using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // player controller settings
    public float movementSpeed;
    public float rotationSpeed;
    public float actionCooldown;

    // tag definitions
    public const string turretPlaceTag = "turret_place";
    public const string turretTag = "turret";
    public const string treeTagSuffix = "_tree";
    public const string mineTagSuffix = "_mine";

    // carriable object prefabs
    public GameObject redFoodPrefab;
    public GameObject greenFoodPrefab;
    public GameObject blueFoodPrefab;
    public GameObject redMinePrefab;
    public GameObject greenMinePrefab;
    public GameObject blueMinePrefab;

    private Animator animator;
    private List<GameObject> targets;
    private float nextActionTime;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        targets = new List<GameObject>();

        nextActionTime = Time.time;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            bool shouldResetCooldown = TakeAction(Time.time > nextActionTime);
            if (shouldResetCooldown) {
                nextActionTime = Time.time + actionCooldown;
            }
        }
    }

    void FixedUpdate()
    {
        // get keyboard inputs
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput);

        // set animation
        animator.SetFloat("speed", moveDirection.magnitude);

        // move player
        moveDirection.Normalize();
        transform.Translate(moveDirection * movementSpeed * Time.deltaTime, Space.World);

        // gracefully rotate to look forward
        if (moveDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PlayerController.OnTriggerEnter] enter {other.transform.name}");

        targets.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other) {
        Debug.Log($"[PlayerController.OnTriggerExit] exit {other.transform.name}");

        targets.Remove(other.gameObject);
    }

    // TakeAction returns true if should reset the cooldown
    private bool TakeAction(bool isInCooldown) {
        bool shouldResetCooldown = false;

        foreach (GameObject target in targets) {
            if (target.CompareTag(turretPlaceTag)) {
                Debug.Log($"[PlayerController.TakeAction] interacting with turret place: {target.name}");

                // TODO: call the turret place method to interact with the turret place
            }

            if (target.CompareTag(turretTag)) {
                Debug.Log($"[PlayerController.TakeAction] interacting with turret: {target.name}");

                // TODO: call the turret method to interact with the turret
            }

            if (target.tag.EndsWith(treeTagSuffix) && isInCooldown) {
                Debug.Log($"[PlayerController.TakeAction] interacting with tree: {target.name}");

                // TODO: call the tree method to interact with the tree

                animator.SetTrigger("cutting");
                shouldResetCooldown = true;
            }
            
            if (target.tag.EndsWith(mineTagSuffix) && isInCooldown) {
                Debug.Log($"[PlayerController.TakeAction] interacting with mine: {target.name}");

                // TODO: call the mine method to interact with the mine

                animator.SetTrigger("mining");
                shouldResetCooldown = true;
            }
        }

        return shouldResetCooldown;
    }
}

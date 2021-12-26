using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // player controller settings
    public float movementSpeed;
    public float rotationSpeed;
    public float actionCooldown;

    // tag definitions
    public const string cannonPlaceTag = "cannon_place";
    public const string cannonTag = "cannon";
    public const string treeTagSuffix = "_tree";
    public const string mineTagSuffix = "_mine";
    public const string foodTagSuffix = "_food";

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
    private GameObject carryingObject;
    private GameObject toBeDestroyedObject;

    // prefabs collections for convinience
    private Dictionary<string, GameObject> foodPrefabs;
    private Dictionary<string, GameObject> minePrefabs;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        targets = new List<GameObject>();

        nextActionTime = Time.time;

        foodPrefabs = new Dictionary<string, GameObject>(){
            {"red", redFoodPrefab},
            {"green", greenFoodPrefab},
            {"blue", blueFoodPrefab},
        };

        minePrefabs = new Dictionary<string, GameObject>(){
            {"red", redMinePrefab},
            {"green", greenMinePrefab},
            {"blue", blueMinePrefab},
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool shouldResetCooldown = TakeAction(Time.time > nextActionTime);
            if (shouldResetCooldown)
            {
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

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"[PlayerController.OnTriggerExit] exit {other.transform.name}");

        targets.Remove(other.gameObject);
    }

    // TakeAction returns true if should reset the cooldown
    private bool TakeAction(bool notInCooldown)
    {
        bool shouldResetCooldown = false;

        foreach (GameObject target in targets)
        {
            if (target.CompareTag(cannonPlaceTag) && IsCarryingMine())
            {
                Debug.Log($"[PlayerController.TakeAction] interacting with cannon place: {target.name}");

                // TODO: call the cannon place method to interact with the cannon place

                ReleaseResource();

                animator.SetTrigger("building");
            }

            if (target.CompareTag(cannonTag) && IsCarryingFood())
            {
                Debug.Log($"[PlayerController.TakeAction] interacting with cannon: {target.name}");

                // TODO: call the cannon method to interact with the cannon
                ReleaseResource();

                animator.SetTrigger("reloading");
            }

            if (target.tag.EndsWith(treeTagSuffix) && notInCooldown && carryingObject == null)
            {
                Debug.Log($"[PlayerController.TakeAction] interacting with tree: {target.name}");

                // TODO: call the tree method to interact with the tree
                if (MockResourceTakeDamage(target))
                {
                    DestroyResource(target, "tree", target.tag.Split('_')[0]);
                }

                animator.SetTrigger("cutting");
                shouldResetCooldown = true;
            }

            if (target.tag.EndsWith(mineTagSuffix) && notInCooldown && carryingObject == null)
            {
                Debug.Log($"[PlayerController.TakeAction] interacting with mine: {target.name}");

                // TODO: call the mine method to interact with the mine
                if (MockResourceTakeDamage(target))
                {
                    DestroyResource(target, "mine", target.tag.Split('_')[0]);
                }

                animator.SetTrigger("mining");
                shouldResetCooldown = true;
            }
        }

        // we delete the object after the loop finished,
        // since we cannot delete object when there is an foreach loop using the object
        if (toBeDestroyedObject != null)
        {
            targets.Remove(toBeDestroyedObject);
            Destroy(toBeDestroyedObject);
            toBeDestroyedObject = null;
        }

        return shouldResetCooldown;
    }

    private void DestroyResource(GameObject o, string resourceType, string color = "")
    {
        Debug.Log($"[PlayerController.DestroyResource] destroy {resourceType} with color {color}");

        if (resourceType == "tree")
        {
            GameObject food = Instantiate(foodPrefabs[color], gameObject.transform);
            carryingObject = food;
        }
        else if (resourceType == "mine")
        {
            GameObject mine = Instantiate(minePrefabs[color], gameObject.transform);
            carryingObject = mine;
        }

        carryingObject.transform.localPosition = new Vector3(0, 2.2f, 0);

        toBeDestroyedObject = o;
    }

    private void ReleaseResource()
    {
        Destroy(carryingObject);
        carryingObject = null;
    }

    private bool IsCarryingMine()
    {
        return carryingObject && carryingObject.tag.EndsWith(mineTagSuffix);
    }

    private bool IsCarryingFood()
    {
        return carryingObject && carryingObject.tag.EndsWith(foodTagSuffix);
    }

    private bool MockResourceTakeDamage(GameObject o)
    {
        bool resourceHealthZero = Random.Range(0, 4) == 0;
        return resourceHealthZero;
    }
}

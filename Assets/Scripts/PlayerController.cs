using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // player controller settings
    public float movementSpeed;
    public float rotationSpeed;
    public float actionCooldown;
    public float resetBoundaryY;

    // tag definitions
    public const string cannonPlaceTag = "cannon_place";
    public const string cannonTagSuffix = "_cannon";
    public const string v1CannonTag = "v1_cannon";
    public const string v2CannonTag = "v2_cannon";
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

    // Sound Effects
    public AudioClip cuttingSoundEffect;
    public AudioClip miningSoundEffect;
    public AudioClip buildSuccessSoundEffect;
    public AudioClip loadBulletSoundEffect;

    private Vector2 movementInput;
    private bool takeActionInput;
    private Animator animator;
    private AudioSource audioSource;
    private List<GameObject> targets;
    private float nextActionTime;
    private GameObject carryingObject;
    private GameObject toBeDestroyedObject;
    private bool isTriggeringAnimation;

    // map generator
    private MapGenerator mapGenerator;

    // prefabs collections for convinience
    private Dictionary<string, GameObject> foodPrefabs;
    private Dictionary<string, GameObject> minePrefabs;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();

        targets = new List<GameObject>();

        nextActionTime = Time.time;

        carryingObject = null;
        toBeDestroyedObject = null;
        isTriggeringAnimation = false;

        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();

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
        if (takeActionInput)
        {
            bool shouldResetCooldown = TakeAction(Time.time > nextActionTime);
            if (shouldResetCooldown)
            {
                nextActionTime = Time.time + actionCooldown;
            }
        }

        // we delete the object after taking action finish
        // since we cannot delete object when there is an foreach loop using the object
        if (toBeDestroyedObject != null)
        {
            targets.Remove(toBeDestroyedObject);
            Destroy(toBeDestroyedObject);
            toBeDestroyedObject = null;
        }
    }

    void FixedUpdate()
    {
        if (isTriggeringAnimation) return;

        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);

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

        if (transform.position.y < resetBoundaryY)
        {
            transform.position = mapGenerator.ResetPlayerPosition();
            StartCoroutine(ReleaseResource());
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

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnTakeAction(InputAction.CallbackContext context)
    {
        takeActionInput = context.action.triggered;
    }

    // TakeAction returns true if should reset the cooldown
    private bool TakeAction(bool notInCooldown)
    {
        bool shouldResetCooldown = false;

        foreach (GameObject target in targets)
        {
            // Skipping null values
            if (target == null) continue;

            if (target.CompareTag(cannonPlaceTag) && IsCarryingMine("red") && !isTriggeringAnimation)
            {
                Debug.Log($"[PlayerController.TakeAction] interacting with cannon place: {target.name}");

                isTriggeringAnimation = true;
                StartCoroutine(TriggerBlockingAnimation("building"));

                CannonPlace cannonPlace = target.GetComponent<CannonPlace>();
                if (cannonPlace.GetMine())
                {
                    audioSource.PlayOneShot(buildSuccessSoundEffect);
                    StartCoroutine(ReleaseResource());
                    StartCoroutine(DestroyResource(target, "cannon_place"));
                }
            }

            if (target.tag.EndsWith(cannonTagSuffix) && IsCarryingFood() && !isTriggeringAnimation)
            {
                Debug.Log($"[PlayerController.TakeAction] interacting with cannon: {target.name}");

                CannonSenseShoot cannonSenseShoot = target.GetComponent<CannonSenseShoot>();
                if (cannonSenseShoot.LoadBullet(carryingObject))
                {
                    audioSource.PlayOneShot(loadBulletSoundEffect);

                    isTriggeringAnimation = true;
                    StartCoroutine(TriggerBlockingAnimation("reloading"));

                    StartCoroutine(ReleaseResource());
                }
            }

            if (target.CompareTag(v1CannonTag) && IsCarryingMine("blue") && !isTriggeringAnimation)
            {
                Debug.Log($"[PlayerController.TakeAction] interacing with v1 cannon: {target.name}");

                CannonUpgrade cannonUpgrade = target.GetComponent<CannonUpgrade>();
                if (cannonUpgrade.GetUpgradeMaterial())
                {
                    audioSource.PlayOneShot(buildSuccessSoundEffect);
                    StartCoroutine(TriggerBlockingAnimation("building"));
                    StartCoroutine(ReleaseResource());
                    StartCoroutine(DestroyResource(target, "cannon"));
                }
            }

            if (target.CompareTag(v2CannonTag) && IsCarryingMine("green") && !isTriggeringAnimation)
            {
                Debug.Log($"[PlayerController.TakeAction] interacing with v2 cannon: {target.name}");

                CannonUpgrade cannonUpgrade = target.GetComponent<CannonUpgrade>();
                if (cannonUpgrade.GetUpgradeMaterial())
                {
                    audioSource.PlayOneShot(buildSuccessSoundEffect);
                    StartCoroutine(TriggerBlockingAnimation("building"));
                    StartCoroutine(ReleaseResource());
                    StartCoroutine(DestroyResource(target, "cannon"));
                }
            }

            if (target.tag.EndsWith(treeTagSuffix) && notInCooldown && carryingObject == null && !isTriggeringAnimation)
            {
                Debug.Log($"[PlayerController.TakeAction] interacting with tree: {target.name}");

                audioSource.PlayOneShot(cuttingSoundEffect);
                isTriggeringAnimation = true;
                StartCoroutine(TriggerBlockingAnimation("cutting"));

                ResourceController resourceController = target.GetComponent<ResourceController>();
                if (resourceController.GetHit(1))
                {
                    StartCoroutine(DestroyResource(target, "tree", target.tag.Split('_')[0]));
                }

                shouldResetCooldown = true;
            }

            if (target.tag.EndsWith(mineTagSuffix) && notInCooldown && carryingObject == null && !isTriggeringAnimation)
            {
                Debug.Log($"[PlayerController.TakeAction] interacting with mine: {target.name}");

                audioSource.PlayOneShot(miningSoundEffect);
                isTriggeringAnimation = true;
                StartCoroutine(TriggerBlockingAnimation("mining"));

                ResourceController resourceController = target.GetComponent<ResourceController>();
                if (resourceController.GetHit(1))
                {
                    StartCoroutine(DestroyResource(target, "mine", target.tag.Split('_')[0]));
                }

                shouldResetCooldown = true;
            }
        }

        return shouldResetCooldown;
    }

    private IEnumerator DestroyResource(GameObject o, string resourceType, string color = "")
    {
        yield return new WaitUntil(() => (!isTriggeringAnimation));

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
        else if (resourceType == "cannon_place")
        {
            o.GetComponent<CannonPlace>().ConstructCannon();
        }
        else if (resourceType == "cannon")
        {
            o.GetComponent<CannonUpgrade>().UpgradeCannon();
        }

        if (carryingObject != null)
        {
            carryingObject.transform.localPosition = new Vector3(0, 2.2f, 0);
        }

        toBeDestroyedObject = o;
    }

    private IEnumerator TriggerBlockingAnimation(string triggerName)
    {
        Debug.Log("[PlayerController.TriggerBlockingAnimation]: trigger starts");

        animator.SetTrigger(triggerName);

        yield return new WaitForSeconds(0.417f);

        isTriggeringAnimation = false;
        Debug.Log("[PlayerController.TriggerBlockingAnimation]: trigger ends");
    }

    private IEnumerator ReleaseResource()
    {
        // To set null early
        GameObject tempCarryingObject = carryingObject;
        carryingObject = null;

        yield return new WaitUntil(() => (!isTriggeringAnimation));

        Destroy(tempCarryingObject);
    }

    private bool IsCarryingMine(string color = "")
    {
        return carryingObject && carryingObject.tag.EndsWith(mineTagSuffix) && (color == "" || carryingObject.CompareTag(minePrefabs[color].tag));
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

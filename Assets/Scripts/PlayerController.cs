using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // player controller settings
    public float movementSpeed;
    public float rotationSpeed;

    // tag definitions
    public const string towerPlaceTag = "towerPlace";
    public const string towerTag = "tower";
    public const string treeTag = "tree";

    private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
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
        Debug.Log($"[PlayerController.OnTriggerEnter] hit {other.transform.name}");

        switch (other.tag)
        {
            case towerPlaceTag:
                break;
            case towerTag:
                break;
            case treeTag:
                break;
            default:
                break;
        }
    }
}

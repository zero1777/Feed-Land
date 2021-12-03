using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;

    private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
    }
 
    void FixedUpdate() {
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //for NavMeshAgent

public class MonsterControl : MonoBehaviour
{
    // public float randomForce = 200.0f;
    public GameObject redFood; // tomatoes
    public GameObject blueFood; // bluberries
    public GameObject greenFood; // bananas
    public AudioClip eatSound;
    public AudioClip successSound;
    public AudioClip roarSound;
    public AudioClip roleHurtSound;
    public GameObject unicorn;

    // store path
    public List<Vector3> storedPath;
    private NavMeshAgent nma = null;

    // use to decide route
    private Vector3 initDestination;
    private Vector3 newDirection;
    private Vector3 newDestination;
    private bool canMove;
    public int bufferSize = 15;


    // how many numbers of food will be generated, default is 5 
    public int redFoodNum = 5;
    public int blueFoodNum = 5;
    public int greenFoodNum = 5;

    public float attackDistance;

    private Animator animator;
    private List<string> foodList = new List<string> { "red_food", "blue_food", "green_food" };
    private AudioSource audioPlayer;

    void Start()
    {
        canMove = true;
        audioPlayer = gameObject.GetComponent<AudioSource>();

        int randomRedFood = Random.Range(1, redFoodNum);
        if (gameObject.tag == "red_monster")
        {
            for (int i = 0; i < randomRedFood; i++)
            {
                GameObject redFruit = Instantiate(redFood, gameObject.transform);
                redFruit.transform.localPosition = new Vector3(0, 2.0f + 0.5f * i, 0);
            }
        }


        int randomBlueFood = Random.Range(1, blueFoodNum);
        if (gameObject.tag == "blue_monster")
        {
            for (int i = 0; i < randomBlueFood; i++)
            {
                GameObject blueFruit = Instantiate(blueFood, gameObject.transform);
                blueFruit.transform.localPosition = new Vector3(0, 2.0f + 0.6f * i, 0);
            }
        }

        int randomGreenFood = Random.Range(1, greenFoodNum);
        if (gameObject.tag == "green_monster")
        {
            for (int i = 0; i < randomBlueFood; i++)
            {
                GameObject greenFruit = Instantiate(greenFood, gameObject.transform);
                greenFruit.transform.localPosition = new Vector3(0, 2.0f + 0.8f * i, 0);
            }
        }

        animator = GetComponent<Animator>();
        // this set the monster to be in idle state
        animator.SetFloat("Speed", 0);
        nma = this.GetComponent<NavMeshAgent>();
        // init route;
        initDestination = new Vector3(Mathf.Round(this.gameObject.transform.position.x), this.gameObject.transform.position.y, Mathf.Round(this.gameObject.transform.position.z));
        newDirection = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 2));
        newDestination = initDestination + newDirection;
        InitFirst15Path();
        //InvokeRepeating("MovedRandomly", 0.0f, 0.5f);
    }

    void Update()
    {
        // check distance, if the monster and the role is close, monster will use roar to warn players
        float distance = (unicorn.transform.position - transform.position).magnitude;

        // distance can be changed in the future
        if (distance < attackDistance)
        {
            animator.SetBool("Attack", true);
            // PlaySoundEffect(roarSound);
        }
        else
        {
            animator.SetBool("Attack", false);
        }

        // dynamic generate route list
        if (storedPath.Count < bufferSize && canMove == true)
        {
            newDirection = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 2));
            newDestination += newDirection;
            // store path
            storedPath.Add(newDestination);
        }
        // check role is moving or not
        else if (nma.hasPath == false && canMove == true)
        {
            nma.SetDestination(storedPath[0]);
            storedPath.RemoveAt(0);
        }
        else
        {
            float randomSpeed = Random.Range(0.1f, 1.0f);
            animator.SetFloat("Speed", randomSpeed);
        }

        // will be replaced after (eat animation will be triggered by the food shoot)
        if (Input.GetKey(KeyCode.E))
        {
            animator.SetTrigger("Eat");
            GameObject eattenFood = transform.GetChild(transform.childCount - 1).gameObject;
            GameObject lastTwoEattenFood = transform.GetChild(transform.childCount - 2).gameObject;
            // double checked to make sure didn't delete wrong object
            if (foodList.Contains(eattenFood.tag))
            {
                PlaySoundEffect(eatSound);
                Destroy(eattenFood, 0.5f);

                // if the food is the last one, after eatten the monster should feel satisfy and disappear
                if (!foodList.Contains(lastTwoEattenFood.tag))
                {
                    // stop chasing the unicorn
                    canMove = false;
                    nma.isStopped = true;
                    animator.SetBool("Satisfied", true);
                    PlaySoundEffect(successSound);
                    Destroy(gameObject, 3.0f);
                }
            }
        }
    }


    /* void MovedRandomly()
    {
        float randomNum = Random.Range(-1.0f, 1.0f);
        float absSpeed = Mathf.Abs(randomNum);
        animator.SetFloat("Speed", absSpeed);
        if (randomNum > 0.0f)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.forward * randomForce);
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(-1 * Vector3.back * randomForce);
        }
    } */

    void PlaySoundEffect(AudioClip soundEffect)
    {
        audioPlayer.PlayOneShot(soundEffect);
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

    public void AppendPath(Vector3 newPath)
    {
        storedPath.Add(newPath);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "role")
        {
            other.gameObject.GetComponent<RoleController>().TakeDamage(1);
            if (other.gameObject.GetComponent<RoleController>().currentHealth >= 0)
            {
                PlaySoundEffect(roleHurtSound);
            }
        }
    }

    // // draw the checking radius
    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.white;
    //     Gizmos.DrawWireSphere(transform.position, 5.0f);
    // }
}

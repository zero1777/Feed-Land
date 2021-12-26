using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // use to decide route
    public float monsterMovingSpeed = 5.0f;
    public float monsterRotSpeed = 0.1f;
    public MapGenerator mapGenerator;
    private Vector3 targetPosition;
    private Vector3 lookAtTarget;
    private Quaternion roleRot;
    private bool canMove;
    private bool isMoving = false;
    private Rigidbody rb;


    // how many numbers of food will be generated, default is 5 
    public int redFoodNum = 5;
    public int blueFoodNum = 5;
    public int greenFoodNum = 5;

    public float attackDistance;

    private Animator animator;
    private List<string> foodList = new List<string> { "red_food", "blue_food", "green_food" };
    private AudioSource audioPlayer;

    IEnumerator Start()
    {
        //init para
        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
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
        //InvokeRepeating("MovedRandomly", 0.0f, 0.5f);
        yield return new WaitUntil(() => mapGenerator.isInitialized);
        GetMapPath();
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

        // check is moving 
        if (isMoving && canMove == true)
            MoveRole();
        // check role is  not
        // dynamic generate route list
        if (isMoving == false && canMove == true)
        {
            if (storedPath.Count > 0)
            {
                targetPosition = storedPath[0];
                SetTargetPosition();
                animator.SetFloat("Speed", 1.0f);
                storedPath.RemoveAt(0);
            }

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

        if (foodList.Contains(other.gameObject.tag))
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
                    animator.SetBool("Satisfied", true);
                    PlaySoundEffect(successSound);
                    Destroy(gameObject, 3.0f);
                }
            }

        }
    }

    // // draw the checking radius
    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.white;
    //     Gizmos.DrawWireSphere(transform.position, 5.0f);
    // }

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
        transform.rotation = Quaternion.Slerp(transform.rotation, roleRot, monsterRotSpeed * Time.fixedDeltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, monsterMovingSpeed * Time.fixedDeltaTime);
        if (transform.position == targetPosition)
        {
            isMoving = false;
        }
    }
}

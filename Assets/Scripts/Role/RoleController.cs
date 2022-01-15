using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleController : MonoBehaviour
{
    public int maxHealth = 5;
    public float roleMovingSpeed = 2.0f;
    public float roleRotSpeed = 0.1f;
    public int currentHealth;

    public HealthBar healthBar;

    public enum Direction
    {
        UP,
        Down,
        Left,
        Right
    };

    public List<GameObject> enemies = new List<GameObject>();
    public List<Vector3> storedPath;

    // map generator
    private MapGenerator mapGenerator;
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
    private int nextGetMapIdx;
    // generate monster
    private UITimer uITimer;
    private bool canGenerateMonster = true;
    private bool canHugeWave = false;
    private int currentHugeWave = 1;
    // huge wave setting
    private int hugeWaveInterval = 100;
    private int hugeWaveDifficulty = 3;
    private float prepareTime = 10.0f;
    [SerializeField] private float monsterSpawnWait;
    private float monsterLeastSpawnWait = 5.0f;
    private float monsterMostSpawnWait = 10.0f;
    // sound effect
    private AudioSource audioPlayer;
    public AudioClip hugeWaveSound;
    // visual effect
    private GameObject hugeWaveText;

    IEnumerator Start()
    {
        // init para
        nextGetMapIdx = 0;
        uITimer = GameObject.Find("UITimer").GetComponent<UITimer>();
        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        audioPlayer = GetComponent<AudioSource>();
        hugeWaveText = GameObject.Find("HugeWaveText");
        hugeWaveText.SetActive(false);
        rb = GetComponent<Rigidbody>();
        canMove = true;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        animator = GetComponent<Animator>();

        mainCamera = GameObject.Find("Main Camera");
        light = GameObject.Find("Directional Light");

        // generate Enemy after 1 second, every 10 second generate another monster
        //InvokeRepeating("GenerateEnemy", 5.0f, 7.0f);
        StartCoroutine(MonsterSpawner());
        // wait mapGenerator has terminated own "Start" life cycle
        yield return new WaitUntil(() => mapGenerator.isInitialized);
        GetMapPath();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (nextGetMapIdx < mapGenerator.GetCurrentMapIdx()) GetMapPath();
    }

    void FixedUpdate()
    {
        /* Debug only
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TakeDamage(1);
        }
        */

        // check is moving 
        if (isMoving && canMove == true)
            MoveRole();

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
        // decide difficulty
        DecideDifficulty();
        // spwan monster
        monsterSpawnWait = Random.Range(monsterLeastSpawnWait, monsterMostSpawnWait);
        // check Huge Wave
        CheckHugeWave();
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
        // for (int i=0; i < mapGenerator.mapNum; i++)
        // {
        //     for (int j = 0; j < mapGenerator.GetPath(i).Count; j++)
        //     {
        //         storedPath.Add(mapGenerator.GetPath(i)[j]);
        //         //Debug.Log(mapGenerator.GetPath(i)[j]);
        //     }
        // }
        for ( ; nextGetMapIdx < mapGenerator.GetCurrentMapIdx(); nextGetMapIdx++)
        {
            for (int j = 0; j < mapGenerator.GetPath(nextGetMapIdx).Count; j++)
            {
                storedPath.Add(mapGenerator.GetPath(nextGetMapIdx)[j]);
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
    // normal spawn monster
    private IEnumerator MonsterSpawner()
    {
        // difficult of generate monster
        yield return new WaitForSeconds(prepareTime);
        while (canGenerateMonster == true)
        {
            GenerateEnemy();
            yield return new WaitForSeconds (monsterSpawnWait);
        }
        
    }
    // wave spawn monster
    private IEnumerator MonsterSpawnerWave(int spawnNumber)
    {
        int hadSpawn = 0;
        // difficult of generate monster
        while (canHugeWave == true)
        {
            if (hadSpawn >= (spawnNumber * hugeWaveDifficulty))
            {
                canHugeWave = false;
                hugeWaveText.SetActive(false);
                break;
            }
            GenerateEnemy();
            hadSpawn++;
            yield return new WaitForSeconds(Random.Range(0.1f, 1.0f));
        }

    }
    // used to decide difficulty
    private void DecideDifficulty()
    {
        if (uITimer.timerFloat < 1800)
        {
            monsterLeastSpawnWait = 2100 / (uITimer.timerFloat + 300);
            monsterMostSpawnWait = 3600 / (uITimer.timerFloat + 300);
        }
        else
        {
            monsterLeastSpawnWait = 1.0f;
            monsterMostSpawnWait = 1.5f;
        }
    }
    // check huge wave
    private void CheckHugeWave()
    {
        if (canHugeWave == false && Mathf.Floor((uITimer.timerFloat/ hugeWaveInterval)) == currentHugeWave)
        {
            canHugeWave = true;
            Debug.Log("Huge Wave incoming!!!");
            hugeWaveText.SetActive(true);
            audioPlayer.PlayOneShot(hugeWaveSound);
            StartCoroutine(MonsterSpawnerWave(currentHugeWave));
            currentHugeWave++;
        }

    }
}

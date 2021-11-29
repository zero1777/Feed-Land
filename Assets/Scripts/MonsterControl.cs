using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : MonoBehaviour
{
	public float randomForce = 200.0f;
	public string foodType;
	public GameObject redFood; // tomatoes
	public GameObject blueFood; // bluberries
	public GameObject greenFood; // bananas
	public AudioClip eatSound;
	public AudioClip successSound;

	// how many numbers of food will be generated, default is 5 
	public int redFoodNum = 5;
	public int blueFoodNum = 5;
	public int greenFoodNum = 5;

	private Animator animator;
	private List<string> foodList = new List<string> { "red_food", "blue_food", "green_food" };
	private AudioSource audioPlayer;

	void Start()
	{
		audioPlayer = gameObject.GetComponent<AudioSource>();

		int randomRedFood = Random.Range(1, redFoodNum);
		if (gameObject.tag == "red_monster")
		{
			for (int i = 0; i < randomRedFood; i++)
			{
				GameObject redFruit;
				redFruit = Instantiate(redFood, gameObject.transform);
				redFruit.transform.localPosition = new Vector3(0, 2.0f + 0.5f*i, 0);
			}
		}


		int randomBlueFood = Random.Range(1, blueFoodNum);
		if (gameObject.tag == "blue_monster")
		{
			for (int i = 0; i < randomBlueFood; i++)
			{
				GameObject blueFruit;
				blueFruit = Instantiate(blueFood, gameObject.transform);
				blueFruit.transform.localPosition = new Vector3(0, 2.0f + 0.6f*i, 0);
			}
		}

		GameObject greenFruit;
		int randomGreenFood = Random.Range(1, greenFoodNum);
		if (gameObject.tag == "green_monster")
		{
			for (int i = 0; i < randomBlueFood; i++)
			{
				greenFruit = Instantiate(greenFood, gameObject.transform);
				greenFruit.transform.localPosition = new Vector3(0, 2.0f + 0.8f*i, 0);
			}
		}

		animator = GetComponent<Animator>();
		animator.SetFloat("animation", 12); // (this set monster animation to run)
		InvokeRepeating("MovedRandomly", 0.0f, 0.5f);
	}

	void Update()
	{
		// will be replaced after (eat animation will be triggered by the food shoot)
		if (Input.GetKey(KeyCode.E))
		{
			animator.SetTrigger("Eat");
			GameObject eattenFood;
			GameObject lastTwoEattenFood;
			eattenFood = transform.GetChild(transform.childCount - 1).gameObject;
			lastTwoEattenFood = transform.GetChild(transform.childCount - 2).gameObject;
			// double checked to make sure didn't delete wrong object
			if (foodList.Contains(eattenFood.tag))
			{
				PlaySoundEffect(eatSound);
				Destroy(eattenFood, 0.5f);

				// if the food is the last one, after eatten the monster should feel satisfy and disappear
				if (!foodList.Contains(lastTwoEattenFood.tag))
				{
					animator.SetBool("Satisfied", true);
					PlaySoundEffect(successSound);
					Destroy(gameObject, 3.0f);
				}
			}
		}
	}


	// will be replaced after (follow the player)
	void MovedRandomly()
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
	}

	void PlaySoundEffect(AudioClip soundEffect)
	{
		audioPlayer.PlayOneShot(soundEffect);
	}
}

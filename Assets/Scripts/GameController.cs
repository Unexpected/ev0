using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameController : MonoBehaviour {

	private GameController instance; 
	private BoardController board; 

	public bool gameOver;
	public GameObject gameOverText; 


	//Awake is always called before any Start functions
	void Awake() {
		//Check if instance already exists
		if (instance == null)
			//if not, set instance to this
			instance = this;
		//If instance already exists and it's not this:
		else if (instance != this)
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);    

		//Get a component reference to the attached BoardManager script
		board = GetComponent<BoardController>();

		//Call the InitGame function to initialize the first level 
		board.InitGame();
	}

	public void EndGame() {
		gameOver = true; 
		gameOverText.SetActive (true);

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (gameOver && Input.GetMouseButtonDown (0)) {
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
		}
		if (Input.GetKey("escape"))
			Application.Quit();
		
	}
}

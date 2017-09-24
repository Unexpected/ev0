using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameController : MonoBehaviour {

	private GameController instance; 
	private BoardController board; 
	public GameObject settingsPanel;
	public GameObject modalPanel;

	public bool gameOver;
	public bool gamePaused;
	public GameObject gameOverText; 

	private float doubleTapTimer = 0.0f;
	private int tapCount = 0; 

	private float fingerStartTime  = 0.0f;
	private Vector2 fingerStartPos = Vector2.zero;

	private bool isSwipe = false;
	private float minSwipeDist  = 50.0f;
	private float maxSwipeTime = 0.5f;

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
			StartNewGame ();
		}

		if (handleMenuButton ())
			return; 

		if (gamePaused) {
			handleMenuInput (); 
		} else {
			if (board.IsMoving()) {
				return; 
			}
			handleGameInput ();
		}
	}

	void handleMenuInput() {

	}

	void handleGameInput() {
		if (handleKeyboard ()) {
			return; 
		}

		// Detect double touch
		if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began)
			tapCount++;
		if (tapCount > 0)
			doubleTapTimer += Time.deltaTime;
		if (tapCount >= 2) {
			board.Top ();
			doubleTapTimer = 0.0f;
			tapCount = 0;
		}
		if (doubleTapTimer > 0.5f) {
			doubleTapTimer = 0f;
			tapCount = 0;
		}

		if (Input.touchCount > 0) {
			foreach (Touch touch in Input.touches) {
				switch (touch.phase) {
				case TouchPhase.Began:
					/* this is a new touch */
					isSwipe = true;
					fingerStartTime = Time.time;
					fingerStartPos = touch.position;
					break;

				case TouchPhase.Canceled:
					/* The touch is being canceled */
					isSwipe = false;
					break;

				case TouchPhase.Ended:
					float gestureTime = Time.time - fingerStartTime;
					float gestureDist = (touch.position - fingerStartPos).magnitude;

					if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist) {
						Vector2 direction = touch.position - fingerStartPos;
						Vector2 swipeType = Vector2.zero;

						if (Mathf.Abs (direction.x) > Mathf.Abs (direction.y)) {
							// the swipe is horizontal:
							swipeType = Vector2.right * Mathf.Sign (direction.x);
						} else {
							// the swipe is vertical:
							swipeType = Vector2.up * Mathf.Sign (direction.y);
						}

						if (swipeType.x != 0.0f) {
							tapCount = 0;
							doubleTapTimer = 0f;
							if (swipeType.x > 0.0f) {
								board.Right ();
							} else {
								board.Left ();
							}
						}

						if (swipeType.y != 0.0f) {
							tapCount = 0;
							doubleTapTimer = 0f;
							if (swipeType.y > 0.0f) {
								board.Top ();
							} else {
								board.Down ();
							}
						}
					}
					break;
				}
			}
		}
	}

	bool handleKeyboard() {
		if (Input.GetKey ("escape")) {
			Quit ();
			return true; 
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow)){
			board.Left ();
			return true; 
		}
		if (Input.GetKeyUp (KeyCode.RightArrow)){
			board.Right ();
			return true; 
		}
		if (Input.GetKeyUp (KeyCode.UpArrow)){
			board.Top ();
			return true; 
		}
		if (Input.GetKeyUp (KeyCode.DownArrow)){
			board.Down ();
			return true; 
		}
		return false;
	}

	public void Quit() {
		Application.Quit ();
	}

	bool handleMenuButton() {
		if (Input.GetMouseButtonDown (0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			RaycastHit2D hitInformation = Physics2D.Raycast(mousePos, Camera.main.transform.forward);
			if (hitInformation.collider != null) {
				if (hitInformation.collider.tag == "SettingsButton" || hitInformation.collider.tag == "ContinueButton") {
					toggleMenu ();
					return true;
				}
				if (hitInformation.collider.tag == "QuitButton") {
					Quit ();
					return true;
				}
				if (hitInformation.collider.tag == "NewGameButton" || hitInformation.collider.tag == "NoButton") {
					toggleModal ();
					return true;
				}
				if (hitInformation.collider.tag == "YesButton") {
					StartNewGame ();
					return true;
				}
			}
		}
		return false; 
	}

	void toggleMenu() {
		gamePaused = !gamePaused;
		settingsPanel.SetActive (gamePaused);
	}

	void toggleModal() {
		modalPanel.SetActive (!modalPanel.activeSelf);
	}

	void StartNewGame() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
}

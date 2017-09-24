using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour {

	public int WIDTH = 6; 
	public int HEIGHT = 9; 

	public int maxLevel = 0;
	public int score = 0;

	public GameObject[] dropPrefabs; 
	public GameObject spawn; 
	public Text scoreText; 

	List<GameObject> unlocked;

	public Drop[][] grid; 

	private GameController gameController;

	private Drop activeDrop1; 
	private Drop activeDrop2; 

	private System.Random rnd = new System.Random (); 

	// Use this for initialization
	void Start () {
		gameController = GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameController.gameOver || gameController.gamePaused) {
			return;
		}
		if (IsMoving()) {
			return; 
		}
		if (activeDrop1 == null && activeDrop2 == null) {
			if (Fall ()) {
				return; 
			}
			if (Evolve ()) {
				return; 
			}
			// check game over
			if (isGameOver ()) {
				gameController.EndGame ();
			} else {
				CreateDrops ();
			}
		}
	}

	public bool IsMoving() {
		for (int y = 0; y < HEIGHT; y++) {
			for (int x = 0; x < WIDTH; x++) {
				if (grid [x] [y] != null && grid [x] [y].moving) {
					return true;
				}
			}
		}
		return false; 
	}

	public void InitGame() {
		maxLevel = 0;
		score = 0;
		grid = new Drop[WIDTH][];
		for (int x = 0; x < WIDTH; x++) {
			grid[x] = new Drop[HEIGHT+3]; 
		}
		InitUnlockedElements ();
		CreateDrops ();
	}

	void InitUnlockedElements() {
		unlocked = new List<GameObject> ();
		for (int i = 0; i < 13; i++) {
			GameObject drop = Instantiate (dropPrefabs [i], new Vector3 (7, i*0.9f, 0), Quaternion.identity);
			drop.GetComponent<Drop> ().pos = new Vector3 (7, i*0.9f, 0);
			if (i > 0)
				drop.GetComponent<SpriteRenderer> ().color = Color.black;
			unlocked.Add (drop);
		}
	}

	void CreateDrops() {
		int leftLevel = rnd.Next (maxLevel + 1);
		int rightLevel = rnd.Next (maxLevel + 1);
		GameObject leftDrop = Instantiate (dropPrefabs[leftLevel], new Vector3(0, 9.5f, 0), Quaternion.identity);
		GameObject rightDrop = Instantiate (dropPrefabs[rightLevel], new Vector3(1, 9.5f, 0), Quaternion.identity);

		activeDrop1 = leftDrop.GetComponent<Drop> ();
		activeDrop1.Initialize (leftLevel, 0, 9.5f);

		activeDrop2 = rightDrop.GetComponent<Drop> ();
		activeDrop2.Initialize (rightLevel, 1, 9.5f);
	}

	public void Left() {
		if (activeDrop1.pos.x == 0 || activeDrop2.pos.x == 0 )
			return; 
		activeDrop1.Left (); 
		activeDrop2.Left (); 
	}

	public void Right() {
		if (activeDrop1.pos.x == WIDTH - 1 || activeDrop2.pos.x == WIDTH - 1)
			return; 
		activeDrop1.Right (); 
		activeDrop2.Right ();
	}

	public void Top() {
		if (activeDrop1.pos.x == activeDrop2.pos.x) {
			// Drop 1 on top of drop 2
			Drop tmp = activeDrop2;
			activeDrop2 = activeDrop1;
			activeDrop2.pos.y -= 1;
			activeDrop2.pos.x += 1;
			activeDrop1 = tmp;
		} else {
			activeDrop1.pos.y += 1;
			activeDrop2.pos.x -= 1;
		}
	}

	public void Down() {
		int y1 = HEIGHT + 1; 
		int y2 = HEIGHT + 1;
		if (activeDrop1.pos.x == activeDrop2.pos.x) {
			y1 = HEIGHT + 2;
		} 
		Drop[] leftCol = grid[(int) activeDrop1.pos.x]; 
		leftCol[y1] = activeDrop1; 
		leftCol [y1].pos.y = y1;
		activeDrop1 = null; 

		Drop[] rightCol = grid[(int) activeDrop2.pos.x]; 
		rightCol[y2] = activeDrop2; 
		rightCol[y2].pos.y  = y2; 
		activeDrop2 = null; 
	}

	bool Fall() {
		bool fell = false; 
		List<Drop> falling = new List<Drop> ();
		for (int x = 0; x < WIDTH; x++) {
			Drop[] column = grid [x];
			int floor = 0;
			for (int y = 0; y < HEIGHT + 3; y++) {
				if (column [y] != null) {
					if (floor != y) {
						// Fall
						column[floor] = column[y];
						column [floor].pos.y = floor;
						column [y] = null;
						fell = true;
						falling.Add (column [floor]);
					}
					floor ++;
				}
			}
		}

		return fell;
	}

	private List<Drop> getGroup(Drop drop) {
		List<Drop> group = new List<Drop> ();
		group.Add (drop); 

		List<Drop> todo = new List<Drop> ();
		todo.Add (drop);

		HashSet<int> done = new HashSet<int> ();

		while (todo.Count > 0) {
			Drop current = todo [0];
			todo.RemoveAt (0);
			int x = (int) current.pos.x; 
			int y = (int) current.pos.y; 
			if (y > 0 && grid[x][y-1] != null) {
				Drop down = grid[x][y-1];
				if (!done.Contains (x + (y-1)*WIDTH) && down.level == drop.level) {
					group.Add (down);
					todo.Add (down);
				}
			}
			if (x > 0 && grid[x-1][y] != null) {
				Drop left = grid[x-1][y];
				if (!done.Contains (x-1+y*WIDTH) && left.level == drop.level) {
					group.Add (left);
					todo.Add (left);
				}
			}
			if (x < WIDTH - 1 && grid[x+1][y] != null) {
				Drop right = grid[x+1][y];
				if (!done.Contains (x+1+y*WIDTH) && right.level == drop.level) {
					group.Add (right);
					todo.Add (right);
				}
			}
			if (y < HEIGHT && grid[x][y+1] != null) {
				Drop top = grid[x][y+1];
				if (!done.Contains (x+(y+1)*WIDTH) && top.level == drop.level) {
					group.Add (top);
					todo.Add (top);
				}
			}
			done.Add (x + y * WIDTH);
		}
		return group; 
	}

	bool Evolve() {
		bool evolved = false; 
		List<int> marked = new List<int> ();
		List<List<Drop>> evolving = new List<List<Drop>> ();

		// find evolving groups
		for (int y = 0; y < HEIGHT; y++) {
			for (int x = 0; x < WIDTH; x++) {
				Drop drop = grid [x] [y];
				if (drop == null || marked.Contains (x + y * WIDTH)) {
					continue;
				}
				List<Drop> group = getGroup (drop);
				foreach (Drop d in group) {
					marked.Add (((int)d.pos.x) + ((int)d.pos.y) * WIDTH);
				}
				if (group.Count >= 3) {
					evolving.Add (group);
				}
			}
		}

		foreach (List<Drop> group in evolving) {
			int botLeftX = WIDTH; 
			int botLeftY = HEIGHT;
			int level = 0;
			foreach (Drop drop in group) {
				int x = (int)drop.pos.x;
				int y = (int) drop.pos.y;
				level = drop.level;
				if (y < botLeftY || y == botLeftY && x < botLeftX) {
					botLeftX = x; 
					botLeftY = y; 
				}
				grid [x] [y] = null;
			}
			score += (level + 1) * 10 * group.Count; 
			scoreText.text = "Score: " + score; 

			foreach (Drop drop in group) {
				drop.pos.x = botLeftX;
				drop.pos.y = botLeftY;
				drop.destroyed = true;
			}

			GameObject spawnParticle = Instantiate (spawn, new Vector3(botLeftX, botLeftY, 0), Quaternion.identity);
			Destroy(spawnParticle, 2);
			GameObject newDrop = Instantiate (dropPrefabs[level+1], new Vector3(botLeftX, botLeftY, 0), Quaternion.identity);
			grid[botLeftX][botLeftY] = newDrop.GetComponent<Drop> ();
			grid[botLeftX][botLeftY].Initialize (level+1, botLeftX, botLeftY);
			if (level + 1 > maxLevel) {
				maxLevel = level + 1;
				GameObject unlockedDrop = unlocked [maxLevel];
				unlockedDrop.GetComponent<SpriteRenderer> ().color = Color.white;
				GameObject unlockParticle = Instantiate (spawn, new Vector3(unlockedDrop.transform.position.x, unlockedDrop.transform.position.y, 0), Quaternion.identity);
				Destroy(unlockParticle, 2);
			}

			evolved = true; 
		}

		// for each group, mark bottom-left pos
		// destroy prev group
		// spawn a new drop with level + 1 at bot left pos
		// increase max level if needed

		// fall everything and check for evolution
		return evolved;
	}

	bool isGameOver() {
		for (int x = 0; x < WIDTH; x++) {
			if (grid[x][HEIGHT-1] != null)
				return true; 
		}		
		return false; 
	}


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour {

	public int level = 0; 
	public Vector3 pos = new Vector3 (0, 0, 0);
	public bool moving = false; 
	public bool destroyed = false; 

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		this.transform.position = Vector3.MoveTowards (this.transform.position, pos, Time.deltaTime * 10f);
		moving = (this.pos.x != this.transform.position.x || this.pos.y != this.transform.position.y);
		if (!moving && destroyed) {
			Destroy (this.gameObject);
		}
	}

	public void Initialize(int level, float x, float y) {
		this.level = level;
		this.pos = new Vector3(x, y, 0);
		this.transform.position = this.pos;
	}

    public void Left()
    {
		this.pos.x -= 1;
    }

	public void Right() 
    {
		this.pos.x += 1;
    }
}

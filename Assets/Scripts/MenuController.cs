using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	public GameObject fadingText; 
	bool fadingOut; 

	// Use this for initialization
	void Start () {
		fadingText.GetComponent<Text> ().canvasRenderer.SetAlpha (0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		// fade to transparent over 500ms.
		Text t = fadingText.GetComponent<Text> ();
		
		if (fadingOut) {
			t.CrossFadeAlpha(0.0f, 0.5f, false);
			if (t.canvasRenderer.GetAlpha () < 0.05f) {
				fadingOut = false; 
			}
		} else {
			t.CrossFadeAlpha(1.0f, 0.5f, false);
			if (t.canvasRenderer.GetAlpha () > 0.9f) {
				fadingOut = true; 
			}
		}

		if (Input.GetMouseButtonDown (0)) {
			Debug.Log (SceneManager.GetActiveScene ().name);
			SceneManager.LoadScene (SceneManager.GetSceneByName ("MainScene").buildIndex);
		}
	}
}

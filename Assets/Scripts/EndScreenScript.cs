using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenScript : MonoBehaviour {

	// Use this for initialization
	public void LoadMainMenu() {
		SceneManager.LoadScene("Menu");
	}
	
	// Update is called once per frame
	public void ReLoadScene() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}

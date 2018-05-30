using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt("Level1Gems") == 0) {
			GameObject.Find("Level1Gem").SetActive(false);
		}
		if (PlayerPrefs.GetInt("Level2Gems") == 0) {
			GameObject.Find("Level2Gem").SetActive(false);
		}
		if (PlayerPrefs.GetInt("Level3Gems") == 0) {
			GameObject.Find("Level3Gem").SetActive(false);
		}
	}
	
	// Update is called once per frame
	public void LoadLevel(int i) {
		SceneManager.LoadScene("Level" + i.ToString());
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt("Level1") == 0) {
			transform.Find("coin1").gameObject.GetComponentInChildren<Renderer>().material.color = Color.black;
		}

		if (PlayerPrefs.GetInt("Level2") == 0) {
			transform.Find("coin2").gameObject.GetComponentInChildren<Renderer>().material.color = Color.black;
		}

		if (PlayerPrefs.GetInt("Level3") == 0) {
			transform.Find("coin3").gameObject.GetComponentInChildren<Renderer>().material.color = Color.black;
		}
	}
	
}

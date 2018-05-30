using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelsScript : MonoBehaviour {

	// Use this for initialization
	public TMP_FontAsset FontNormal;
    public TMP_FontAsset FontGlow;
	public void GetPrefs () {
		if (PlayerPrefs.GetInt("Level1Gems") == 0) {
			GameObject.Find("Level1Gem").SetActive(false);
		}
		if (PlayerPrefs.GetInt("Level2Gems") == 0) {
			GameObject.Find("Level2Gem").SetActive(false);
		}
		if (PlayerPrefs.GetInt("Level3Gems") == 0) {
			GameObject.Find("Level3Gem").SetActive(false);
		}

		if (PlayerPrefs.GetInt("Level1") == 1) {
			transform.Find("Level1Button").GetComponentInChildren<TextMeshProUGUI>().font = FontGlow;
		} else {
			transform.Find("Level1Button").GetComponentInChildren<TextMeshProUGUI>().font = FontNormal;
		}
		if (PlayerPrefs.GetInt("Level2") == 1) {
			transform.Find("Level2Button").GetComponentInChildren<TextMeshProUGUI>().font = FontGlow;
		} else {
			transform.Find("Level2Button").GetComponentInChildren<TextMeshProUGUI>().font = FontNormal;
		}
		if (PlayerPrefs.GetInt("Level3") == 1) {
			transform.Find("Level3Button").GetComponentInChildren<TextMeshProUGUI>().font = FontGlow;
		} else {
			transform.Find("Level3Button").GetComponentInChildren<TextMeshProUGUI>().font = FontNormal;
		}
	}
	
	// Update is called once per frame
	public void LoadLevel(int i) {
		SceneManager.LoadScene("Level" + i.ToString());
	}
}

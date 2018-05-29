using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandlerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(!PlayerPrefs.HasKey("Level1Gems")) PlayerPrefs.SetInt("Level1Gems",0);
		if(!PlayerPrefs.HasKey("Level2Gems")) PlayerPrefs.SetInt("Level2Gems",0);
		if(!PlayerPrefs.HasKey("Level3Gems")) PlayerPrefs.SetInt("Level3Gems",0);
		if(!PlayerPrefs.HasKey("Level1")) PlayerPrefs.SetInt("Level1",0);
		if(!PlayerPrefs.HasKey("Level2")) PlayerPrefs.SetInt("Level2",0);
		if(!PlayerPrefs.HasKey("Level3")) PlayerPrefs.SetInt("Level3",0);
	}
}

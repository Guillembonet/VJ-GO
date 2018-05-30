using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject.Find("Spider").SetActive(false);
		GameObject.Find("Player").SetActive(false);
	}
}

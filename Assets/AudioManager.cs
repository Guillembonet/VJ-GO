using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	static List<GameObject> audios;
	
	void Awake(){
		audios = new List<GameObject>();
		foreach(Transform child in transform) audios.Add(child.gameObject);
		//audios.ForEach(a => Debug.Log(a.name));
	}

	public static void Play(string audio){
		var audioObject = audios.FindLast(audioObj => audioObj.name == audio);
		if(audioObject != null)
			audioObject.GetComponent<AudioSource>().Play();
	}
}

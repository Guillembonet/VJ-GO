using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour {

	public Slider slider;
	public void OnValueChanged (){
		AudioListener.volume = slider.value;
	}

	public void ResetProgress (){
		PlayerPrefs.DeleteAll();
	}
}

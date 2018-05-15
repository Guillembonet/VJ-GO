using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    float h;
    public float H { get { return h; }}

    float v;
    public float V { get { return v; } }

    bool inputEnabled = false;
    public bool InputEnabled {  get { return inputEnabled; } set { inputEnabled = value; }}
	
    public void GetKeyInput()
    {
        if (inputEnabled)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }
    }
}

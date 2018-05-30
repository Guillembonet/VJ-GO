using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour {

	public float speed = 10f;
	void Update ()
    {
        transform.Rotate(new Vector3(0f, 0f, 1f), speed * Time.deltaTime);
    }
}

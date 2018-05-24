using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour {
    public float degrees = 180;
    GameObject m_model;

    void Awake()
    {
        m_model = gameObject.transform.Find("Model").gameObject;
    }

    void Start()
    {
        StartCoroutine("move");
    }

    // Update is called once per frame
    void Update () {
        m_model.transform.Rotate(new Vector3(-180, 0, 0) * Time.deltaTime);
    }

    IEnumerator move()
    {
        yield return new WaitForSeconds(2);
        iTween.MoveTo(gameObject, iTween.Hash(
            "z", 2f,
            "delay", 0f,
            "speed", 4f,
            "easetype", iTween.EaseType.linear
        ));
        yield return new WaitForSeconds(2);
           
        Debug.Log(transform.rotation.eulerAngles);
        iTween.MoveAdd(gameObject, iTween.Hash(
            "y", -1f,
            "delay", 0f,
            "time", 0.1f,
            "easetype", iTween.EaseType.linear
        ));
        yield return new WaitForSeconds(0.1f);

        iTween.RotateAdd(gameObject, iTween.Hash(
            "y", 180f,
            "delay", 0f,
            "time", 0.1f,
            "easetype", iTween.EaseType.linear
        ));
        yield return new WaitForSeconds(0.1f);

        iTween.MoveAdd(gameObject, iTween.Hash(
            "y", 1f,
            "delay", 0f,
            "time", 0.5f,
            "easetype", iTween.EaseType.linear
        ));
        yield return new WaitForSeconds(2);

        iTween.MoveTo(gameObject, iTween.Hash(
            "z", 0f,
            "delay", 0f,
            "speed", 4f,
            "easetype", iTween.EaseType.linear
        ));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SawBlade : MonoBehaviour {
    public float degrees = 180;
    GameObject m_model;

    public Vector3 destination;
    public bool isMoving = false;
    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;

    public float moveSpeed = 1.5f;
    public float rotateTime = 0.5f;
    public float iTweenDelay = 0f;
    
    Board m_board;
    public UnityEvent PlayerKilledEvent;

    void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_model = gameObject.transform.Find("Model").gameObject;
    }

    // Update is called once per frame
    void Update () {
        m_model.transform.Rotate(new Vector3(-180, 0, 0) * Time.deltaTime);
    }

    public void NextMove()
    {
        if (m_board != null)
        {
            //Debug.Log("First level");
            Node current = m_board.FindNodeAt(transform.position);

            /* Check if player come to us */
            if (current.Coordinates == m_board.PlayerNode.Coordinates) PlayerKilledEvent.Invoke();

            Node nextNode = current.GetLinkedNodeInDirection(transform.forward);
            if (nextNode != null)
            {
                bool shouldKill = nextNode.Coordinates == m_board.PlayerNode.Coordinates;
                Node nextNode2 = nextNode.GetLinkedNodeInPlainDirection(transform.forward);

                if (nextNode2 != null || !nextNode.isGross)
                {
                    Move(nextNode.Coordinates, false, shouldKill);
                }
                else
                {
                    Debug.Log("ROTAA");
                    Move(nextNode.Coordinates, true, shouldKill);
                }

            }
        }
    }

    public void Move(Vector3 destinationPos, bool shouldRotate, bool shouldKill, float delayTime = 0f)
    {
        if (m_board != null)
        {
            StartCoroutine(MoveRoutine(destinationPos, shouldRotate, shouldKill, delayTime));
        }
    }

    IEnumerator MoveRoutine(Vector3 destinationPos, bool shouldRotate, bool shouldKill, float delayTime)
    {
        isMoving = true;
        destination = destinationPos;
        yield return new WaitForSeconds(delayTime);

        iTween.MoveTo(gameObject, iTween.Hash(
            "x", destinationPos.x,
            "y", destinationPos.y,
            "z", destinationPos.z,
            "delay", iTweenDelay,
            "easetype", easeType,
            "speed", moveSpeed
        ));

        while (Vector3.Distance(destinationPos, transform.position) > 0.01f)
        {
            yield return null;
        }

        iTween.Stop(gameObject);
        transform.position = destinationPos;
        if (shouldKill)
        {
            PlayerKilledEvent.Invoke();
        }

        KillEnemies();

        if (shouldRotate && !shouldKill)
        {

            iTween.MoveAdd(gameObject, iTween.Hash(
                "y", -1f,
                "delay", delayTime,
                "time", 0.1f,
                "easetype", iTween.EaseType.linear
            ));
            yield return new WaitForSeconds(0.1f);

            iTween.RotateAdd(gameObject, iTween.Hash(
                "y", 180f,
                "delay", delayTime,
                "time", 0.1f,
                "easetype", iTween.EaseType.linear
            ));
            yield return new WaitForSeconds(0.1f);

            iTween.MoveAdd(gameObject, iTween.Hash(
                "y", 1f,
                "delay", delayTime,
                "time", 0.1f,
                "easetype", iTween.EaseType.easeInOutExpo
            ));
            yield return new WaitForSeconds(0.1f);
        }

        isMoving = false;
    }

    void KillEnemies()
    {
        foreach (var enemy in m_board.Enemies)
        {
            if (enemy.GetNode().Coordinates == Utility.Vector3Round(transform.position))
                enemy.Kill();
        }
    }
}

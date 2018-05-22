using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MutantMover : MonoBehaviour, IEnemy
{
    public Vector3 destination;
    public bool isMoving = false;
    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;

    public float moveSpeed = 0.5f;
    public float rotateTime = 0.5f;
    public float iTweenDelay = 0f;

    Vector3 speed;

    Animator anim;

    Board m_board;

    public UnityEvent PlayerKilledEvent;
    bool m_dead = false;

    void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        anim = GetComponentInChildren<Animator>();
    }

    public void NextMove()
    {
        //Debug.Log("GETTTTTT");
        if (m_board != null && !m_dead)
        {
            //Debug.Log("First level");
            Node current = m_board.FindNodeAt(transform.position);
            if (current != null) Debug.Log("Got it");
            else
            {
                Debug.Log(transform.position);
            }
            Node nextNode = current.GetLinkedNodeInDirection(transform.forward);
            if (nextNode != null)
            {
                bool shouldKill = nextNode.Coordinates == m_board.PlayerNode.Coordinates;
                //Node nextNode2 = nextNode.GetLinkedNodeInDirection(transform.forward);

                if (shouldKill)
                {
                    Move(nextNode.Coordinates, shouldKill);
                    //Debug.Log("Dont rotate");
                }
            }

        }

    }

    public void Move(Vector3 destinationPos, bool shouldKill, float delayTime = 0f)
    {
        if (m_board != null)
        {
            StartCoroutine(MoveRoutine(destinationPos, shouldKill, delayTime));
        }
    }

    IEnumerator MoveRoutine(Vector3 destinationPos, bool shouldKill, float delayTime)
    {
        isMoving = true;
        destination = destinationPos;
        yield return new WaitForSeconds(delayTime);

        //iTween.MoveTo(gameObject, iTween.Hash(
        //    "x", destinationPos.x,
        //    "y", destinationPos.y,
        //    "z", destinationPos.z,
        //    "delay", iTweenDelay,
        //    "easetype", easeType,
        //    "speed", moveSpeed,
        //    "onstart", "SetKillAnimation"
        //));

        //while (Vector3.Distance(destinationPos, transform.position) > 0.01f)
        //{
        //    yield return null;
        //}

        //iTween.Stop(gameObject);
        //transform.position = destinationPos;
        SetKillAnimation();
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Kill")) yield return null;
        
        PlayerKilledEvent.Invoke();

        isMoving = false;
    }

    void SetWalkAnimation()
    {
        anim.SetTrigger("Walk");
    }

    void SetIdleAnimation()
    {
        anim.SetTrigger("Idle");
    }

    void SetKillAnimation()
    {
        anim.SetTrigger("Kill");
    }

    void SetDieAnimation()
    {
        anim.SetTrigger("Die");
    }

    Node IEnemy.GetNode()
    {
        return m_board.FindNodeAt(transform.position);
    }

    public bool CanKill()
    {
        return true;
    }

    public void Kill()
    {
        m_dead = true;
        StartCoroutine("KillRoutine");
    }

    IEnumerator KillRoutine()
    {
        SetDieAnimation();
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);

        m_board.Enemies.RemoveAll((e) => e.GetNode().Coordinates == Utility.Vector3Round(transform.position));
    }

    bool IEnemy.isMoving()
    {
        return isMoving;
    }
}

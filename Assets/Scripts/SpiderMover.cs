using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpiderMover : MonoBehaviour, IEnemy
{
    public Vector3 destination;
    public bool isMoving = false;
    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;

    public float moveSpeed = 1.5f;
    public float rotateTime = 0.5f;
    public float iTweenDelay = 0f;

    Vector3 speed;

    Animator anim;

    Board m_board;

    public UnityEvent PlayerKilledEvent;
    bool dead = false;

    void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        //StartCoroutine(CalcVelocity());
    }

    public void NextMove()
    {
        //Debug.Log("GETTTTTT");
        if (m_board != null && !dead)
        {
            //Debug.Log("First level");
            Node current = m_board.FindNodeAt(transform.position);
            Node nextNode = current.GetLinkedNodeInDirection(transform.forward);
            if (nextNode != null)
            {
                bool shouldKill = nextNode.Coordinates == m_board.PlayerNode.Coordinates;
                Node nextNode2  = nextNode.GetLinkedNodeInDirection(transform.forward);
                
                if (nextNode2 != null)
                {
                    Move(nextNode.Coordinates, false, shouldKill);
                    //Debug.Log("Dont rotate");
                }
                else
                {
                    Move(nextNode.Coordinates, true, shouldKill);
                    //Debug.Log("ROtateee");
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

        //var heading = destinationPos - transform.position;
        //if (heading/heading.magnitude != transform.forward)
        //{
        //    iTween.LookTo(gameObject, iTween.Hash(
        //        "looktarget", destinationPos,
        //        "delay", iTweenDelay,
        //        "easetype", easeType,
        //        "time", rotateTime
        //    ));

        //    yield return new WaitForSeconds(0.5f);
        //}

        iTween.MoveTo(gameObject, iTween.Hash(
            "x", destinationPos.x,
            "y", destinationPos.y,
            "z", destinationPos.z,
            "delay", iTweenDelay,
            "easetype", easeType,
            "speed", moveSpeed,
            "onstart", "SetWalkAnimation"
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
            SetKillAnimation();
        }
        else SetIdleAnimation();

        if (shouldRotate && !shouldKill)
        {
            iTween.RotateAdd(gameObject, iTween.Hash(
                "y", 180f,
                "easetype", easeType,
                "delay", delayTime,
                "time", rotateTime
            ));
            yield return new WaitForSeconds(rotateTime);
        }
        
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
        dead = true;
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

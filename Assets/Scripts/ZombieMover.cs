using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMover : MonoBehaviour
{
    public Vector3 destination;
    public bool isMoving = false;
    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;

    public float moveSpeed = 1.5f;
    public float rotateTime = 0.5f;
    public float iTweenDelay = 0f;

    bool foundPlayer = false;

    Vector3 speed;
    Vector3 prevPos;

    Animator anim;

    Board m_board;
    void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        StartCoroutine(CalcVelocity());
    }

    public void NextMove()
    {
        if (m_board != null)
        {
            Node currentNode = m_board.FindNodeAt(transform.position).GetLinkedNodeInDirection(transform.forward);
            if (currentNode != null)
            {
                Node nextNode = currentNode.GetLinkedNodeInDirection(transform.forward);
                if (nextNode != null && nextNode == m_board.PlayerNode)
                {
                    Move(currentNode.Coordinates);
                    foundPlayer = true;
                }
            }
            
        }
        
    }

    //true = player moved; false = player couldn't move
    public bool Move(Vector3 destinationPos, float delayTime = 0f)
    {
        if (m_board != null)
        {
            Node targetNode = m_board.FindNodeAt(destinationPos);
            Node zombieNode = m_board.FindZombieNode(this);
            if (targetNode != null && zombieNode != null && zombieNode.LinkedNodes.Contains(targetNode))
            {
                StartCoroutine(MoveRoutine(destinationPos, delayTime));
                return true;
            }
        }
        return false;
    }

    IEnumerator CalcVelocity()
    {
        while (Application.isPlaying)
        {
            // Position at frame start
            prevPos = transform.position;
            // Wait till it the end of the frame
            yield return new WaitForEndOfFrame();
            // Calculate velocity: Velocity = DeltaPosition / DeltaTime
            speed = (prevPos - transform.position) / Time.deltaTime;
            anim.SetFloat("BlendZ", speed.z);
            anim.SetFloat("BlendX", speed.x);
        }
    }

    IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        isMoving = true;
        destination = destinationPos;
        yield return new WaitForSeconds(delayTime);

        var heading = destinationPos - transform.position;
        if (heading/heading.magnitude != transform.forward)
        {
            iTween.LookTo(gameObject, iTween.Hash(
                "looktarget", destinationPos,
                "delay", iTweenDelay,
                "easetype", easeType,
                "time", rotateTime
            ));

            yield return new WaitForSeconds(0.5f);
        }

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
        isMoving = false;
    }

    public void MoveLeft()
    {
        Vector3 newPosition = transform.position + new Vector3(Board.spacing, 0f, 0f);
        if (!Move(newPosition))
        {
            newPosition = transform.position + new Vector3(Board.spacing / 2f, -Board.spacing / 2f, 0f);
            Move(newPosition);
        }

    }

    public void MoveRight()
    {
        Vector3 newPosition = transform.position + new Vector3(-Board.spacing, 0f, 0f);
        if (!Move(newPosition))
        {
            newPosition = transform.position + new Vector3(-Board.spacing / 2f, Board.spacing / 2f, 0f);
            Move(newPosition);
        }
    }

    public void MoveForward()
    {
        Vector3 newPosition = transform.position + new Vector3(0f, 0f, -Board.spacing);
        if (!Move(newPosition))
        {
            newPosition = transform.position + new Vector3(0f, Board.spacing / 2f, -Board.spacing / 2f);
            if (!Move(newPosition))
            {
                newPosition = transform.position + new Vector3(0f, Board.spacing, 0f);
                Move(newPosition);
            }
        }
    }

    public void MoveBackward()
    {
        Vector3 newPosition = transform.position + new Vector3(0f, 0f, Board.spacing);
        if (!Move(newPosition))
        {
            newPosition = transform.position + new Vector3(0f, -Board.spacing / 2f, Board.spacing / 2f);
            if (!Move(newPosition))
            {
                newPosition = transform.position + new Vector3(0f, -Board.spacing, 0f);
                Move(newPosition);
            }
        }
    }
}

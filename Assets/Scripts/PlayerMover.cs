using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour {

    public Vector3 destination;
    public bool isMoving = false;
    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;

    public float moveSpeed = 1.5f;
    public float iTweenDelay = 0f;

    Board m_board;

    Animator animator;
	void Awake ()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        UpdateBoard();

    }
    
    //true = player moved; false = player couldn't move
    public bool Move(Vector3 destinationPos, float delayTime = 0f)
    {
        if (m_board != null)
        {
            Node targetNode = m_board.FindNodeAt(destinationPos);
            if (targetNode != null && m_board.PlayerNode.LinkedNodes.Contains(targetNode))
            {
                StartCoroutine(MoveRoutine(destinationPos, delayTime));
                return true;
            }
        }
        return false;
    }

    IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        animator.SetFloat("Blend", 1);
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
        isMoving = false;
        UpdateBoard();
        animator.SetFloat("Blend", 0);
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

    void UpdateBoard()
    {
        if (m_board != null)
        {
            m_board.UpdatePlayerNode();
        }
    }
}

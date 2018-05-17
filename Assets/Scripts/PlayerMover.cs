using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class PlayerMover : MonoBehaviour {

    public Vector3 destination;
    public bool isMoving = false;
    public iTween.EaseType easeTypeMove = iTween.EaseType.linear;
    public iTween.EaseType easeTypeRotate = iTween.EaseType.easeInOutExpo;

    public float moveSpeed = 1.5f;
    public float iTweenDelay = 0f;
    public float rotateTime = 0.5f;

    public UnityEvent playerMovesEvent;

    Board m_board;

    Animator animator;
    public bool hack = false;
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
                playerMovesEvent.Invoke();
                StartCoroutine(MoveRoutine(destinationPos, delayTime));
                return true;
            }
        }
        return false;
    }

    IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        isMoving = true;
        destination = destinationPos;
        yield return new WaitForSeconds(delayTime);

        var dirVector = destinationPos - transform.position;
        var rotation = Quaternion.LookRotation(dirVector);
        // We just want to rotate our 'y'
        var cleanedRotation = new Vector3(0, rotation.eulerAngles.y, 0);

        // If 'y' has changed then rotate and furthermore is necessary that our x or z coords have been change too
        // just for avoid rotation when we are climbing
        if (cleanedRotation.y != transform.rotation.eulerAngles.y &&
            (destination.x != transform.position.x || destination.z != transform.position.z ))
        {
            iTween.RotateTo(gameObject, iTween.Hash(
                "rotation", cleanedRotation,
                "delay", iTweenDelay,
                "easetype", easeTypeRotate,
                "time", rotateTime
            ));

            yield return new WaitForSeconds(0.5f);
        }

        if(AreDiagonallyAligned(transform.position, destinationPos))
        {
            var xDestination = transform.position.x - 0.7f;
            var yDestination = transform.position.y + 1f;
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", xDestination,
                "delay", iTweenDelay,
                "easetype", easeTypeMove,
                "speed", moveSpeed,
                "onstart", "SetRunAnimation",
                "oncomplete", "SetClimbUpAnimation"
            ));
            while (transform.position.x != xDestination) yield return null;

            iTween.MoveTo(gameObject, iTween.Hash(
                "y", yDestination,
                "delay", iTweenDelay,
                "easetype", easeTypeMove,
                "speed", moveSpeed
            ));
            while (transform.position.y != yDestination) yield return null;
        }
        else
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", destinationPos.x,
                "y", destinationPos.y,
                "z", destinationPos.z,
                "delay", iTweenDelay,
                "easetype", easeTypeMove,
                "speed", moveSpeed,
                "onstart", "SetRunAnimation"
            ));

            while (Vector3.Distance(destinationPos, transform.position) > 0.1f)
            {
                yield return null;
            }

            iTween.Stop(gameObject);
            SetIdleAnimation();
            transform.position = destinationPos;
            
        }

        isMoving = false;
        UpdateBoard();
    }

    public void MoveRight()
    {
        Vector3 newPosition = transform.position + new Vector3(Board.spacing, 0f, 0f);
        if (!Move(newPosition))
        {
            newPosition = transform.position + new Vector3(Board.spacing / 2f, -Board.spacing / 2f, 0f);
            Move(newPosition);
        }

    }

    public void MoveLeft()
    {
        Vector3 newPosition = transform.position + new Vector3(-Board.spacing, 0f, 0f);
        if (!Move(newPosition))
        {
            newPosition = transform.position + new Vector3(-Board.spacing / 2f, Board.spacing / 2f, 0f);
            Move(newPosition);
        }
    }

    public void MoveBackward()
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

    public void MoveForward()
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

    void SetRunAnimation()
    {
        animator.SetTrigger("Run");
    }

    void SetIdleAnimation()
    {
        animator.SetTrigger("Idle");
    }

    void SetClimbUpAnimation()
    {
        animator.SetTrigger("ClimbUp");
    }

    bool AreDiagonallyAligned(Vector3 start, Vector3 end)
    {
        if (start.y - end.y != 0f && (start.x != end.x || start.z != end.z)) return true;
        else return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{

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

    void Awake()
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
            (destination.x != transform.position.x || destination.z != transform.position.z))
        {
            iTween.RotateTo(gameObject, iTween.Hash(
                "rotation", cleanedRotation,
                "delay", iTweenDelay,
                "easetype", easeTypeRotate,
                "time", rotateTime
            ));

            yield return new WaitForSeconds(0.5f);
        }
        Node NodeDestination = m_board.FindNodeAt(destinationPos);

        if (AreDiagonallyAligned(transform.position, destinationPos))
        {
            // Si el nodo destino es vertical y además no es escalada vertical
            if (NodeDestination.wall &&
                (destinationPos.x != transform.position.x || destinationPos.z != transform.position.z))
            {
                Debug.Log("Entramos modo1");
                string horizontal = "";
                float hDestination;
                float vDestination;
                if (transform.forward == new Vector3(-1, 0, 0))
                {
                    horizontal = "x";
                    hDestination = transform.position.x - 0.7f;
                }
                else
                {
                    horizontal = "z";
                    hDestination = transform.position.z - 0.7f;
                }
                vDestination = transform.position.y + Board.spacing / 2;

                iTween.MoveTo(gameObject, iTween.Hash(
                    horizontal, hDestination,
                    "delay", iTweenDelay,
                    "easetype", easeTypeMove,
                    "speed", moveSpeed,
                    "onstart", "SetRunAnimation",
                    "oncomplete", "SetClimbUpAnimation"
                ));
                if (horizontal == "x")
                    while (transform.position.x != hDestination) yield return null;
                else
                    while (transform.position.z != hDestination) yield return null;

                iTween.MoveTo(gameObject, iTween.Hash(
                    "y", vDestination,
                    "delay", iTweenDelay,
                    "easetype", easeTypeMove,
                    "speed", 0.5f
                ));
                while (transform.position.y != vDestination) yield return null;

            }
            // Si el nodo destino no es vertical, no es escalada vertical, es decir, llegamos a la cumbre
            else
            {   
                // Si vamos hacia arriba
                if (destinationPos.y > transform.position.y)
                {
                    Debug.Log("Entramos modo 3");

                    string horizontal = "";
                    float hDestination;
                    float vDestination;
                    if (transform.forward == new Vector3(-1, 0, 0))
                    {
                        horizontal = "x";
                        hDestination = transform.position.x - 1.3f;
                    }
                    else
                    {
                        horizontal = "z";
                        hDestination = transform.position.z - 1.3f;
                    }
                    vDestination = transform.position.y + Board.spacing / 2;

                    iTween.MoveTo(gameObject, iTween.Hash(
                        "y", vDestination,
                        "delay", iTweenDelay,
                        "easetype", easeTypeMove,
                        "speed", 0.5f,
                        "onstart", "SetClimbEndAnimation"
                    ));
                    while (transform.position.y != vDestination) yield return null;

                    iTween.MoveTo(gameObject, iTween.Hash(
                        horizontal, hDestination,
                        "delay", iTweenDelay,
                        "easetype", easeTypeMove,
                        "speed", moveSpeed,
                        "onstart", "SetRunAnimation"
                    ));
                    if (horizontal == "x")
                        while (transform.position.x != hDestination) yield return null;
                    else
                        while (transform.position.z != hDestination) yield return null;
                    SetIdleAnimation();
                }
                // Si vamos hacia abajo
                else
                {
                    string horizontal = "";
                    float hDestination;
                    float vDestination;
                    if (Utility.Vector3Round(transform.forward) == (new Vector3(1f, 0, 0)))
                    {
                        Debug.Log(transform.forward);
                        horizontal = "x";
                        hDestination = transform.position.x + 0.7f;
                    }
                    else
                    {
                        horizontal = "z";
                        hDestination = transform.position.z + 0.7f;
                    }
                    vDestination = transform.position.y - Board.spacing / 2;

                    iTween.MoveTo(gameObject, iTween.Hash(
                        "y", vDestination,
                        "delay", iTweenDelay,
                        "easetype", easeTypeMove,
                        "speed", 3f,
                        "onstart", "SetIdleAnimation"
                    ));
                    while (transform.position.y != vDestination) yield return null;

                    iTween.MoveTo(gameObject, iTween.Hash(
                        horizontal, hDestination,
                        "delay", iTweenDelay,
                        "easetype", easeTypeMove,
                        "speed", moveSpeed,
                        "onstart", "SetRunAnimation"
                    ));
                    if (horizontal == "x")
                        while (transform.position.x != hDestination) yield return null;
                    else
                        while (transform.position.z != hDestination) yield return null;
                    SetIdleAnimation();
                    Debug.Log("aqui");
                }

            }
        }
        // Si la escalada es vertical..
        else if (NodeDestination.wall)
        {
            Debug.Log("Entramos modo 222");
            float vDestination = 0;
            if (destinationPos.y > transform.position.y)
                vDestination = transform.position.y + Board.spacing;
            else
                vDestination = transform.position.y - Board.spacing;
            iTween.MoveTo(gameObject, iTween.Hash(
                "y", vDestination,
                "delay", iTweenDelay,
                "easetype", easeTypeMove,
                "speed", 1f,
                "onstart", "SetClimbUpAnimation"
            ));
            while (transform.position.y != vDestination) yield return null;
        }
        // Si no hay nada raro, caminamos en horizontal
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
        /*Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbUp"));
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbUp") && animator.isInitialized)
        {
            animator.Rebind();
            Debug.Log("Rebindo");
        }
        else animator.SetTrigger("ClimbUp");*/
        //animator.Play("ClimbUp", 0, 0f);


        animator.SetTrigger("ClimbUp");
    }

    void SetClimbEndAnimation()
    {
        animator.SetTrigger("ClimbEnd");
    }

    bool AreDiagonallyAligned(Vector3 start, Vector3 end)
    {
        if (start.y - end.y != 0f && (start.x != end.x || start.z != end.z)) return true;
        else return false;
    }
}

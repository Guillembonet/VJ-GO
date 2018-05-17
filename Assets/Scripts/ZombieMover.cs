using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMover : MonoBehaviour
{
    public Vector3 destination;
    public bool isMoving = false;
    public iTween.EaseType easeTypeRotate = iTween.EaseType.easeInOutExpo;
    public iTween.EaseType easeTypeMove = iTween.EaseType.linear;

    public float moveSpeed = 1.5f;
    public float rotateTime = 0.5f;
    public float iTweenDelay = 0f;

    bool m_foundPlayer = false;
    Node m_nextMove;

    bool m_standing = true;

    Vector3 speed;
    Vector3 prevPos;

    Animator anim;

    Board m_board;
    void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        anim = GetComponentInChildren<Animator>();
    }

    public void NextMove()
    {
        if (m_board != null)
        {
            if (m_foundPlayer)
            {
                if (m_nextMove != null)
                {
                    if (!Move(m_nextMove.Coordinates)) m_foundPlayer = false;
                    else m_nextMove = m_board.PreviousPlayerNode;
                }
            } else
            {
                Node currentNode = m_board.FindNodeAt(transform.position).GetLinkedNodeInDirection(transform.forward);
                if (currentNode != null)
                {
                    Node nextNode = currentNode.GetLinkedNodeInDirection(transform.forward);
                    if (nextNode != null && nextNode == m_board.PlayerNode)
                    {
                        //zombie finds player and prepares to move in the next turn
                        m_foundPlayer = true;
                        m_nextMove = currentNode;
                    }
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

    IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        isMoving = true;
        destination = destinationPos;
        yield return new WaitForSeconds(delayTime);

        var lookPos = new Vector3(destinationPos.x, transform.position.y, destinationPos.z);
        var heading = lookPos - transform.position;
        

        if (Utility.AreParallelToFloor(transform.position, destinationPos)) {
            if (heading / heading.magnitude != transform.forward)
            {
                iTween.LookTo(gameObject, iTween.Hash(
                    "looktarget", destinationPos,
                    "delay", iTweenDelay,
                    "easetype", easeTypeRotate,
                    "time", rotateTime
                ));

                yield return new WaitForSeconds(iTweenDelay + rotateTime);
            }

            iTween.MoveTo(gameObject, iTween.Hash(
                "x", destinationPos.x,
                "y", destinationPos.y,
                "z", destinationPos.z,
                "delay", iTweenDelay,
                "easetype", easeTypeMove,
                "speed", moveSpeed,
                "onstart", "SetRunAnimation"
            ));
        } else if (Utility.AreDiagonallyAligned(transform.position, destinationPos))
        {
            if (Utility.Arg1IsHigherThanArg2(transform.position, destinationPos))
            {
                if (m_standing) //from edge to climbing
                {
                    m_standing = false;

                } else //from climbing to floor
                {
                    m_standing = true;
                }
            } else
            {
                if (m_standing) //from floor to climbing
                {
                    Debug.Log("Floor to Climb");
                    if (heading / heading.magnitude != transform.forward)
                    {
                        iTween.LookTo(gameObject, iTween.Hash(
                            "looktarget", lookPos,
                            "delay", iTweenDelay,
                            "easetype", easeTypeRotate,
                            "time", rotateTime
                        ));

                        yield return new WaitForSeconds(iTweenDelay + rotateTime);
                    }

                    float destinationX = 0f;
                    float destinationZ = 0f;
                    Utility.GetClimbOffset(ref destinationX, ref destinationZ, destinationPos, transform.forward);

                    iTween.MoveTo(gameObject, iTween.Hash(
                        "x", destinationX,
                        "z", destinationZ,
                        "delay", iTweenDelay,
                        "easetype", easeTypeMove,
                        "speed", moveSpeed,
                        "onstart", "SetRunAnimation",
                        "oncomplete", "SetClimbUpAnimation"
                    ));

                    while (transform.position != new Vector3(destinationX, transform.position.y, destinationZ)) yield return null;

                    iTween.MoveTo(gameObject, iTween.Hash(
                        "y", destinationPos.y,
                        "delay", iTweenDelay,
                        "easetype", easeTypeMove,
                        "speed", 0.5f
                    ));

                    m_standing = false;
                }
                else //from climbing to edge
                {
                    m_standing = true;
                }
            }
        } else if (Utility.AreVerticallyAligned(transform.position, destinationPos))
        {
            if (Utility.Arg1IsHigherThanArg2(transform.position, destinationPos)) //descending climb
            {

            }
            else //ascending climb
            {

            }
        }

        if (m_standing)
        {
            while (Vector3.Distance(destinationPos, transform.position) > 0.01f)
            {
                yield return null;
            }
        }

        iTween.Stop(gameObject);
        transform.position = destinationPos;
        if (m_standing) SetIdleAnimation();
        isMoving = false;
    }

    void SetRunAnimation()
    {
        anim.SetTrigger("Run");
    }

    void SetIdleAnimation()
    {
        anim.SetTrigger("Idle");
    }

    void SetClimbUpAnimation()
    {
        anim.SetTrigger("ClimbUp");
    }
}

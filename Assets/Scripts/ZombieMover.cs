using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ZombieMover : MonoBehaviour
{
    public Vector3 destination;
    public bool isMoving = false;
    public iTween.EaseType easeTypeRotate = iTween.EaseType.easeInOutExpo;
    public iTween.EaseType easeTypeMove = iTween.EaseType.linear;

    public UnityEvent PlayerKilledEvent;

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
                    if (m_nextMove == m_board.PlayerNode)
                    {
                        PlayerKilledEvent.Invoke();
                        Debug.Log("KILL");
                    } else
                    {
                        if (!Move(m_nextMove.Coordinates)) m_foundPlayer = false;
                        else m_nextMove = m_board.PreviousPlayerNode;
                    }
                }
            } else
            {
                Node currentNode = m_board.FindNodeAt(transform.position).GetLinkedNodeInDirection(transform.forward);
                if (currentNode != null)
                {
                    if (currentNode == m_board.PlayerNode)
                    {
                        PlayerKilledEvent.Invoke();
                        Debug.Log("KILL");
                    } else
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
        

        if (Utility.AreParallelToFloor(Utility.Vector3Round(transform.position), destinationPos)) {
            if (m_standing) //Normal run
            {
                yield return new WaitForSeconds(LookTo(heading, transform.forward, lookPos));
                RunToXYZ(destinationPos);
            } else //sideways wall moving
            {
                //TODO
            }

        } else if (Utility.AreDiagonallyAligned(Utility.Vector3Round(transform.position), destinationPos))
        {
            if (Utility.Arg1IsHigherThanArg2(transform.position, destinationPos))
            {
                if (m_standing) //from edge to climbing
                {
                    yield return new WaitForSeconds(LookTo(heading, transform.forward, lookPos));

                    float destinationX = 0f;
                    float destinationZ = 0f;
                    Utility.GetClimbDescendOffset(ref destinationX, ref destinationZ, destinationPos, transform.forward);

                    RunToXZ(destinationX, destinationZ);
                    while (transform.position != new Vector3(destinationX, transform.position.y, destinationZ)) yield return null; //wait for run to end

                    yield return new WaitForSeconds(RotateAdd(180f));

                    anim.SetFloat("Direction", -1.0f);
                    ClimbUp(destinationPos.y);
                    while (transform.position.y != destinationPos.y) yield return null; //wait for climb to end
                    anim.SetFloat("Direction", 1.0f);

                    m_standing = false;

                } else //from climbing to floor
                {
                    anim.SetFloat("Direction", -1.0f);
                    ClimbUp(destinationPos.y + 0.5f);
                    while (transform.position.y != destinationPos.y + 0.5f) yield return null; //wait for climb to end
                    anim.SetFloat("Direction", 1.0f);
                    SetIdleAnimation();
                    transform.position = new Vector3(transform.position.x, destinationPos.y, transform.position.z);

                    yield return new WaitForSeconds(RotateAdd(180f));

                    RunToXYZ(destinationPos);

                    m_standing = true;
                }
            } else
            {
                if (m_standing) //from floor to climbing
                {
                    yield return new WaitForSeconds(LookTo(heading, transform.forward, lookPos));

                    float destinationX = 0f;
                    float destinationZ = 0f;
                    Utility.GetClimbOffset(ref destinationX, ref destinationZ, destinationPos, transform.forward);

                    RunToXZ(destinationX, destinationZ);
                    while (transform.position != new Vector3(destinationX, transform.position.y, destinationZ)) yield return null; //wait for run to end

                    ClimbUp(destinationPos.y);

                    m_standing = false;
                }
                else //from climbing to edge
                {

                    ClimbUp(destinationPos.y, onstart: "SetClimbEndAnimation");

                    while (transform.position.y != destinationPos.y) yield return null; //wait for climb to end

                    RunToXZ(destinationPos.x, destinationPos.z);

                    m_standing = true;
                }
            }
        } else if (Utility.AreVerticallyAligned(Utility.Vector3Round(transform.position), destinationPos)) //climbing
        {
            ClimbUp(destinationPos.y, speed: 1f);
            while (transform.position.y != destinationPos.y) yield return null; //wait for climb to end
        }

        if (m_standing)
        {
            while (Vector3.Distance(destinationPos, transform.position) > 0.01f) //wait for run to end
            {
                yield return null;
            }

            iTween.Stop(gameObject);
            transform.position = destinationPos;
            SetIdleAnimation();
        }
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

    void SetClimbEndAnimation()
    {
        anim.SetTrigger("ClimbEnd");
    }

    void RunToXYZ(Vector3 destination)
    {
        iTween.MoveTo(gameObject, iTween.Hash(
                "x", destination.x,
                "y", destination.y,
                "z", destination.z,
                "delay", iTweenDelay,
                "easetype", easeTypeMove,
                "speed", moveSpeed,
                "onstart", "SetRunAnimation"
            ));
    }

    void RunToXZ(float x, float z, bool climbAtEnd = false)
    {
        Hashtable itweenHash = iTween.Hash(
            "x", x,
            "z", z,
            "delay", iTweenDelay,
            "easetype", easeTypeMove,
            "speed", moveSpeed,
            "onstart", "SetRunAnimation"
        );

        if (climbAtEnd) itweenHash.Add("oncomplete", "SetClimbUpAnimation");

        iTween.MoveTo(gameObject, itweenHash);
    }

    float LookTo(Vector3 heading, Vector3 forward, Vector3 destination)
    {
        if (heading / heading.magnitude != transform.forward)
        {
            iTween.LookTo(gameObject, iTween.Hash(
                "looktarget", destination,
                "delay", iTweenDelay,
                "easetype", easeTypeRotate,
                "time", rotateTime
            ));

            return iTweenDelay + rotateTime;
        }
        else return 0f;
    }

    float RotateAdd(float degrees)
    {
        iTween.RotateAdd(gameObject, iTween.Hash(
            "y", degrees,
            "delay", iTweenDelay,
            "easetype", easeTypeRotate,
            "time", rotateTime
        ));

        return rotateTime + iTweenDelay;
    }

    void ClimbUp(float y, string onstart = "SetClimbUpAnimation", float speed = 0.5f)
    {
        iTween.MoveTo(gameObject, iTween.Hash(
            "y", y,
            "delay", iTweenDelay,
            "easetype", easeTypeMove,
            "speed", speed,
            "onstart", onstart
        ));
    }
}

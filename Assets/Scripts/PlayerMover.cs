﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    public Board Board { get { return m_board; } }

    Animator animator;

    void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        UpdateBoard();
        //TODO: comment for production
        PlayerPrefs.DeleteAll();
        Object.FindObjectOfType<TextMeshProUGUI>().GetComponent<TextMeshProUGUI>().text = (PlayerPrefs.GetInt("Level1Gems") + PlayerPrefs.GetInt("Level2Gems") + PlayerPrefs.GetInt("Level3Gems")).ToString();
        if (!PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Gems").Equals(0)) {
            GameObject.Find("Gem").SetActive(false);
        }
    }

    //true = player moved; false = player couldn't move
    public bool Move(Vector3 destinationPos, float delayTime = 0f)
    {
        if (m_board != null)
        {
            Node targetNode = m_board.FindNodeAt(destinationPos);
            
            if (targetNode != null && m_board.PlayerNode.LinkedNodes.Contains(targetNode))
            {
                IEnemy enemy = null;
                if (m_board.Enemies != null)
                {
                    IEnemy enemyFound = m_board.Enemies.Find(e => e.CanKill() && (e.GetNode() != null && e.GetNode().Coordinates == targetNode.Coordinates));
                    if (enemyFound != null) enemy = enemyFound;
                }

                StartCoroutine(MoveRoutine(destinationPos, delayTime, enemy));
                
                return true;
            }
        }
        return false;
    }

    // If enemy != null we should attack
    IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime, IEnemy enemy)
    {
        isMoving = true;
        destination = destinationPos;
        yield return new WaitForSeconds(delayTime);

        var dirVector = destinationPos - transform.position;
        var rotation = Quaternion.LookRotation(dirVector);
        // We just want to rotate our 'y'
        var cleanedRotation = new Vector3(0, rotation.eulerAngles.y, 0);
        Node nodeDestination = m_board.FindNodeAt(destinationPos);
        // If 'y' has changed then rotate and furthermore is necessary that our x or z coords have been change too
        // just for avoid rotation when we are climbing
        if (cleanedRotation.y != transform.rotation.eulerAngles.y &&
            (destination.x != transform.position.x || destination.z != transform.position.z) &&
            !(nodeDestination.wall && destination.y == transform.position.y))
        {
            iTween.RotateTo(gameObject, iTween.Hash(
                "rotation", cleanedRotation,
                "delay", iTweenDelay,
                "easetype", easeTypeRotate,
                "time", rotateTime
            ));

            yield return new WaitForSeconds(0.5f);
        }
        

        if (Utility.AreDiagonallyAligned(transform.position, destinationPos))
        {
            // Si el nodo destino es vertical y además no es escalada vertical
            if (nodeDestination.wall &&
                (destinationPos.x != transform.position.x || destinationPos.z != transform.position.z))
            {
                if (destinationPos.y > transform.position.y)
                {
                    Debug.Log("Caso1");
                    string horizontal = "";
                    float hDestination;
                    float vDestination;
                    if (Utility.Vector3Round(transform.forward) == new Vector3(-1, 0, 0))
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
                else{
                    //Debug.Log("Caso2");
                    string horizontal = "";
                    float hDestination;
                    float vDestination;
                    if (Utility.Vector3Round(transform.forward) == new Vector3(1, 0, 0))
                    {
                        horizontal = "x";
                        hDestination = transform.position.x + 1.3f;
                    }
                    else
                    {
                        horizontal = "z";
                        hDestination = transform.position.z + 1.3f;
                    }
                    vDestination = transform.position.y - Board.spacing / 2;
                    
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
                    
                    iTween.RotateAdd(gameObject, iTween.Hash(
                        "y", 180f,
                        "delay", iTweenDelay,
                        "easetype", easeTypeRotate,
                        "time", rotateTime
                    ));
                    yield return new WaitForSeconds(rotateTime);

                    // We reverse climb up animation
                    animator.SetFloat("Direction", -1.0f);
                    iTween.MoveTo(gameObject, iTween.Hash(
                        "y", vDestination,
                        "delay", iTweenDelay,
                        "easetype", easeTypeMove,
                        "speed", 0.5f,
                        "onstart", "SetClimbUpAnimation"
                    ));
                    while (transform.position.y != vDestination) yield return null;
                    // Set default animation
                    animator.SetFloat("Direction", 1.0f);
                }
            }
            // Si el nodo destino no es vertical, no es escalada vertical, es decir, llegamos a la cumbre
            else
            {   
                // Si vamos hacia arriba
                if (destinationPos.y > transform.position.y)
                {
                    //Debug.Log("Caso3");
                    string horizontal = "";
                    float hDestination;
                    float vDestination;
                    if (Utility.Vector3Round(transform.forward) == new Vector3(-1, 0, 0))
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
                    //Debug.Log("Caso4");
                    string horizontal = "";
                    float hDestination;
                    float vDestination;
                    if (Utility.Vector3Round(transform.forward) == (new Vector3(1f, 0, 0)))
                    {
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
                }

            }
        }
        // Si la escalada es vertical..
        else if (nodeDestination.wall)
        {
            if(destinationPos.y != transform.position.y)
            {
                //Debug.Log("Caso5 - Escalada vertical");
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
            else
            {
                //Debug.Log("Secret");
                string horizontal = "";
                float hDestination = 0;
                // Vamos de izquierda a derecha
                if(transform.position.x != destinationPos.x)
                {
                    horizontal = "x";
                    if(transform.position.x > destinationPos.x)
                        hDestination = transform.position.x - Board.spacing;
                    else
                        hDestination = transform.position.x + Board.spacing;
                }
                else
                {
                    horizontal = "z";
                    if (transform.position.z < destinationPos.z)
                        hDestination = transform.position.z + Board.spacing;
                    else
                        hDestination = transform.position.z - Board.spacing;
                }

                iTween.MoveTo(gameObject, iTween.Hash(
                    horizontal, hDestination,
                    "delay", iTweenDelay,
                    "easetype", easeTypeMove,
                    "speed", 1f,
                    "onstart", "SetClimbUpAnimation"
                ));
                if(horizontal == "x")
                    while (transform.position.x != hDestination) yield return null;
                else
                    while (transform.position.z != hDestination) yield return null;
            }
        }
        // Si no hay nada raro, caminamos en horizontal
        else
        {
            //Debug.Log("Caso6 - Horizontal");
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
            if (enemy != null)
            {
                enemy.Kill();
                SetKillAnimation();
            }
            else SetIdleAnimation();
            transform.position = destinationPos;

        }
        isMoving = false;
        UpdateBoard();
        playerMovesEvent.Invoke();
    }

    public void MoveBackward()
    {
        Vector3 newPosition = transform.position + new Vector3(Board.spacing, 0f, 0f);
        if (!Move(newPosition))
        {
            newPosition = transform.position + new Vector3(Board.spacing / 2f, -Board.spacing / 2f, 0f);
            Move(newPosition);
        }

    }

    public void MoveForward()
    {
        Vector3 newPosition = transform.position + new Vector3(-Board.spacing, 0f, 0f);
        if (!Move(newPosition))
        {
            newPosition = transform.position + new Vector3(-Board.spacing / 2f, Board.spacing / 2f, 0f);
            Move(newPosition);
        }
    }

    public void MoveLeft()
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

    public void MoveRight()
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
            m_board.CheckButtons();
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

    void SetClimbEndAnimation()
    {
        animator.SetTrigger("ClimbEnd");
    }

    void SetDieAnimation()
    {
        animator.SetTrigger("Die");
    }

    void SetKillAnimation()
    {
        animator.SetTrigger("Kill");
    }

    public void Kill()
    {
        SetDieAnimation();
        //playerMovesEvent.Invoke();
    }

    public void FallAndDie()
    {
        StartCoroutine(FallAndDieRoutine());
    }

    IEnumerator FallAndDieRoutine()
    {
        iTween.MoveAdd(gameObject, iTween.Hash(
            "y", -2f,
            "easetype", iTween.EaseType.easeInOutElastic,
            "time", 1f
        ));

        yield return new WaitForSeconds(1);
    }

    void OnTriggerEnter(Collider other) {
        if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Gems").Equals(0)) {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "Gems", PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Gems")+1);
            Object.FindObjectOfType<TextMeshProUGUI>().GetComponent<TextMeshProUGUI>().text = (PlayerPrefs.GetInt("Level1Gems") + PlayerPrefs.GetInt("Level2Gems") + PlayerPrefs.GetInt("Level3Gems")).ToString();
            Debug.Log(PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Gems"));
            StartCoroutine(explodeGemAnim(other.gameObject));
        }
    }

    IEnumerator explodeGemAnim(GameObject g) {
        g.GetComponentInChildren<ParticleSystem>().Play();
        g.GetComponentInChildren<MeshRenderer>().enabled = false;
        g.GetComponent<Behaviour>().enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(g);
        
    }
}

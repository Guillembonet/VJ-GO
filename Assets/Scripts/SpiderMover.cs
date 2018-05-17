﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMover : MonoBehaviour
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
        if (m_board != null)
        {
            //Debug.Log("First level");
            Node current = m_board.FindNodeAt(transform.position);
            Node nextNode = current.GetLinkedNodeInDirection(transform.forward);
            if (nextNode != null)
            {
                //Debug.Log("Second level");
                Node nextNode2  = nextNode.GetLinkedNodeInDirection(transform.forward);
                if (nextNode2 != null)
                {
                    Move(nextNode.Coordinates, false);
                    Debug.Log("Dont rotate");
                }
                else
                {
                    Move(nextNode.Coordinates, true);
                    Debug.Log("ROtateee");
                }
                
            }
            
        }
        
    }
    
    public void Move(Vector3 destinationPos, bool shouldRotate ,float delayTime = 0f)
    {
        if (m_board != null)
        { 
            StartCoroutine(MoveRoutine(destinationPos, shouldRotate, delayTime));
        }
    }

    IEnumerator MoveRoutine(Vector3 destinationPos, bool shouldRotate, float delayTime)
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
        SetIdleAnimation();

        if (shouldRotate)
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

    //public void MoveLeft()
    //{
    //    Vector3 newPosition = transform.position + new Vector3(Board.spacing, 0f, 0f);
    //    if (!Move(newPosition))
    //    {
    //        newPosition = transform.position + new Vector3(Board.spacing / 2f, -Board.spacing / 2f, 0f);
    //        Move(newPosition);
    //    }

    //}

    //public void MoveRight()
    //{
    //    Vector3 newPosition = transform.position + new Vector3(-Board.spacing, 0f, 0f);
    //    if (!Move(newPosition))
    //    {
    //        newPosition = transform.position + new Vector3(-Board.spacing / 2f, Board.spacing / 2f, 0f);
    //        Move(newPosition);
    //    }
    //}

    //public void MoveForward()
    //{
    //    Vector3 newPosition = transform.position + new Vector3(0f, 0f, -Board.spacing);
    //    if (!Move(newPosition))
    //    {
    //        newPosition = transform.position + new Vector3(0f, Board.spacing / 2f, -Board.spacing / 2f);
    //        if (!Move(newPosition))
    //        {
    //            newPosition = transform.position + new Vector3(0f, Board.spacing, 0f);
    //            Move(newPosition);
    //        }
    //    }
    //}

    //public void MoveBackward()
    //{
    //    Vector3 newPosition = transform.position + new Vector3(0f, 0f, Board.spacing);
    //    if (!Move(newPosition))
    //    {
    //        newPosition = transform.position + new Vector3(0f, -Board.spacing / 2f, Board.spacing / 2f);
    //        if (!Move(newPosition))
    //        {
    //            newPosition = transform.position + new Vector3(0f, -Board.spacing, 0f);
    //            Move(newPosition);
    //        }
    //    }
    //}
}
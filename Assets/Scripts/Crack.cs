using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Crack : MonoBehaviour {

    GameObject m_fase1;
    GameObject m_fase2;
    GameObject m_fase3;

    enum state {fase1, fase2, fase3};

    state actualState;

    Board m_board;

    public UnityEvent PlayerKilledEvent;

    void Awake()
    {
        actualState = state.fase1;
        m_fase1 = transform.Find("Fase1").gameObject;
        m_fase2 = transform.Find("Fase2").gameObject;
        m_fase3 = transform.Find("Fase3").gameObject;

        m_fase3.transform.localScale = Vector3.zero;

        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }
    
    public void Footle()
    {
        if(m_board.PlayerNode.Coordinates == Utility.Vector3Round(transform.position))
        {
            if (actualState == state.fase1)
            {
                HandleState1();
            }
            else if (actualState == state.fase2)
            {
                StartCoroutine(HandleState2Routine());
            }
            else if (actualState == state.fase3)
            {
                HandleState3();
            }
        }
    }

    void HandleState1()
    {
        m_fase2.SetActive(true);
        actualState = state.fase2;
    }

    IEnumerator HandleState2Routine()
    {
        m_fase3.SetActive(true);
        iTween.ScaleTo(m_fase3, iTween.Hash(
            "time", 1f,
            "scale", new Vector3(4, 4, 4),
            "easetype", iTween.EaseType.easeInOutElastic,
            "delay", 0f
        ));
        while (m_fase3.transform.localScale != new Vector3(4, 4, 4)) yield return null;

        actualState = state.fase3;

        if(m_board.PlayerNode.Coordinates == Utility.Vector3Round(transform.position))
        {
            PlayerKilledEvent.Invoke();
            Debug.Log("Polee");
        }
    }

    void HandleState3()
    {
        PlayerKilledEvent.Invoke();
        Debug.Log("Polee2");
    }
}

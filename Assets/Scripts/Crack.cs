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

    bool m_occupied = false;

    void Awake()
    {
        actualState = state.fase1;
        m_fase1 = transform.Find("Fase1").gameObject;
        m_fase2 = transform.Find("Fase2").gameObject;
        m_fase3 = transform.Find("Fase3").gameObject;

        m_fase3.transform.localScale = Vector3.zero;

        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }
    
    bool HasThePlayerSteppedOnMe()
    {
        return m_board.PlayerNode.Coordinates == Utility.Vector3Round(transform.position);
    }

    bool HasSomeEnemySteppedOnMe()
    {
        return m_board.Enemies.Find(e => e.GetNode().Coordinates == Utility.Vector3Round(transform.position)) != null;
    }

    bool HasSomeoneSteppedOnMe()
    {
        return HasThePlayerSteppedOnMe() || HasSomeEnemySteppedOnMe();
    }

    public void Footle()
    {
        if(HasSomeoneSteppedOnMe() && !m_occupied)
        {
            m_occupied = true;
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

        if (!HasSomeoneSteppedOnMe()) m_occupied = false;
    }

    void HandleState1()
    {
        AudioManager.Play("CrackBreak");
        m_fase2.SetActive(true);
        actualState = state.fase2;
    }

    IEnumerator HandleState2Routine()
    {
        AudioManager.Play("CrackBreak");
        m_fase3.SetActive(true);
        iTween.ScaleTo(m_fase3, iTween.Hash(
            "time", 1f,
            "scale", new Vector3(4, 4, 4),
            "easetype", iTween.EaseType.easeInOutElastic,
            "delay", 0f
        ));
        yield return new WaitForSeconds(0.8f);
        AudioManager.Play("CrackSuck");
        yield return new WaitForSeconds(0.2f);

        actualState = state.fase3;

        HandleState3();
    }

    void HandleState3()
    {
        if (HasThePlayerSteppedOnMe())
        {
            PlayerKilledEvent.Invoke();
        }
        if (HasSomeEnemySteppedOnMe())
        {
            var enemy = m_board.Enemies.Find((e) => e.GetNode().Coordinates == Utility.Vector3Round(transform.position));
            enemy.FallAndKill();
        }
    }
}

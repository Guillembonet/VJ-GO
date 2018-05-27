using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBlock : MonoBehaviour {

    bool m_activated = false;
    Board m_board;

    void Start()
    {
        m_board = GameObject.FindObjectOfType<Board>();
    }

    public void Activate()
    {
        if (!m_activated)
        {
            m_activated = true;
            StartCoroutine(ActivateRoutine());
        }
    }

    IEnumerator ActivateRoutine()
    {
        Node destNode = m_board.FindNodeAt(transform.position - new Vector3(0f, Board.spacing*3f, 0f));
        if (destNode != null)
        {
            destNode.RemoveNeighbors(false);
        }
        iTween.MoveTo(gameObject, iTween.Hash(
            "y", transform.position.y - Board.spacing * 2f,
            "easetype", iTween.EaseType.easeOutBounce,
            "time", 1.0f
        ));
        List<IEnemy> killedEnemies = m_board.Enemies.FindAll(e => e.GetNode() == destNode);
        foreach( IEnemy e in killedEnemies)
        {
            e.Kill();
        }
        yield return new WaitForSeconds(1.1f);
    }
}

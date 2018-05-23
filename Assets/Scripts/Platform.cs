using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, ActivatedObject {

    Vector3 m_originalPos;
    Node m_node;

    void Start()
    {
        m_originalPos = transform.position;
        m_node = GetComponentInChildren<Node>();
    }

    public void Activate()
    {
        StartCoroutine(ActivateRoutine());
        
    }

    IEnumerator ActivateRoutine()
    {
        if (transform.position == m_originalPos)
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "y", m_originalPos.y + Board.spacing * 2f,
                "easetype", iTween.EaseType.easeOutBack,
                "time", 1.0f
            ));
            yield return new WaitForSeconds(1f);
            m_node.StartNode();
        }
        else
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "y", m_originalPos.y,
                "easetype", iTween.EaseType.easeOutBack,
                "time", 1.0f
            ));
        }
    }
}

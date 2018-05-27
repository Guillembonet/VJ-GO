using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, ActivatedObject {

    Vector3 m_originalPos;
    Node m_node;
    public string Axis = "y";
    public int NBlocksMoved = 2;

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
            float originalPos;
            if (Axis.Equals("x")) originalPos = m_originalPos.x;
            else if (Axis.Equals("y")) originalPos = m_originalPos.y;
            else if (Axis.Equals("z")) originalPos = m_originalPos.z;
            else originalPos = m_originalPos.x;
            m_node.RemoveNeighbors();
            iTween.MoveTo(gameObject, iTween.Hash(
                Axis, originalPos + Board.spacing * NBlocksMoved,
                "easetype", iTween.EaseType.easeOutBack,
                "time", 1.0f
            ));
            yield return new WaitForSeconds(1.1f);
            m_node.StartNode();
        }
        else
        {
            m_node.RemoveNeighbors();
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", m_originalPos.x,
                "y", m_originalPos.y,
                "z", m_originalPos.z,
                "easetype", iTween.EaseType.easeOutBack,
                "time", 1.0f
            ));
            yield return new WaitForSeconds(1.1f); 
            m_node.StartNode();
        }
    }
}

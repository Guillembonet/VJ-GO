using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public static float spacing = 2f;
    public static readonly Vector3[] directions =
    {
        new Vector3(spacing, 0f, 0f),
        new Vector3(-spacing, 0f, 0f),
        new Vector3(0f, 0f, spacing),
        new Vector3(0f, 0f, -spacing),
        new Vector3(-spacing/2f, spacing/2f, 0f),
        new Vector3(spacing/2f, -spacing/2f, 0f),
        new Vector3(0f, -spacing/2f, spacing/2f),
        new Vector3(0f, spacing/2f, -spacing/2f),
        new Vector3(0f, -spacing, 0f),
        new Vector3(0f, spacing, 0f)
    };

    List<Node> m_allNodes = new List<Node>();
    public List<Node> allNodes { get { return m_allNodes; } }

    Node m_playerNode;
    public Node PlayerNode { get { return m_playerNode; } }

    Node m_goalNode;
    public Node GoalNode { get { return m_goalNode; } }

    public GameObject goalPrefab;
    public float drawGoalTime = 2f;
    public float drawGoalDelay = 2f;
    public iTween.EaseType drawGoalEaseType = iTween.EaseType.easeOutExpo;

    PlayerMover m_player;
    void Awake()
    {
        m_player = Object.FindObjectOfType<PlayerMover>().GetComponent<PlayerMover>();
        GetNodeList();

        m_goalNode = FindGoalNode();
    }

    public void GetNodeList()
    {
        Node[] nList = GameObject.FindObjectsOfType<Node>();
        m_allNodes = new List<Node>(nList);
    }

    public Node FindNodeAt(Vector3 pos)
    {
        Vector3 boardCoord = Utility.Vector3Round(pos);
        return m_allNodes.Find(n => n.transform.position == boardCoord);
    }

    Node FindGoalNode()
    {
        return m_allNodes.Find(n => n.isLevelGoal);
    }

    public void DrawGoal()
    {
        if (goalPrefab != null && m_goalNode != null)
        {
            GameObject goalInstance = Instantiate(goalPrefab, m_goalNode.transform.position, Quaternion.identity);
            iTween.ScaleFrom(goalInstance, iTween.Hash(
                "scale", Vector3.zero,
                "time", drawGoalTime,
                "delay", drawGoalDelay,
                "easeType", drawGoalEaseType
            ));
        }
    }

    public Node FindPlayerNode()
    {
        if (m_player != null && !m_player.isMoving)
        {
            return FindNodeAt(m_player.transform.position);
        }
        return null;
    }

    public Node FindZombieNode(ZombieMover zm)
    {
        if (zm != null && !zm.isMoving)
        {
            return FindNodeAt(zm.transform.position);
        }
        return null;
    }

    public void UpdatePlayerNode()
    {
        m_playerNode = FindPlayerNode();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
        if (m_playerNode != null)
        {
            Gizmos.DrawSphere(m_playerNode.transform.position, 0.2f);
        }
    }

    public void InitBoard()
    {
        if (m_playerNode != null)
        {
            m_playerNode.InitNode();
        }
    }
}

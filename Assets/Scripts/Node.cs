using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    List<Node> m_neighborNodes = new List<Node>();
    public List<Node> NeighborNodes { get { return m_neighborNodes; } }

    List<Node> m_linkedNodes = new List<Node>();
    public List<Node> LinkedNodes { get { return m_linkedNodes; } }

    Board m_board;

    public GameObject geometry;
    public GameObject linkPrefab;

    public float scaleTime = 0.3f;
    public iTween.EaseType easeType = iTween.EaseType.easeInExpo;

    bool m_isInitialized = false;

    public float delay = 1f;

    public LayerMask obstacleLayer;

    public bool isLevelGoal = false;
    private void Awake()
    {
        m_board = GameObject.FindObjectOfType<Board>();
    }
	void Start () {
		if (geometry != null)
        {
            geometry.transform.localScale = Vector3.zero;

            if (m_board != null) m_neighborNodes = FindNeighbors(m_board.allNodes);
        }
	}

    public void ShowGeometry()
    {
        if (geometry != null)
        {
            iTween.ScaleTo(geometry, iTween.Hash(
                "time", scaleTime,
                "scale", Vector3.one,
                "delay", delay,
                "easetype", easeType
            ));
        }
    }

    public List<Node> FindNeighbors(List<Node> nodes)
    {
        List<Node> nList = new List<Node>();
        foreach (Vector3 d in Board.directions)
        {
            Node foundNeighbour = nodes.Find(n => n.transform.position == transform.position + d);
            if (foundNeighbour != null && !nList.Contains(foundNeighbour)) nList.Add(foundNeighbour);
        }
        return nList;
    }

    public void InitNode()
    {
        if (!m_isInitialized)
        {
            ShowGeometry();
            InitNeighbors();
            m_isInitialized = true;
        }
    }

    void InitNeighbors()
    {
        StartCoroutine(InitNeighborsRoutine());
    }

    IEnumerator InitNeighborsRoutine()
    {
        yield return new WaitForSeconds(delay);

        foreach (Node n in m_neighborNodes)
        {
            if (!m_linkedNodes.Contains(n))
            {
                Obstacle obstacle = FindObstacle(n);
                if (obstacle == null)
                {
                    LinkNode(n);
                    n.InitNode();
                }
            }
        }
    }

    void LinkNode(Node targetNode)
    {
        if (linkPrefab != null)
        {
            if (AreDiagonallyAligned(transform.position, targetNode.transform.position))
            {
                GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity);
                GameObject linkInstance2 = Instantiate(linkPrefab, transform.position, Quaternion.identity);
                linkInstance.transform.parent = transform;
                linkInstance2.transform.parent = transform;


                if (transform.rotation.x == 0f && transform.rotation.z == 0f)
                {
                    Link link = linkInstance.GetComponent<Link>();
                    if (link != null)
                    {
                        link.DrawLinktoWall(transform.position, new Vector3(targetNode.transform.position.x, transform.position.y, targetNode.transform.position.z));
                    }

                    Link link2 = linkInstance2.GetComponent<Link>();
                    if (link2 != null)
                    {
                        link2.DrawLinkfromWall(new Vector3(targetNode.transform.position.x, transform.position.y, targetNode.transform.position.z), targetNode.transform.position);
                    }
                } else
                {
                    Link link = linkInstance.GetComponent<Link>();
                    if (link != null)
                    {
                        link.DrawLinktoWall(transform.position, new Vector3(transform.position.x, targetNode.transform.position.y, transform.position.z));
                    }

                    Link link2 = linkInstance2.GetComponent<Link>();
                    if (link2 != null)
                    {
                        link2.DrawLinkfromWall(new Vector3(transform.position.x, targetNode.transform.position.y, transform.position.z), targetNode.transform.position);
                    }
                }
                

                if (!m_linkedNodes.Contains(targetNode))
                    m_linkedNodes.Add(targetNode);
                if (!targetNode.LinkedNodes.Contains(this))
                    targetNode.LinkedNodes.Add(this);
            } else
            {
                GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity);
                linkInstance.transform.parent = transform;

                Link link = linkInstance.GetComponent<Link>();
                if (link != null)
                {
                    link.DrawLink(transform.position, targetNode.transform.position);
                }

                if (!m_linkedNodes.Contains(targetNode))
                    m_linkedNodes.Add(targetNode);
                if (!targetNode.LinkedNodes.Contains(this))
                    targetNode.LinkedNodes.Add(this);
            }
        }
    }

    bool AreVerticallyAligned(Vector3 start, Vector3 end)
    {
        if (start.y - end.y != 0f && start.x == end.x && start.z == end.z) return true;
        else return false;
    }

    bool AreDiagonallyAligned(Vector3 start, Vector3 end)
    {
        if (start.y - end.y != 0f && (start.x != end.x || start.z != end.z)) return true;
        else return false;
    }

    Obstacle FindObstacle(Node targetNode)
    {
        Vector3 checkDirection = targetNode.transform.position - transform.position;
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, checkDirection, out raycastHit, Board.spacing + 0.1f, obstacleLayer))
        {
            //Debug.Log("Obstacle hit");
            return raycastHit.collider.GetComponent<Obstacle>();
        }
        return null;
    }
}

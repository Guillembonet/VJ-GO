using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    List<Node> m_neighborNodes = new List<Node>();
    public List<Node> NeighborNodes { get { return m_neighborNodes; } }

    List<Node> m_linkedNodes = new List<Node>();
    public List<Node> LinkedNodes { get { return m_linkedNodes; } }

    public Vector3 Coordinates { get { return Utility.Vector3Round(transform.position); } }

    Board m_board;

    public GameObject geometry;
    public GameObject linkPrefab;

    public float scaleTime = 0.3f;
    public iTween.EaseType easeType = iTween.EaseType.easeInExpo;

    bool m_isInitialized = false;

    public float delay = 1f;

    public LayerMask obstacleLayer;

    public bool isLevelGoal = false;
    public bool wall = false;
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

    public void StartNode()
    {
        if (m_board != null) m_neighborNodes = FindNeighbors(m_board.allNodes);
        InitNode(true);
    }

    public void RemoveNeighbors()
    {
        foreach(Node n in m_neighborNodes)
        {
            n.NeighborNodes.Remove(this);
            n.LinkedNodes.Remove(this);
        }
        m_neighborNodes.Clear();
        m_linkedNodes.Clear();
        List<Link> links = new List<Link>(GetComponentsInChildren<Link>());
        Debug.Log(links.Count);
        foreach(Link l in links)
        {
            Destroy(l.linkObject);
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

    public void InitNode(bool reinitialize = false)
    {
        if (!m_isInitialized || reinitialize)
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
            GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity);
            linkInstance.transform.parent = transform;

            if (AreDiagonallyAligned(transform.position, targetNode.transform.position)) //when the line finds an edge
            {
                GameObject linkInstance2 = Instantiate(linkPrefab, transform.position, Quaternion.identity);
                linkInstance2.transform.parent = transform;


                if (transform.rotation.x == 0f && transform.rotation.z == 0f) //if the first node is not on a wall
                {
                    Link link = linkInstance.GetComponent<Link>();
                    if (link != null)
                    {
                        //draw horizontal line to wall
                        link.DrawLinktoWall(transform.position, new Vector3(targetNode.transform.position.x, transform.position.y, targetNode.transform.position.z));
                    }

                    Link link2 = linkInstance2.GetComponent<Link>();
                    if (link2 != null)
                    {
                        //draw vertical line from edge
                        link2.DrawLinkfromWall(new Vector3(targetNode.transform.position.x, transform.position.y, targetNode.transform.position.z), targetNode.transform.position);
                    }
                } else
                {
                    Link link = linkInstance.GetComponent<Link>();
                    if (link != null)
                    {
                        //draw vertical line to edge
                        link.DrawLinktoWall(transform.position, new Vector3(transform.position.x, targetNode.transform.position.y, transform.position.z));
                    }

                    Link link2 = linkInstance2.GetComponent<Link>();
                    if (link2 != null)
                    {
                        //draw horizontal line from edge
                        link2.DrawLinkfromWall(new Vector3(transform.position.x, targetNode.transform.position.y, transform.position.z), targetNode.transform.position);
                    }
                }
            } else //when the line doesn't find an edge
            {
                Link link = linkInstance.GetComponent<Link>();
                if (link != null)
                {
                    link.DrawLink(transform.position, targetNode.transform.position);
                }
            }

            //add nodes to linkednodes list
            if (!m_linkedNodes.Contains(targetNode))
                m_linkedNodes.Add(targetNode);
            if (!targetNode.LinkedNodes.Contains(this))
                targetNode.LinkedNodes.Add(this);
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

    public Node GetLinkedNodeInDirection(Vector3 direction)
    {
        return m_linkedNodes.Find(n => n.Coordinates == Utility.Vector3Round(transform.position + (direction * Board.spacing))
                || n.Coordinates == transform.position + (direction * (Board.spacing / 2f)) + new Vector3(0f, Board.spacing / 2f, 0f)
                || n.Coordinates == transform.position + (direction * (Board.spacing / 2f)) - new Vector3(0f, Board.spacing / 2f, 0f));
    }
}

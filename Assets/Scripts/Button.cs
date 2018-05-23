using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    Board m_board;
    bool m_activated = false;
    public Platform targetPlatform;
    public Platform targetBlock;

    // Use this for initialization
    void Start () {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    public void CheckButton()
    {
        if (Utility.Vector3Round(m_board.PlayerNode.Coordinates) == transform.position || m_board.Enemies.Exists(e => e.GetNode().Coordinates == transform.position))
        {
            if (targetPlatform != null)
            {
                targetPlatform.Activate();
            }
        }
    }
}

using UnityEngine;

public class Link : MonoBehaviour {

    public float borderWidth = 0.02f;
    public float lineThickness = 0.5f;
    public float scaleTime = 0.25f;
    public float delay = 0.1f;
    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;
    public iTween.EaseType easeTypeIn = iTween.EaseType.easeInExpo;
    public iTween.EaseType easeTypeOut = iTween.EaseType.easeOutExpo;
    public GameObject linkObject;

    Board m_board;
    void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    public void DrawLink(Vector3 startPos, Vector3 endPos)
    {
        Node a = m_board.FindNodeAt(Utility.Vector3Round(startPos));

        Vector3 vectorUp = new Vector3(0, 1, 0);
        if (Utility.Vector3Round(a.transform.forward) == new Vector3(0, -1, 0))
            vectorUp = new Vector3(0, 0, 1);
        else if (Utility.Vector3Round(a.transform.forward) == new Vector3(0, 0, -1))
            vectorUp = new Vector3(1, 0, 0);
        else if (Utility.Vector3Round(a.transform.forward) == new Vector3(0, 0, 1))
            vectorUp = new Vector3(0, 1, 0);

        linkObject = gameObject;
        transform.localScale = new Vector3(lineThickness, 1f, 0f);
        Vector3 dirVector = endPos - startPos;
        //dirVector = new Vector3(dirVector.x, dirVector.y, dirVector.z - 0.2f);
        float zScale = dirVector.magnitude - borderWidth * 2f;
        Vector3 newScale = new Vector3(lineThickness, 1f, zScale);
        transform.rotation = Quaternion.LookRotation(dirVector, vectorUp);
        if (dirVector.x == 0f && dirVector.z == 0)
            transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
        transform.position = startPos + (transform.forward * borderWidth);

        iTween.ScaleTo(gameObject, iTween.Hash(
            "time", scaleTime,
            "scale", newScale,
            "easetype", easeType,
            "delay", delay
        ));
    }

    public void DrawGrossLink(Vector3 startPos, Vector3 endPos)
    {
        /* IMPORTANT: We suppose they are not diagonally oriented */
        Node a = m_board.FindNodeAt(Utility.Vector3Round(startPos));

        Vector3 vectorUp = new Vector3();
        if (Utility.Vector3Round(a.transform.forward) == new Vector3(0, -1, 0))
            vectorUp = new Vector3(0, 0, 1);
        else if (Utility.Vector3Round(a.transform.forward) == new Vector3(0, 0, -1))
            vectorUp = new Vector3(1, 0, 0);
        else if (Utility.Vector3Round(a.transform.forward) == new Vector3(0, 0, 1))
            vectorUp = new Vector3(0, 1, 0);

        linkObject = gameObject;
        transform.localScale = new Vector3(lineThickness * 3, 1f, 0f);
        Vector3 dirVector = endPos - startPos;
        //dirVector = new Vector3(dirVector.x, dirVector.y, dirVector.z - 0.2f);
        float zScale = dirVector.magnitude - borderWidth * 2f;
        Vector3 newScale = new Vector3(lineThickness * 3, 1f, zScale);
        transform.rotation = Quaternion.LookRotation(dirVector, vectorUp);
        if (dirVector.x == 0f && dirVector.z == 0)
            transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
        transform.position = startPos + (transform.forward * borderWidth);

        iTween.ScaleTo(gameObject, iTween.Hash(
            "time", scaleTime,
            "scale", newScale,
            "easetype", easeType,
            "delay", delay
        ));
    }

    public void DrawLinktoWall(Vector3 startPos, Vector3 endPos)
    {
        linkObject = gameObject;
        transform.localScale = new Vector3(lineThickness, 1f, 0f);
        Vector3 dirVector = endPos - startPos;
        float zScale = dirVector.magnitude - borderWidth;
        Vector3 newScale = new Vector3(lineThickness, 1f, zScale);
        transform.rotation = Quaternion.LookRotation(dirVector);
        if (dirVector.x == 0f && dirVector.z == 0)
            transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
        transform.position = startPos + (transform.forward * borderWidth);

        iTween.ScaleTo(gameObject, iTween.Hash(
            "time", scaleTime/2f,
            "scale", newScale,
            "easetype", easeTypeIn,
            "delay", delay
        ));
    }

    public void DrawLinkfromWall(Vector3 startPos, Vector3 endPos)
    {
        linkObject = gameObject;
        transform.localScale = new Vector3(lineThickness, 1f, 0f);
        Vector3 dirVector = endPos - startPos;
        float zScale = dirVector.magnitude - borderWidth;
        Vector3 newScale = new Vector3(lineThickness, 1f, zScale);
        transform.rotation = Quaternion.LookRotation(dirVector);
        if (dirVector.x == 0f && dirVector.z == 0)
            transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
        transform.position = startPos;

        iTween.ScaleTo(gameObject, iTween.Hash(
            "time", scaleTime / 2f,
            "scale", newScale,
            "easetype", easeTypeOut,
            "delay", delay + scaleTime / 2f
        ));
    }
}

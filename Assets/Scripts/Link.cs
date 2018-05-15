using UnityEngine;

public class Link : MonoBehaviour {

    public float borderWidth = 0.02f;
    public float lineThickness = 0.5f;
    public float scaleTime = 0.25f;
    public float delay = 0.1f;
    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;
    public iTween.EaseType easeTypeIn = iTween.EaseType.easeInExpo;
    public iTween.EaseType easeTypeOut = iTween.EaseType.easeOutExpo;

    public void DrawLink(Vector3 startPos, Vector3 endPos)
    {
        transform.localScale = new Vector3(lineThickness, 1f, 0f);
        Vector3 dirVector = endPos - startPos;
        //dirVector = new Vector3(dirVector.x, dirVector.y, dirVector.z - 0.2f);
        float zScale = dirVector.magnitude - borderWidth * 2f;
        Vector3 newScale = new Vector3(lineThickness, 1f, zScale);
        transform.rotation = Quaternion.LookRotation(dirVector);
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

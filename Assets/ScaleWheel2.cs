using UnityEngine;
using System.Collections;

public class ScaleCube2 : MonoBehaviour
{

    Vector3 minScale2 = new Vector3(0.1f, 0.1f, 0.1f);
    Vector3 maxScale2 = new Vector3(3, 3, 3);

    // Update is called once per frame
    void Update()
    {
        float zoomValue = Input.GetAxis("Mouse ScrollWheel");

        if (zoomValue != 0 && CursorOnMe())
        {
            transform.localScale += Vector3.one * zoomValue;
            transform.localScale = Vector3.Max(transform.localScale, minScale2);
            transform.localScale = Vector3.Min(transform.localScale, maxScale2);
        }
    }

    bool CursorOnMe()
    {
        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] cursorHits = Physics.RaycastAll(cursorRay);
        for (int n = 0; n < cursorHits.Length; n++)
        {
            if (cursorHits[n].collider == GetComponent<Collider>())
            {
                //HIT!
                return true;
            }
        }
        return false;
    }
}
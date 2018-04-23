using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MakeSphericalGrid : MonoBehaviour
{
    public GameObject prefab;

    [Header("Size")]
    public int columns = 40;
    public int rows = 5;
    public float objectSpacing = .05f;

    [Header("Circular Setup")]
    public Vector3 centerPos = new Vector3(0, 0, 0);//center of circle/elipsoid
    public float radiusX = 1f;
    public float radiusY = 1f;
    public float radiusZ = 1f;
    public float span = 360;//portion of circle/sphere to create

    private GameObject go;

    private float rowHeight;


    void Start()
    {
        //set distance between rows
        rowHeight = prefab.GetComponent<Renderer>().bounds.size.x + objectSpacing;

        span = 360 / span;
        if (span <= 1f)
            span = 1f;


        //LOOP
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < columns; col++)
            {

                float pointNum = (col * 1.0f) / columns;

                //angle along the unit circle for placing points
                float angle = pointNum * Mathf.PI * 2 / span;


                float x = Mathf.Sin(angle) * radiusX;
                float z = Mathf.Cos(angle) * radiusZ;


                Vector3 pointPos = new Vector3(x, row * rowHeight, z) + centerPos;

                go = Instantiate(prefab, pointPos, Quaternion.identity, transform);
                go.transform.LookAt(centerPos);

            }
        }

    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowObject : MonoBehaviour {

    public Vector3 curpos;
    public Vector3 curpos1;
    public Vector3 temp = new Vector3(0, 1, 1);
    public Vector3 tempY = new Vector3(0, 1, 1);
    public GameObject bomb;
    public GameObject bomb1;
    public float timer;
    public float cimer;
    public int maxObjects;
    public int maxObjects2;
    private float rowHeight;
    public int columns = 4;
    public int rows = 4;
    public float objectSpacing = 2.05f;

    // Use this for initialization
    void Start()
    {
        rowHeight = bomb.GetComponent<Renderer>().bounds.size.x + objectSpacing;

        for (var row = 0; row < rows; row++)
        {
            row += 2;
            //curpos = new Vector3(0, curpos.y, columns);
            for (var col = 0; col < columns; col++)
            {

                curpos = new Vector3(0, row, curpos.z + columns);


                Instantiate(bomb, curpos, transform.rotation);

            }
        }







    }

    // Update is called once per frame
    //void Update()
    //{

    //    if(maxObjects>0)
    //    cimer += Time.deltaTime * 2;
    //    timer += Time.deltaTime;
    //    if (timer < 100f)
    //    {
    //        if (cimer > 2f)
    //        {

    //            curpos = transform.position += temp;
    //            Instantiate(bomb, curpos, transform.rotation);
    //            cimer = 0f;
    //            maxObjects -= 1;

    //        }
    //    }
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTEst : MonoBehaviour {

    [Range(-1,1)]
    public float qX;
    [Range(-1, 1)]
    public float qY;
    [Range(-1, 1)]
    public float qZ;
    [Range(-1, 1)]
    public float qW;

	// Update is called once per frame
	void Update () {

        transform.rotation = new Quaternion(qX, qY, qZ, qW);
	}
}

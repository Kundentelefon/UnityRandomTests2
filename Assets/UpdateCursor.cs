using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCursor : MonoBehaviour {

    HandleCursor cursor;
    bool carrying;

	// Use this for initialization
	void Start () {
        cursor = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<HandleCursor>();
	}
	
	// Update is called once per frame
	void Update () {
        if (carrying)
            cursor.setGrab();
	}

    private void OnMouseEnter()
    {
        cursor.setHand();
    }

    private void OnMouseExit()
    {
        cursor.setMouse();
    }

    private void OnMouseDown()
    {
        carrying = true;
    }

    private void OnMouseUp()
    {
        carrying = false;
        cursor.setMouse();
    }
}

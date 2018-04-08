using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCursor : MonoBehaviour {

    public Texture2D mouse;
    public Texture2D hand;
    public Texture2D grab;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setMouse()
    {
        Cursor.SetCursor(mouse, hotSpot, cursorMode);
    }
    public void setHand()
    {
        Cursor.SetCursor(mouse, hotSpot, cursorMode);
    }
    public void setGrab()
    {
        Cursor.SetCursor(mouse, hotSpot, cursorMode);
    }
}

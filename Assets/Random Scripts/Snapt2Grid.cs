using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapt2Grid : MonoBehaviour {

    private Vector3 screenPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

        Cursor.visible = false;


    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

        transform.position = SnapToGrid(curPosition);

    }

    void OnMouseUp()
    {
        Cursor.visible = true;

    }

    Vector3 SnapToGrid(Vector3 Position)
    {
        GameObject grid = GameObject.Find("grid");
        if (!grid)
            return Position;

        //	get grid size from the texture tiling
        Vector2 GridSize = grid.GetComponent<Renderer>().material.mainTextureScale;

        //	get position on the quad from -0.5...0.5 (regardless of scale)
        Vector3 gridPosition = grid.transform.InverseTransformPoint(Position);
        //	scale up to a number on the grid, round the number to a whole number, then put back to local size
        gridPosition.x = Mathf.Round(gridPosition.x * GridSize.x) / GridSize.x;
        gridPosition.y = Mathf.Round(gridPosition.y * GridSize.y) / GridSize.y;

        //	don't go out of bounds
        gridPosition.x = Mathf.Min(0.5f, Mathf.Max(-0.5f, gridPosition.x));
        gridPosition.y = Mathf.Min(0.5f, Mathf.Max(-0.5f, gridPosition.y));

        Position = grid.transform.TransformPoint(gridPosition);
        return Position;

        // extents is half the size of the bounding box (in world space, so do this AFTER TransformPoint())
        //Position.y -= this.renderer.bounds.extents.y;
        
    }
}
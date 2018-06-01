using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//to get options on the Inspector editor
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        //reference to MapGenerator
        MapGenerator mapGen = (MapGenerator)target;

        //if any value ever changed
        if (DrawDefaultInspector())
        {
            if(mapGen.autoUpdate)
                mapGen.DrawMapInEditor();
        }
        //add in a button
        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }
    }

}

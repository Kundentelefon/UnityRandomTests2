using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//put on Map generator gameobject
//take the noisemap and return it as a Texture and apply this texture on a plane in the scene
public class MapDisplay : MonoBehaviour {

    //reference to the renderer of the plane with its texture
    //apply on plane wit Map Mat (unlit texture shader)
    public Renderer textureRenderer;

    //Drawing a texture to the screen
    public void DrawTexture(Texture2D texture)
    {

        //apply  the texture to the texture renderer (generate inside editor and not until play mode)
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);

        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator {

    //methode to create a Texture out of a 1d colormap
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        //fix the blurryness at the corners of the regions
        texture.filterMode = FilterMode.Point;
        //change the Wrapmode so the Texture wont repeat at the other side
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        //apply the colour of each of the pixel in the texture
        texture.Apply();
        return texture;
    }

    //getting texture of 2D heightmap
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        //get the dimensions of the 2D float
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        //its faster to generate an array of all of the pixel and set them all at once
        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //set the colour map, first we need the index of the colormap because it is a single array, get the index by y*width(row) + x (columN)
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColourMap(colourMap, width, height);

    }
}

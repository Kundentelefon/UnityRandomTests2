using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//put on Map Generator GameObject
public class MapGenerator : MonoBehaviour {

    //Drawmode between black and white or colorMap
    public enum DrawMode {NoiseMap, ColourMap, FalloffMap};
    public DrawMode drawMode;

    //values to define the map
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    //So that the prng always displays the same with the same seed
    public int seed;
    //to scroll through the Noise
    public Vector2 offset;

    public bool useFalloff;
    public Vector2 falloffValues = new Vector2(3, 2.2f);

    //Updates whenever a change in the Inspector happens
    public bool autoUpdate;

    public TerrainType[] regions;
    
    //store the falloffMap
    float[,] falloffMap;

    void Awake()
    {
        //only create ones
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight, falloffValues);
        DrawMapInEditor();

    }

    //drawing the Map in the Editor
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData();
        //process the noisemap to show it on terrainmap
        //call the mapdisplay with the noiseMap, -> refernce to mapdisplay
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        else if (drawMode == DrawMode.ColourMap)
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapWidth, mapHeight));
        else if (drawMode == DrawMode.FalloffMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight, falloffValues)));
    }

    //fetching the 2D NoiseMap to the NoiseClass
    MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        //create a 1d colorMap out of 2d NoiseMap
        Color[] colourMap = new Color[mapWidth * mapHeight];

        //loop through noiseMap and give values
        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                
                if (useFalloff)
                {
                    //subtract from the noiseMap the fallofMap between 0 and 1
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }

                //height at this point from the noisemap
                float currentHeight = noiseMap[x, y];
                //loop through all of the regions and see which  region the current height falls within
                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        //break if its already in this region is in the coordinate so we dont have to look the other regions
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap);
        
    }

    //let the values not go out of bounce
    private void OnValidate()
    {
        if(mapWidth < 1)
            mapWidth = 1;

        if(mapHeight < 1)
            mapHeight = 1;

        if (lacunarity < 1)
            lacunarity = 1;

        if (octaves < 0)
            octaves = 0;

        if (falloffValues.x < 0)
            falloffValues.x = 0.001f;

        if (falloffValues.y < 0)
            falloffValues.y = 0.001f;

        DrawMapInEditor();
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight, falloffValues);
        
    }
}
//Serializable so it will show up in the inspector
[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public float[,] heightMap;
    public Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}

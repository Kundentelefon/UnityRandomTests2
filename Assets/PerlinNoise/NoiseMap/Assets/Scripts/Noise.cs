using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise  {

    //Method to generate a Noise map
    //return a grid of  value between 0 and 1

    //Map has a lot of values like lacunarity and persistance value
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        //prng aus dem Seed
        System.Random prng = new System.Random(seed);
        //each octave to bee sampled from a different locations
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            //dont let give Mathf.PerlinNoise a number to high, because it keeps returning the same value over and over again
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

        }

        //handle 0 case so it wont divide by 0
        if (scale <= 0)
        {
            //clamp to min value
            scale = 0.0001f;
        }

        //keep track of the lowest and highest values to normalize it later
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //for zooming in the middle with scale
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        //loop through noiseMap
        for(int y =0; y < mapHeight; y++)
        {
            for(int x= 0; x< mapWidth; x++)
            {
                //frequencie and altitude variables
                float amplitude = 1;
                float frequency = 1;
                //keep track of current height value
                float noiseHeight = 0;

                //loop through all octaves
                for (int i = 0; i < octaves; i++) { 
                //sample coordinates and let frequency take effect, the higher the frequency the further apart the sample parts will be (height values will change more rapidly
                float sampleX = (x-halfWidth + octaveOffsets[i].x) / scale * frequency ;
                float sampleY = (y-halfHeight + octaveOffsets[i].y) / scale * frequency ;

                //use unitys perlinNoise function, to get the range between -1 to 1 => *2 -1
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    //increase the noiseHeight by the perlin value of each object
                    noiseHeight += perlinValue * amplitude;

                    //at the end of each octave, the amplitude get multiplied by the persistance value. It decreased because its range 0-1
                    amplitude *= persistance;
                    //frequency increases because lacunarity is over 1
                    frequency *= lacunarity;

                }

                //normalize it so all the values are between 0 and 1
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                //apply the noiseHeight to noiseMap
                noiseMap[x, y] = noiseHeight;
            }
        }

        //normalize it so all the values are between 0 and 1 for all
        //after we know which range the noiseMap values are in, than for each value in the noise map set them equal to InverseLerp (return between 0 and 1)
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

                return noiseMap;
    }
}

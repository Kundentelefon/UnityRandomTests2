using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator  {

    public static float[,] GenerateFalloffMap(int width, int height, Vector2 falloffValues)
    {
        float[,] map = new float[width, height];
        //pobulate the map with values
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //i and j are coordinates for points in the square map
                //range between -1 and 1
                float x = i / (float)width * 2 - 1;
                float y = j / (float)height * 2 - 1;

                //get value for format, find out which x or y is closest to edge
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value, falloffValues);
            }
        }
        return map;
    }

    static float Evaluate (float value, Vector2 falloffValues)
    {
        float a = falloffValues.x;
        float b = falloffValues.y;
        //Formula: x^a / ( (x^a) + (b - bx)^a )
        return Mathf.Pow(value, a)/ (Mathf.Pow(value, a) + Mathf.Pow(b-b*value, a));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helper
{
    public static float GetMedian(this IEnumerable<float> source)
    {
        // Create a copy of the input, and sort the copy
        float[] temp = source.ToArray();
        Array.Sort(temp);

        int count = temp.Length;
        if (count == 0)
        {
            throw new InvalidOperationException("Empty collection");
        }
        else if (count % 2 == 0)
        {
            // count is even, average two middle elements
            float a = temp[count / 2 - 1];
            float b = temp[count / 2];
            return (a + b) / 2f;
        }
        else
        {
            // count is odd, return the middle element
            return temp[count / 2];
        }
    }

    public static ButtonBhv GetFieldButton(string gameObjectName)
    {
        var main = GameObject.Find(gameObjectName);
        if (main == null)
            return null;
        var bar = main.transform.Find("Bar");
        if (bar == null)
            return null;
        return bar.GetComponent<ButtonBhv>();
    }

    public static TMPro.TextMeshPro GetFieldData(string gameObjectName)
    {
        var main = GameObject.Find(gameObjectName);
        if (main == null)
            return null;
        var bar = main.transform.Find("Bar");
        if (bar == null)
            return null;
        var data = bar.transform.Find("Data");
        if (data == null)
            return null;
        return data.GetComponent<TMPro.TextMeshPro>();
    }

    public static Camera GetMainCamera()
    {
        return Camera.allCameras.FirstOrDefault(c => c.name.ToLower().Contains("main"));
    }

    public static int RoundToClosestTable(int value, int table)
    {
        value += 100;
        var superior = value % table == 0 ? 0 : table - value % table;
        var inferior = value % table;
        if (superior == 0)
            return value - 100;
        else if (superior < inferior)
            return value + superior - 100;
        else
            return value - inferior - 100;
    }
}

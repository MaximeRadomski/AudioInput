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
}

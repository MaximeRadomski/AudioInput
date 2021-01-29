using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexComparer : IComparer<Peak>
{
    public int Compare(Peak a, Peak b)
    {
        return a.index.CompareTo(b.index);
    }
}

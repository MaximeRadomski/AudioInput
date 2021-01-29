using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmpComparer : IComparer<Peak>
{
    public int Compare(Peak a, Peak b)
    {
        return 0 - a.amplitude.CompareTo(b.amplitude);
    }
}

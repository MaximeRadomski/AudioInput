using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const float Pixel = 0.14285f;

    public const string AbjectAudioInputs = "AbjectAudioInputs";

    public static Color ColorBlackTransparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    public static Color ColorPlain = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public static Color ColorPlainTransparent = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    public static Color ColorPlainSemiTransparent = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    public static Color ColorPlainQuarterTransparent = new Color(1.0f, 1.0f, 1.0f, 0.25f);
    public static Color ColorBlack = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    public static int InputLayer = 0;
    public static bool DoubleClick = false;
    public static string ClickHistory = null;
    public static string LastEndActionClickedName = null;
    public static List<string> InputTopLayerNames = null;

    public static void SetLastEndActionClickedName(string name)
    {
        if (!DoubleClick) //Prevents triple click
            ClickHistory = LastEndActionClickedName;
        LastEndActionClickedName = name;

        if (ClickHistory == LastEndActionClickedName)
        {
            DoubleClick = true;
            ClickHistory = string.Empty; //Prevents triple click
        }
        else
            DoubleClick = false;
    }

    public static void IncreaseInputLayer(string name)
    {
        ++InputLayer;
        if (InputTopLayerNames == null)
            InputTopLayerNames = new List<string>();
        InputTopLayerNames.Add(name);
    }

    public static void DecreaseInputLayer()
    {
        --InputLayer;
        if (InputTopLayerNames == null)
            return;
        if (InputTopLayerNames.Count <= 0)
        {
            InputLayer = 0;
            return;
        }
        InputTopLayerNames.RemoveAt(InputTopLayerNames.Count - 1);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WindowsInput.Native;

public static class Helper
{
    private static NumberFormatInfo _nfi;
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

    public static string GetDescription(this Enum value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var description = value.ToString();
        var fieldInfo = value.GetType().GetRuntimeField(description);

        if (fieldInfo == null)
            return string.Empty;
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes.Length > 0)
        {
            description = attributes[0].Description;
        }

        return description;
    }

    public static AudioInputJson ToAudioInputJson(this AudioInput audioInput)
    {
        var json = new AudioInputJson();
        json.Name = audioInput.Name;
        json.Enabled = audioInput.Enabled;
        json.Hz0 = audioInput.Frequencies[0].ToString("F2");
        json.Hz1 = audioInput.Frequencies[1].ToString("F2");
        json.Hz2 = audioInput.Frequencies[2].ToString("F2");
        json.Hz3 = audioInput.Frequencies[3].ToString("F2");
        json.Hz4 = audioInput.Frequencies[4].ToString("F2");
        json.Peaks = audioInput.Peaks;
        json.MouseInputId = audioInput.MouseInput.GetHashCode();
        json.KeyboardInputId = audioInput.Key.GetHashCode();
        json.InputTypeId = audioInput.InputType.GetHashCode();
        json.Param = audioInput.Param;
        return json;
    }

    public static AudioInput ToAudioInput(this AudioInputJson json)
    {
        if (_nfi == null)
        {
            var cultureInfo = CultureInfo.CurrentCulture;
            _nfi = cultureInfo.NumberFormat;
        }

        var audioInput = new AudioInput();
        audioInput.Id = -1;
        audioInput.Name = json.Name;
        audioInput.Enabled = json.Enabled;
        audioInput.Frequencies = new List<float>
        {
            CustomFloatParse(json.Hz0),
            CustomFloatParse(json.Hz1),
            CustomFloatParse(json.Hz2),
            CustomFloatParse(json.Hz3),
            CustomFloatParse(json.Hz4)
        };
        audioInput.Peaks = json.Peaks;
        audioInput.MouseInput = (MouseInput)json.MouseInputId;
        audioInput.Key = (VirtualKeyCode)json.KeyboardInputId;
        audioInput.InputType = (InputType)json.InputTypeId;
        audioInput.Param = json.Param;
        return audioInput;
    }

    private static float CustomFloatParse(string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0.0f;
        str = str.Replace(".", _nfi.CurrencyDecimalSeparator);
        str = str.Replace(",", _nfi.CurrencyDecimalSeparator);
        if (str[str.Length - 1] == _nfi.CurrencyDecimalSeparator[0])
            str += "0";
        return float.Parse(str);
    }

    public static bool FloatEqualsPrecision(float float1, float float2, float precision)
    {
        return float1 >= float2 - precision && float1 <= float2 + precision;
    }

    public static IEnumerator ExecuteAfterDelay(float delay, Func<object> func, bool lockInputWhile = true)
    {
        if (lockInputWhile)
            Constants.InputLocked = true;
        yield return new WaitForSeconds(delay);
        func.Invoke();
        if (lockInputWhile)
            Constants.InputLocked = false;
    }

    public static bool IsMouseDirection(MouseInput mouseInput)
    {
        return mouseInput.GetHashCode() >= MouseInput.Up.GetHashCode() && mouseInput.GetHashCode() <= MouseInput.Right.GetHashCode();
    }
}

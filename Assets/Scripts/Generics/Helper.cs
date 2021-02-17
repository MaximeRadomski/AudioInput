using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WindowsInput.Native;

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

    public static AudioInputBo ToAudioInputBo(this AudioInput audioInput)
    {
        var bo = new AudioInputBo();
        bo.IdInScene = -1;
        bo.Enabled = audioInput.Enabled;
        bo.Hz = audioInput.Hz;
        bo.Peaks = audioInput.Peaks;
        bo.MouseInputId = audioInput.MouseInput.GetHashCode();
        bo.KeyId = audioInput.Key.GetHashCode();
        bo.InputTypeId = audioInput.InputType.GetHashCode();
        bo.Param = audioInput.Param;
        return bo;
    }

    public static AudioInput ToAudioInput(this AudioInputBo bo)
    {
        var audioInput = new AudioInput();
        audioInput.Id = -1;
        audioInput.Enabled = bo.Enabled;
        audioInput.Hz = bo.Hz;
        audioInput.Peaks = bo.Peaks;
        audioInput.MouseInput = (MouseInput)bo.MouseInputId;
        audioInput.Key = (VirtualKeyCode)bo.KeyId;
        audioInput.InputType = (InputType)bo.InputTypeId;
        audioInput.Param = bo.Param;
        return audioInput;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowsInput.Native;
using WindowsInput;

public class AudioInput
{
    public int Id;
    public string Name;
    public bool Enabled;
    public List<float> Frequencies;
    public int Peaks;
    public MouseInput Mouse;
    public VirtualKeyCode Key;
    public MouseInput Mouse2;
    public bool HasSecond;
    public VirtualKeyCode Key2;
    public InputType InputType;
    public float Param;
    public float TimeHeldParam;

    public AudioInput()
    {
        Enabled = true;
        Frequencies = new List<float>();
        for (int i = 0; i < 5; ++i)
        {
            Frequencies.Add(0.0f);
        }
        Peaks = 1;
        Key = VirtualKeyCode.NONAME;
        Mouse = MouseInput.None;
        HasSecond = false;
        Key2 = VirtualKeyCode.NONAME;
        Mouse2 = MouseInput.None;
        InputType = InputType.Tap;
        Param = 0.0f;
    }

    public DeviceType GetMainDevice()
    {
        if (Key != VirtualKeyCode.NONAME)
            return DeviceType.Keyboard;
        else if (Mouse != MouseInput.None)
            return DeviceType.Mouse;
        return DeviceType.None;
    }

    public DeviceType GetSecondDevice()
    {
        if (HasSecond)
        {
            if (Key2 != VirtualKeyCode.NONAME)
                return DeviceType.Keyboard;
            else if (Mouse2 != MouseInput.None)
                return DeviceType.Mouse;
            return DeviceType.None;
        }
        return DeviceType.None;
    }

    public AudioInput CloneKeys()
    {
        var clone = new AudioInput();

        clone.Id = Id;
        clone.Mouse = Mouse;
        clone.Key = Key;
        clone.HasSecond = HasSecond;
        clone.Mouse2 = Mouse2;
        clone.Key2 = Key2;

        return clone;
    }

    public AudioInput CloneFrequenciesParam()
    {
        var clone = CloneKeys();
        clone.Frequencies = new List<float>();
        for (int i = 0; i < 5; ++i)
        {
            clone.Frequencies.Add(Frequencies[i]);
        }
        clone.Param = Param;
        return clone;
    }

    public AudioInput CloneAll()
    {
        var clone = CloneFrequenciesParam();

        clone.Name = Name;
        clone.Enabled = Enabled;
        clone.Peaks = Peaks;
        clone.InputType = InputType;

        return clone;
    }

    public string KeyToString()
    {
        string str;
        var mainDevice = GetMainDevice();
        var secondDevice = GetSecondDevice();
        if (mainDevice == DeviceType.Keyboard)
            str = Key.ToString().ShortInput();
        else if (mainDevice == DeviceType.Mouse)
            str = Mouse.GetDescription().ShortInput();
        else
            str = "none";
        if (HasSecond)
        {
            if (secondDevice == DeviceType.Keyboard)
                str += $" + {Key2.ToString().ShortInput()}";
            else if (secondDevice == DeviceType.Mouse)
                str += $" + {Mouse2.ToString().ShortInput()}";
        }
        return str;
    }
}

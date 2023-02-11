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
    public MouseInput MouseInput;
    public VirtualKeyCode Key;
    public InputType InputType;
    public float Param;
    public float HiddenParam;

    public AudioInput()
    {
        Enabled = true;
        Frequencies = new List<float>();
        for (int i = 0; i < 5; ++i)
        {
            Frequencies.Add(0.0f);
        }
        Peaks = 1;
        MouseInput = MouseInput.None;
        Key = VirtualKeyCode.NONAME;
        InputType = InputType.Tap;
        Param = 0.0f;
    }

    public AudioInput Clone()
    {
        var clone = new AudioInput();

        clone.Id = Id;
        clone.Name = Name;
        clone.Enabled = Enabled;
        clone.Frequencies = new List<float>();
        for (int i = 0; i < 5; ++i)
        {
            clone.Frequencies.Add(Frequencies[i]);
        }
        clone.Peaks = Peaks;
        clone.MouseInput = MouseInput;
        clone.Key = Key;
        clone.InputType = InputType;
        clone.Param = Param;

        return clone;
    }
}

public class AudioInputJson
{
    public string Name;
    public bool Enabled;
    public string Hz0;
    public string Hz1;
    public string Hz2;
    public string Hz3;
    public string Hz4;
    public int Peaks;
    public int MouseInputId;
    public int KeyboardInputId;
    public int InputTypeId;
    public float Param;
}

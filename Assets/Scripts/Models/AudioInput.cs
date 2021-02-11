using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowsInput.Native;
using WindowsInput;

public class AudioInput
{
    public int IdInScene;
    public bool Enabled;
    public float Hz;
    public int Peaks;
    public MouseInput MouseInput;
    public VirtualKeyCode Key;
    public InputType InputType;
    public float Param;
    public float HiddenParam;

    public AudioInput()
    {
        Enabled = true;
        Hz = 0.00f;
        Peaks = 0;
        MouseInput = MouseInput.None;
        Key = VirtualKeyCode.NONAME;
        InputType = InputType.SingleTap;
        Param = 0.0f;
    }

    public AudioInput Clone()
    {
        var clone = new AudioInput();

        clone.IdInScene = IdInScene;
        clone.Enabled = Enabled;
        clone.Hz = Hz;
        clone.Peaks = Peaks;
        clone.MouseInput = MouseInput;
        clone.Key = Key;
        clone.InputType = InputType;
        clone.Param = Param;

        return clone;
    }
}

public class AudioInputBo
{
    public int IdInScene;
    public bool Enabled;
    public float Hz;
    public int Peaks;
    public int MouseInputId;
    public int KeyId;
    public int InputTypeId;
    public float Param;
}

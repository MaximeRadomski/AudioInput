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
    public VirtualKeyCode Key;
    public InputType InputType;
    public float Param;

    public AudioInput()
    {
        Enabled = true;
        Hz = 0.00f;
        Peaks = 0;
        Key = VirtualKeyCode.NONAME;
        InputType = InputType.SingleTap;
        Param = 0.0f;
    }
}

public class AudioInputBo
{
    public int IdInScene;
    public bool Enabled;
    public float Hz;
    public int Peaks;
    public int KeyId;
    public int InputTypeId;
    public float Param;
}

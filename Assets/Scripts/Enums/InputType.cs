using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum InputType
{
    [Description("Single Tap")]
    SingleTap,
    [Description("Custom Tap")]
    CustomTap,
    [Description("Held")]
    Held,
    [Description("Time Held")]
    TimeHeld
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum InputType
{
    [Description("Tap")]
    Tap,
    [Description("Custom Tap")]
    CustomTap,
    [Description("Held")]
    Held,
    [Description("Custom Held")]
    CustomHeld
}

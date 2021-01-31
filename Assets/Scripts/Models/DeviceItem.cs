using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceItem : Dropdown.OptionData
{
    public string id;
    public DeviceItem(in Lasp.DeviceDescriptor device)
      => (text, id) = (device.Name, device.ID);
}

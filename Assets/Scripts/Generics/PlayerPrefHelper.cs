using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefHelper : MonoBehaviour
{
    public const string PpHolder = "Holder";
    public const string PpLastSavedDevice = "LastSavedDevice";
    public const string PpLastSavedDeviceDefault = null;

    public static string GetLastSavedDeviceDefault()
    {
        var deviceName = PlayerPrefs.GetString(PpLastSavedDevice, PpLastSavedDeviceDefault);
        return deviceName;
    }

    public static void SetLastSavedDeviceDefault(string name)
    {
        PlayerPrefs.SetString(PpLastSavedDevice, name);
    }
}

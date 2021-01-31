using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel00Bhv : PanelBhv
{
    public IEnumerable<DeviceDescriptor> Devices;

    private SpectrumAnalyzer _spectrumAnalyzer;
    private AudioLevelTracker _audioLevelTracker;
    private string _lastSavedDeviceName;
    private DeviceDescriptor _currentDevice;

    private TMPro.TextMeshPro _idData;

    void Start()
    {
        Init();
        SetButtons();
    }

    public override void Init()
    {
        if (_hasInit)
            return;
        base.Init();
        _spectrumAnalyzer = GameObject.Find(Constants.AbjectAudioInputs).GetComponent<SpectrumAnalyzer>();
        _audioLevelTracker = GameObject.Find(Constants.AbjectAudioInputs).GetComponent<AudioLevelTracker>();
        _lastSavedDeviceName = PlayerPrefHelper.GetLastSavedDeviceDefault();
        _idData = Helper.GetFieldData("Id");

        Devices = AudioSystem.InputDevices;

        int i = 0;
        bool isSet = false;
        foreach (var device in Devices)
        {
            if ((_lastSavedDeviceName == null && i == 0)
                || (_lastSavedDeviceName != null && _lastSavedDeviceName == device.Name))
            {
                _lastSavedDeviceName = device.Name;
                SetDeviceId(device);
                isSet = true;
            }
            ++i;
        }
        if (!isSet)
        {
            i = 0;
            foreach (var device in Devices)
            {
                if (i == 0)
                {
                    _lastSavedDeviceName = device.Name;
                    SetDeviceId(device);
                    isSet = true;
                }
                else
                    break;
                ++i;
            }
        }
        _hasInit = true;
    }

    private void SetButtons()
    {
        Helper.GetFieldButton("Id").EndActionDelegate = SetDeviceIdPopup;
    }

    private void SetDeviceIdPopup()
    {
        if (!_hasInit)
            return;
        _instantiator.NewPopupDeviceId(transform.position, _currentDevice, SetDeviceId);
    }

    private object SetDeviceId(DeviceDescriptor device)
    {
        if (_currentDevice.IsValid == true && device.Name == _currentDevice.Name)
            return false;
        _currentDevice = device;
        _audioLevelTracker.deviceID = device.ID;
        _spectrumAnalyzer.deviceID = device.ID;
        _idData.text = device.Name.ToLower();
        return true;
    }
}

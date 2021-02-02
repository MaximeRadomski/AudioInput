using Lasp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Panel00Bhv : PanelBhv
{
    public IEnumerable<DeviceDescriptor> Devices;

    private SpectrumAnalyzer _spectrumAnalyzer;
    private AudioLevelTracker _audioLevelTracker;
    private string _lastSavedDeviceName;

    private DeviceDescriptor _currentDevice;
    private int _currentChannel;
    private float _hzOffset;
    private int _requiredFrames;
    private PeaksPriority _peaksPriority;
    private int _levelDynamicRange;
    private int _levelGain;
    private int _spectrumDynamicRange;
    private int _spectrumGain;


    private TMPro.TextMeshPro _idData;
    private TMPro.TextMeshPro _channelData;
    private TMPro.TextMeshPro _hzOffsetData;
    private TMPro.TextMeshPro _requiredFramesData;
    private TMPro.TextMeshPro _peaksPriorityData;
    private TMPro.TextMeshPro _levelDynamicRangeData;
    private TMPro.TextMeshPro _levelGainData;
    private TMPro.TextMeshPro _spectrumDynamicRangeData;
    private TMPro.TextMeshPro _spectrumGainData;

    void Start()
    {
        Init();
        SetButtons();
        LoadData();
    }

    public override void Init()
    {
        if (_hasInit)
            return;
        base.Init();
        _spectrumAnalyzer = GameObject.Find(Constants.AbjectAudioInputs).GetComponent<SpectrumAnalyzer>();
        _audioLevelTracker = GameObject.Find(Constants.AbjectAudioInputs).GetComponent<AudioLevelTracker>();

        //PlayerPrefs
        _lastSavedDeviceName = PlayerPrefHelper.GetLastSavedDeviceDefault();
        _currentChannel = PlayerPrefHelper.GetCurrentChannel();
        _hzOffset = PlayerPrefHelper.GetHzOffset();
        _requiredFrames = PlayerPrefHelper.GetRequiredFrames();
        _peaksPriority = PlayerPrefHelper.GetPeaksPriority();
        _levelDynamicRange = PlayerPrefHelper.GetLevelDynamicRange();
        _levelGain = PlayerPrefHelper.GetLevelGain();
        _spectrumDynamicRange = PlayerPrefHelper.GetSpectrumDynamicRange();
        _spectrumGain = PlayerPrefHelper.GetSpectrumGain();

        //FieldsData
        _idData = Helper.GetFieldData("Id");
        _channelData = Helper.GetFieldData("Channel");
        _hzOffsetData = Helper.GetFieldData("HzOffset");
        _requiredFramesData = Helper.GetFieldData("RequiredFrames");
        _peaksPriorityData = Helper.GetFieldData("PeaksPriority");
        _levelDynamicRangeData = Helper.GetFieldData("LevelDynamicRange");
        _levelGainData = Helper.GetFieldData("LevelGain");
        _spectrumDynamicRangeData = Helper.GetFieldData("SpectrumDynamicRange");
        _spectrumGainData = Helper.GetFieldData("SpectrumGain");

        Devices = AudioSystem.InputDevices;

        
        _hasInit = true;
    }

    private void SetButtons()
    {
        Helper.GetFieldButton("Id").EndActionDelegate = SetDeviceIdPopup;
        Helper.GetFieldButton("Channel").EndActionDelegate = SetChannelPopup;
        Helper.GetFieldButton("HzOffset").EndActionDelegate = SetHzOffsetPopup;
        Helper.GetFieldButton("RequiredFrames").EndActionDelegate = SetRequiredFramesPopup;
        Helper.GetFieldButton("PeaksPriority").EndActionDelegate = SetPeaksPriorityPopup;
        Helper.GetFieldButton("LevelDynamicRange").EndActionDelegate = SetLevelDynamicRangePopup;
        Helper.GetFieldButton("LevelGain").EndActionDelegate = SetLevelGainPopup;
        Helper.GetFieldButton("SpectrumDynamicRange").EndActionDelegate = SetSpectrumDynamicRangePopup;
        Helper.GetFieldButton("SpectrumGain").EndActionDelegate = SetSpectrumGainPopup;

        GameObject.Find("LevelDynamicRange-").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetLevelDynamicRange(Helper.RoundToClosestTable(_levelDynamicRange - 5, 5)); };
        GameObject.Find("LevelDynamicRange+").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetLevelDynamicRange(Helper.RoundToClosestTable(_levelDynamicRange + 5, 5)); };
        GameObject.Find("LevelGain-").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetLevelGain(Helper.RoundToClosestTable(_levelGain - 5, 5)); };
        GameObject.Find("LevelGain+").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetLevelGain(Helper.RoundToClosestTable(_levelGain + 5, 5)); };
        GameObject.Find("SpectrumDynamicRange-").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetSpectrumDynamicRange(Helper.RoundToClosestTable(_spectrumDynamicRange - 10, 10)); };
        GameObject.Find("SpectrumDynamicRange+").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetSpectrumDynamicRange(Helper.RoundToClosestTable(_spectrumDynamicRange + 10, 10)); };
        GameObject.Find("SpectrumGain-").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetSpectrumGain(Helper.RoundToClosestTable(_spectrumGain - 10, 10)); };
        GameObject.Find("SpectrumGain+").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetSpectrumGain(Helper.RoundToClosestTable(_spectrumGain + 10, 10)); };
    }

    private void LoadData()
    {
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

        SetHzOffset(_hzOffset);
        SetRequiredFrames(_requiredFrames);
        SetPeaksPriority(_peaksPriority.GetHashCode());
        SetLevelDynamicRange(_levelDynamicRange);
        SetLevelGain(_levelGain);
        SetSpectrumDynamicRange(_spectrumDynamicRange);
        SetSpectrumGain(_spectrumGain);
    }

    private object SetDeviceId(DeviceDescriptor device)
    {
        if (_currentDevice.IsValid == true && device.Name == _currentDevice.Name)
            return false;
        PlayerPrefHelper.SetLastSavedDeviceDefault(device.Name);
        _currentDevice = device;
        try
        {
            _audioLevelTracker.deviceID = device.ID;
            _spectrumAnalyzer.deviceID = device.ID;
            if (_currentChannel >= device.ChannelCount)
                SetChannel(0);
            else
                SetChannel(_currentChannel);
        }
        catch (Exception e)
        {
            if (e.Message.Contains("Stream"))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        _idData.text = device.Name.ToLower();
        return true;
    }

    private object SetChannel(float result)
    {
        var intResult = (int)result;
        if (intResult < 0)
            intResult = 0;
        else if (intResult >= _currentDevice.ChannelCount)
            intResult = _currentDevice.ChannelCount - 1;
        _currentChannel = intResult;
        _audioLevelTracker.channel = _currentChannel;
        _spectrumAnalyzer.channel = _currentChannel;
        PlayerPrefHelper.SetCurrentChannel(intResult);
        _channelData.text = _currentChannel.ToString();
        return true;
    }

    private object SetHzOffset(float offset)
    {
        if (offset < 0)
            offset = 0;
        _hzOffset = offset;
        PlayerPrefHelper.SetHzOffset(offset);
        _hzOffsetData.text = offset.ToString("F2");
        return true;
    }

    private object SetRequiredFrames(float value)
    {
        if (value < 1)
            value = 1;
        var intValue = (int)value;
        _requiredFrames = intValue;
        PlayerPrefHelper.SetRequiredFrames(intValue);
        _requiredFramesData.text = intValue.ToString();
        return true;
    }

    private object SetPeaksPriority(int id)
    {
        _peaksPriority = (PeaksPriority)id;
        PlayerPrefHelper.SetPeaksPriority(_peaksPriority);
        _peaksPriorityData.text = _peaksPriority.ToString().ToLower();
        return true;
    }

    private object SetLevelDynamicRange(float value)
    {
        var intValue = (int)value;
        if (intValue < 1)
            intValue = 1;
        else if (intValue > 40)
            intValue = 40;
        _levelDynamicRange = intValue;
        _audioLevelTracker.dynamicRange = intValue;
        PlayerPrefHelper.SetLevelDynamicRange(intValue);
        _levelDynamicRangeData.text = intValue.ToString();
        return true;
    }

    private object SetLevelGain(float value)
    {
        var intValue = (int)value;
        if (intValue < -10)
            intValue = -10;
        else if (intValue > 40)
            intValue = 40;
        _levelGain = intValue;
        _audioLevelTracker.gain = intValue;
        PlayerPrefHelper.SetLevelGain(intValue);
        _levelGainData.text = intValue.ToString();
        return true;
    }

    private object SetSpectrumDynamicRange(float value)
    {
        var intValue = (int)value;
        if (intValue < 1)
            intValue = 1;
        else if (intValue > 120)
            intValue = 120;
        _spectrumDynamicRange = intValue;
        _spectrumAnalyzer.dynamicRange = intValue;
        PlayerPrefHelper.SetSpectrumDynamicRange(intValue);
        _spectrumDynamicRangeData.text = intValue.ToString();
        return true;
    }

    private object SetSpectrumGain(float value)
    {
        var intValue = (int)value;
        if (intValue < -10)
            intValue = -10;
        else if (intValue > 120)
            intValue = 120;
        _spectrumGain = intValue;
        _spectrumAnalyzer.gain = intValue;
        PlayerPrefHelper.SetSpectrumGain(intValue);
        _spectrumGainData.text = intValue.ToString();
        return true;
    }

    private void SetDeviceIdPopup()
    {
        _instantiator.NewPopupDeviceId(transform.position, _currentDevice, SetDeviceId);
    }

    private void SetChannelPopup()
    {
        var content = $"From 0 to {_currentDevice.ChannelCount - 1}\n(current device range)";
        if (_currentDevice.ChannelCount == 1)
            content = "Your selected device has only one channel";
        _instantiator.NewPopupNumber(transform.position, "Channel", content, _currentChannel, 2, SetChannel);
    }

    private void SetHzOffsetPopup()
    {
        var content = $"0.01 = very strict\n5.00 = very loose";
        _instantiator.NewPopupNumber(transform.position, "Valid Hz Offset", content, _hzOffset, 2, SetHzOffset);
    }

    private void SetRequiredFramesPopup()
    {
        var content = $"1 = minimum, no required frames";
        _instantiator.NewPopupNumber(transform.position, "required frames", content, _requiredFrames, 2, SetRequiredFrames);
    }

    private void SetPeaksPriorityPopup()
    {
        _instantiator.NewPopupEnum<PeaksPriority>(transform.position, "peaks priority", _peaksPriority.GetHashCode(), SetPeaksPriority);
    }

    private void SetLevelDynamicRangePopup()
    {
        var content = $"from 1 to 40";
        _instantiator.NewPopupNumber(transform.position, "Level Dynamic Range", content, _levelDynamicRange, 2, SetLevelDynamicRange);
    }

    private void SetLevelGainPopup()
    {
        var content = $"from -10 to 40";
        _instantiator.NewPopupNumber(transform.position, "Level Gain", content, _levelGain, 2, SetLevelGain);
    }

    private void SetSpectrumDynamicRangePopup()
    {
        var content = $"from 1 to 120";
        _instantiator.NewPopupNumber(transform.position, "Spectrum Dynamic Range", content, _spectrumDynamicRange, 3, SetSpectrumDynamicRange);
    }

    private void SetSpectrumGainPopup()
    {
        var content = $"from -10 to 120";
        _instantiator.NewPopupNumber(transform.position, "Spectrum Gain", content, _spectrumGain, 3, SetSpectrumGain);
    }
}

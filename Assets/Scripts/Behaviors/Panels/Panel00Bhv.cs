using Lasp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WindowsInput.Native;

public class Panel00Bhv : PanelBhv
{
    public Sprite IconDown;
    public Sprite IconUp;

    public IEnumerable<DeviceDescriptor> Devices;
    public float HzOffset;
    public int RequiredFrames;
    public PeaksPriority PeaksPriority;
    public int HeldReset;
    public int SingleTapReset;

    private SpectrumAnalyzer _spectrumAnalyzer;
    private AudioLevelTracker _audioLevelTracker;
    private string _lastSavedDeviceName;
    private SpriteRenderer _mainIcon;
    private Transform _singleTapSpawn;
    private Transform _customTapSpawn;
    private Transform _heldSpawn;
    private Transform _timeHeldSpawn;

    private DeviceDescriptor _currentDevice;
    private int _currentChannel;
    private int _levelDynamicRange;
    private int _levelGain;
    private int _spectrumDynamicRange;
    private int _spectrumGain;
    public float SpectrumThreshold;


    private TMPro.TextMeshPro _idData;
    private TMPro.TextMeshPro _channelData;
    private TMPro.TextMeshPro _hzOffsetData;
    private TMPro.TextMeshPro _requiredFramesData;
    private TMPro.TextMeshPro _peaksPriorityData;
    private TMPro.TextMeshPro _heldResetData;
    private TMPro.TextMeshPro _singleTapResetData;
    private TMPro.TextMeshPro _levelDynamicRangeData;
    private TMPro.TextMeshPro _levelGainData;
    private TMPro.TextMeshPro _spectrumDynamicRangeData;
    private TMPro.TextMeshPro _spectrumGainData;
    private TMPro.TextMeshPro _spectrumThresholdData;

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
        _mainIcon = GameObject.Find("MainIcon").GetComponent<SpriteRenderer>();
        _singleTapSpawn = GameObject.Find("SingleTapSpawn").transform;
        _customTapSpawn = GameObject.Find("CustomTapSpawn").transform;
        _heldSpawn = GameObject.Find("HeldSpawn").transform;
        _timeHeldSpawn = GameObject.Find("TimeHeldSpawn").transform;

        //PlayerPrefs
        _lastSavedDeviceName = PlayerPrefHelper.GetLastSavedDeviceDefault();
        _currentChannel = PlayerPrefHelper.GetCurrentChannel();
        HzOffset = PlayerPrefHelper.GetHzOffset();
        RequiredFrames = PlayerPrefHelper.GetRequiredFrames();
        PeaksPriority = PlayerPrefHelper.GetPeaksPriority();
        HeldReset = PlayerPrefHelper.GetHeldReset();
        SingleTapReset = PlayerPrefHelper.GetSingleTapReset();
        _levelDynamicRange = PlayerPrefHelper.GetLevelDynamicRange();
        _levelGain = PlayerPrefHelper.GetLevelGain();
        _spectrumDynamicRange = PlayerPrefHelper.GetSpectrumDynamicRange();
        _spectrumGain = PlayerPrefHelper.GetSpectrumGain();
        SpectrumThreshold = PlayerPrefHelper.GetSpectrumThreshold();

        //FieldsData
        _idData = Helper.GetFieldData("Id");
        _channelData = Helper.GetFieldData("Channel");
        _hzOffsetData = Helper.GetFieldData("HzOffset");
        _requiredFramesData = Helper.GetFieldData("RequiredFrames");
        _peaksPriorityData = Helper.GetFieldData("PeaksPriority");
        _heldResetData = Helper.GetFieldData("HeldReset");
        _singleTapResetData = Helper.GetFieldData("TapReset");
        _levelDynamicRangeData = Helper.GetFieldData("LevelDynamicRange");
        _levelGainData = Helper.GetFieldData("LevelGain");
        _spectrumDynamicRangeData = Helper.GetFieldData("SpectrumDynamicRange");
        _spectrumGainData = Helper.GetFieldData("SpectrumGain");
        _spectrumThresholdData = Helper.GetFieldData("SpectrumThreshold");

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
        Helper.GetFieldButton("HeldReset").EndActionDelegate = SetHeldResetPopup;
        Helper.GetFieldButton("TapReset").EndActionDelegate = SetSingleTapResetPopup;
        Helper.GetFieldButton("LevelDynamicRange").EndActionDelegate = SetLevelDynamicRangePopup;
        Helper.GetFieldButton("LevelGain").EndActionDelegate = SetLevelGainPopup;
        Helper.GetFieldButton("SpectrumDynamicRange").EndActionDelegate = SetSpectrumDynamicRangePopup;
        Helper.GetFieldButton("SpectrumGain").EndActionDelegate = SetSpectrumGainPopup;
        Helper.GetFieldButton("SpectrumThreshold").EndActionDelegate = SetSpectrumThresholdPopup;

        GameObject.Find("LevelDynamicRange-").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetLevelDynamicRange(Helper.RoundToClosestTable(_levelDynamicRange - 5, 5)); };
        GameObject.Find("LevelDynamicRange+").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetLevelDynamicRange(Helper.RoundToClosestTable(_levelDynamicRange + 5, 5)); };
        GameObject.Find("LevelGain-").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetLevelGain(Helper.RoundToClosestTable(_levelGain - 5, 5)); };
        GameObject.Find("LevelGain+").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetLevelGain(Helper.RoundToClosestTable(_levelGain + 5, 5)); };
        GameObject.Find("SpectrumDynamicRange-").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetSpectrumDynamicRange(Helper.RoundToClosestTable(_spectrumDynamicRange - 10, 10)); };
        GameObject.Find("SpectrumDynamicRange+").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetSpectrumDynamicRange(Helper.RoundToClosestTable(_spectrumDynamicRange + 10, 10)); };
        GameObject.Find("SpectrumGain-").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetSpectrumGain(Helper.RoundToClosestTable(_spectrumGain - 10, 10)); };
        GameObject.Find("SpectrumGain+").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetSpectrumGain(Helper.RoundToClosestTable(_spectrumGain + 10, 10)); };
        GameObject.Find("SpectrumThreshold-").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetSpectrumThreshold(Helper.RoundToClosestTable((int)(SpectrumThreshold * 100) - 5, 5)); };
        GameObject.Find("SpectrumThreshold+").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetSpectrumThreshold(Helper.RoundToClosestTable((int)(SpectrumThreshold * 100) + 5, 5)); };
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

        SetHzOffset(HzOffset);
        SetRequiredFrames(RequiredFrames);
        SetPeaksPriority(PeaksPriority.GetHashCode());
        SetHeldReset(HeldReset);
        SetSingleTapReset(SingleTapReset);
        SetLevelDynamicRange(_levelDynamicRange);
        SetLevelGain(_levelGain);
        SetSpectrumDynamicRange(_spectrumDynamicRange);
        SetSpectrumGain(_spectrumGain);
        SetSpectrumThreshold(SpectrumThreshold * 100);
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
                SetChannel(_currentChannel + 1);
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
        --intResult;
        if (intResult < 0)
            intResult = 0;
        else if (intResult >= _currentDevice.ChannelCount)
            intResult = _currentDevice.ChannelCount - 1;
        _currentChannel = intResult;
        _audioLevelTracker.channel = _currentChannel;
        _spectrumAnalyzer.channel = _currentChannel;
        PlayerPrefHelper.SetCurrentChannel(intResult);
        _channelData.text = (_currentChannel + 1).ToString();
        return true;
    }

    public void ResetDefaultCalibration()
    {
        SetHzOffset(Constants.PpHzOffsetDefault);
        SetRequiredFrames(Constants.PpRequiredFramesDefault);
        SetPeaksPriority(Constants.PpPeaksPriorityDefault);
        SetHeldReset(Constants.PpHeldResetDefault);
        SetSingleTapReset(Constants.PpSingleTapResetDefault);
        SetLevelDynamicRange(Constants.PpLevelDynamicRangeDefault);
        SetLevelGain(Constants.PpLevelGainDefault);
        SetSpectrumDynamicRange(Constants.PpSpectrumDynamicRangeDefault);
        SetSpectrumGain(Constants.PpSpectrumGainDefault);
        SetSpectrumThreshold(Constants.PpSpectrumThresholdDefault * 100);
    }

    private object SetHzOffset(float offset)
    {
        if (offset < 0)
            offset = 0;
        HzOffset = offset;
        PlayerPrefHelper.SetHzOffset(offset);
        _hzOffsetData.text = offset.ToString("F2");
        return true;
    }

    private object SetRequiredFrames(float value)
    {
        if (value < 1)
            value = 1;
        var intValue = (int)value;
        RequiredFrames = intValue;
        PlayerPrefHelper.SetRequiredFrames(intValue);
        _requiredFramesData.text = intValue.ToString();
        return true;
    }

    private object SetPeaksPriority(int id)
    {
        PeaksPriority = (PeaksPriority)id;
        PlayerPrefHelper.SetPeaksPriority(PeaksPriority);
        _peaksPriorityData.text = PeaksPriority.ToString().ToLower();
        return true;
    }

    private object SetHeldReset(float value)
    {
        var intValue = (int)value;
        if (intValue > 0)
            intValue = -intValue;
        if (intValue < -100)
            intValue = -100;
        HeldReset = intValue;
        PlayerPrefHelper.SetHeldReset(intValue);
        _heldResetData.text = intValue.ToString();
        return true;
    }

    private object SetSingleTapReset(float value)
    {
        var intValue = (int)value;
        if (intValue == 0)
            intValue = -1;
        else if (intValue > 0)
            intValue = -intValue;
        if (intValue < -100)
            intValue = -100;
        SingleTapReset = intValue;
        PlayerPrefHelper.SetSingleTapReset(intValue);
        _singleTapResetData.text = intValue.ToString();
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

    private object SetSpectrumThreshold(float value)
    {
        if (value < 0.0f)
            value = 0.0f;
        else if (value > 100.0f)
            value = 100.0f;
        value = value / 100;
        SpectrumThreshold = value;
        PlayerPrefHelper.SetSpectrumThreshold(value);
        _spectrumThresholdData.text = ((int)(value * 100)).ToString();
        return true;
    }

    private void SetDeviceIdPopup()
    {
        Instantiator.NewPopupDeviceId(transform.position, _currentDevice, SetDeviceId);
    }

    private void SetChannelPopup()
    {
        var content = $"From 1 to {_currentDevice.ChannelCount}\n(current device range)";
        if (_currentDevice.ChannelCount == 1)
            content = "Your selected device has only one channel";
        Instantiator.NewPopupNumber(transform.position, "Channel", content, _currentChannel, 2, SetChannel);
    }

    private void SetHzOffsetPopup()
    {
        var content = $"0.01 = very strict\n5.00 = very loose";
        Instantiator.NewPopupNumber(transform.position, "Valid Hz Offset", content, HzOffset, 2, SetHzOffset);
    }

    private void SetRequiredFramesPopup()
    {
        var content = $"1 = minimum, no required frames";
        Instantiator.NewPopupNumber(transform.position, "required frames", content, RequiredFrames, 2, SetRequiredFrames);
    }

    private void SetPeaksPriorityPopup()
    {
        Instantiator.NewPopupEnum<PeaksPriority>(transform.position, "peaks priority", PeaksPriority.GetHashCode(), SetPeaksPriority);
    }

    private void SetHeldResetPopup()
    {
        var content = $"fixed value\nfrom 0 to -100";
        Instantiator.NewPopupNumber(transform.position, "held reset", content, HeldReset, 3, SetHeldReset);
    }

    private void SetSingleTapResetPopup()
    {
        var content = $"unfixed value\nfrom -1 to -100";
        Instantiator.NewPopupNumber(transform.position, "tap reset", content, SingleTapReset, 3, SetSingleTapReset);
    }

    private void SetLevelDynamicRangePopup()
    {
        var content = $"from 1 to 40";
        Instantiator.NewPopupNumber(transform.position, "Level Dynamic Range", content, _levelDynamicRange, 2, SetLevelDynamicRange);
    }

    private void SetLevelGainPopup()
    {
        var content = $"from -10 to 40";
        Instantiator.NewPopupNumber(transform.position, "Level Gain", content, _levelGain, 2, SetLevelGain);
    }

    private void SetSpectrumDynamicRangePopup()
    {
        var content = $"from 1 to 120";
        Instantiator.NewPopupNumber(transform.position, "Spectrum Dynamic Range", content, _spectrumDynamicRange, 3, SetSpectrumDynamicRange);
    }

    private void SetSpectrumGainPopup()
    {
        var content = $"from -10 to 120";
        Instantiator.NewPopupNumber(transform.position, "Spectrum Gain", content, _spectrumGain, 3, SetSpectrumGain);
    }

    private void SetSpectrumThresholdPopup()
    {
        var content = $"from 0 to 100";
        Instantiator.NewPopupNumber(transform.position, "Spectrum Threshold", content, SpectrumThreshold, 3, SetSpectrumThreshold);
    }

    //Icon and PoppingTexr

    private bool _isDown = false;
    private int _minFramesDown = 4;
    private int _framesDown = 0;

    public void UpdateIcon(bool down, string key = null, InputType type = InputType.Tap)
    {
        if (!enabled)
            return;
        if (_framesDown < _minFramesDown)
        {
            ++_framesDown;
            down = true;
        }

        if (down)
        {
            if (!_isDown)
                _framesDown = 0;
            _isDown = true;
            _mainIcon.sprite = IconDown;
        }
        else
        {
            _isDown = false;
            _mainIcon.sprite = IconUp;
        }

        if (key != null)
        {
            var position = _singleTapSpawn.position;
            if (type == InputType.CustomTap)
                position = _customTapSpawn.position;
            else if (type == InputType.Held)
                position = _heldSpawn.position;
            else if (type == InputType.CustomHeld)
                position = _timeHeldSpawn.position;
            if (key == "_")
            {
                position += new Vector3(0.0f, -0.25f, 0.0f);
                Instantiator.PopNoShadowText(key.ToLower(), position + new Vector3(0.0f, 1.5f, 0.0f), transform, distance: 2.0f, startFadingDistancePercent: 0.50f);
            }
            else if (key != string.Empty)
            {
                if (key.ToLower() == "noname")
                    key = "none";
                Instantiator.PopText(key.ToLower(), position + new Vector3(0.0f, 1.5f, 0.0f), this.transform, distance: 2.0f, startFadingDistancePercent: 0.50f);
            }
        }
    }
}

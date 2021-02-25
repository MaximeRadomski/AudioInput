using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowsInput.Native;

public class AudioInputBhv : FrameRateBehavior
{
    private CheckBoxBhv _enabled;
    //private TMPro.TextMeshPro _hzData;
    //private TMPro.TextMeshPro _peaksData;
    private TMPro.TextMeshPro _frequenciesData;
    private TMPro.TextMeshPro _inputData;
    private TMPro.TextMeshPro _typeData;
    private TMPro.TextMeshPro _paramData;
    private SpriteRenderer _back;

    private AudioInput _audioInput;
    private Panel01Bhv _panelBhv;
    //private bool _hasInit;
    private int _id;
    private bool _isTilting;

    public void Init(AudioInput audioInput, Panel01Bhv panelBhv, int id)
    {
        _audioInput = audioInput;
        _panelBhv = panelBhv;
        _id = id;
        _enabled = transform.Find("AudioInputEnable").GetComponent<CheckBoxBhv>();
        //_hzData = transform.Find("AudioInputHz").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        //_peaksData = transform.Find("AudioInputPeaks").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _frequenciesData = transform.Find("AudioInputFrequencies").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _inputData = transform.Find("AudioInputInput").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _typeData = transform.Find("AudioInputType").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _paramData = transform.Find("AudioInputParam").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();

        _back = transform.Find("Back").GetComponent<SpriteRenderer>();

        SetButtons();
        LoadData();
        //_hasInit = true;
    }

    private void SetButtons()
    {
        transform.Find("AudioInputEnable").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetEnabled(!_audioInput.Enabled); };
        //transform.Find("AudioInputHz").GetComponent<ButtonBhv>().EndActionDelegate = SetHzPopup;
        //transform.Find("AudioInputPeaks").GetComponent<ButtonBhv>().EndActionDelegate = SetPeaksPopup;
        transform.Find("AudioInputFrequencies").GetComponent<ButtonBhv>().EndActionDelegate = SetFrequenciesPopup;
        transform.Find("AudioInputInput").GetComponent<ButtonBhv>().EndActionDelegate = SetInputPopup;
        transform.Find("AudioInputType").GetComponent<ButtonBhv>().EndActionDelegate = SetTypePopup;
        transform.Find("AudioInputParam").GetComponent<ButtonBhv>().EndActionDelegate = SetParamPopup;
        transform.Find("AudioInputDelete").GetComponent<ButtonBhv>().EndActionDelegate = Delete;
    }

    private void LoadData()
    {
        SetEnabled(_audioInput.Enabled);
        //SetHz(_audioInput.Hz0);
        //SetPeaks(_audioInput.Peaks);
        SetFrequencies(_audioInput.Frequencies, _audioInput.Peaks);
        if (_audioInput.MouseInput == MouseInput.None)
            SetKeyboardInput(_audioInput.Key.GetHashCode());
        else
            SetMouseInput(_audioInput.MouseInput.GetHashCode());
        SetType(_audioInput.InputType.GetHashCode());
        SetParam(_audioInput.Param);
    }

    private bool UpdateAudioInput()
    {
        PlayerPrefHelper.SetAudioInput(_audioInput, _id);
        return true;
    }

    private void SetEnabled(bool enabled)
    {
        _audioInput.Enabled = enabled;
        if (enabled)
            _enabled.Check();
        else
            _enabled.Uncheck();
        UpdateAudioInput();
        _panelBhv.UpdateAllEnabled();
    }

    //private object SetHz(float value)
    //{
    //    if (value < 0.0f)
    //        value = 0.0f;
    //    _audioInput.Hz0 = value;
    //    _hzData.text = value.ToString("F2");
    //    return UpdateAudioInput();
    //}

    //private object SetPeaks(float value)
    //{
    //    var intValue = (int)value;
    //    if (intValue < 0)
    //        intValue = 0;
    //    _audioInput.Peaks = intValue;
    //    _peaksData.text = intValue.ToString();
    //    return UpdateAudioInput();
    //}

    private object SetFrequencies(List<float> frequencies, int peaksNumber)
    {
        for (int i = 0; i < 5; ++i)
        {
            _audioInput.Frequencies[i] = frequencies[i];
        }
        _audioInput.Peaks = peaksNumber;
        _frequenciesData.text = $"       {_audioInput.Peaks}      |       {_audioInput.Frequencies[0].ToString("F2")}";
        return UpdateAudioInput();
    }

    private object SetKeyboardInput(int keyCodeId)
    {
        VirtualKeyCode keyCode = (VirtualKeyCode)keyCodeId;
        _audioInput.MouseInput = MouseInput.None;
        _audioInput.Key = keyCode;
        _inputData.text = keyCode == VirtualKeyCode.NONAME ? "none" : keyCode.ToString().ToLower();
        SetParam(_audioInput.Param);
        return UpdateAudioInput();
    }

    private object SetMouseInput(int id)
    {
        _audioInput.Key = VirtualKeyCode.NONAME;
        _audioInput.MouseInput = (MouseInput)id;
        _inputData.text = _audioInput.MouseInput.GetDescription().ToLower();
        SetParam(_audioInput.Param);
        return UpdateAudioInput();
    }

    private object SetType(int id)
    {
        _audioInput.InputType = (InputType)id;
        _typeData.text = _audioInput.InputType.GetDescription().ToLower();
        SetParam(_audioInput.Param);
        return UpdateAudioInput();
    }

    private object SetParam(float value)
    {
        if (value < 0.0f)
            value = 0.0f;
        var intValue = (int)value;
        if (intValue < 1)
            intValue = 1;
        if (_audioInput.InputType == InputType.SingleTap && !Helper.IsMouseDirection(_audioInput.MouseInput))
        {
            _audioInput.Param = value;
            _paramData.text = "/";
            return false;
        }
        else if ((_audioInput.InputType == InputType.SingleTap && Helper.IsMouseDirection(_audioInput.MouseInput))
               || _audioInput.InputType == InputType.Held
               || _audioInput.InputType == InputType.CustomTap)
        {
            if (_audioInput.InputType == InputType.Held && intValue > Constants.HeldUntilReleased)
                intValue = Constants.HeldUntilReleased;
            _audioInput.Param = intValue;
            _paramData.text = intValue.ToString();
        }
        else if (_audioInput.InputType == InputType.TimeHeld)
        {
            _audioInput.Param = value;
            _paramData.text = value.ToString("F2");
        }
        return UpdateAudioInput();
    }

    //private void SetHzPopup()
    //{
    //    var content = $"pick a stable one";
    //    _panelBhv.Instantiator.NewPopupNumber(_panelBhv.transform.position, "Frequency in Hz", content, _audioInput.Hz0, 4, SetHz);
    //}

    //private void SetPeaksPopup()
    //{
    //    var content = $"pick a stable one";
    //    _panelBhv.Instantiator.NewPopupNumber(_panelBhv.transform.position, "peaks number", content, _audioInput.Peaks, 3, SetPeaks);
    //}

    private void SetFrequenciesPopup()
    {
        _panelBhv.Instantiator.NewPopupFrequencies(_panelBhv.transform.position, _audioInput.Frequencies, _audioInput.Peaks, SetFrequencies);
    }

    private void SetInputPopup()
    {
        _panelBhv.Instantiator.NewPopupYesNo(_panelBhv.transform.position, "input", string.Empty, "keyboard", "mouse", OnSetInputPopup);
    }

    private object OnSetInputPopup(bool isMouse)
    {
        if (isMouse)
            _panelBhv.Instantiator.NewPopupEnum<MouseInput>(_panelBhv.transform.position, "mouse input", _audioInput.MouseInput.GetHashCode(), SetMouseInput);
        else
            _panelBhv.Instantiator.NewPopupInput(_panelBhv.transform.position, _audioInput.Key.GetHashCode(), SetKeyboardInput);
        return true;
    }

    private void SetTypePopup()
    {
        _panelBhv.Instantiator.NewPopupEnum<InputType>(_panelBhv.transform.position, "input type", _audioInput.InputType.GetHashCode(), SetType);
    }
    private void SetParamPopup()
    {
        if (_audioInput.InputType == InputType.SingleTap && !Helper.IsMouseDirection(_audioInput.MouseInput))
            return;
        var maxDigit = 2;
        var content = $"linked to the input type";
        if (_audioInput.InputType == InputType.SingleTap && Helper.IsMouseDirection(_audioInput.MouseInput))
            content = "cursor offset\n1 = 1 pixel";
        else if (_audioInput.InputType == InputType.Held)
            content = "1 = held until level reset\n2 = held only when listened";
        else if (_audioInput.InputType == InputType.CustomTap)
            content = "the number of times the input\nwill be sent";
        else if (_audioInput.InputType == InputType.TimeHeld)
            content = "the number of seconds the input\nwill be held";
        _panelBhv.Instantiator.NewPopupNumber(_panelBhv.transform.position, "type param", content, _audioInput.Param, maxDigit, SetParam);
    }

    private void Delete()
    {
        _panelBhv.OnDelete(_id);
    }

    public void Tilt()
    {
        _back.color = Constants.ColorPlain;
        _isTilting = true;
    }

    protected override void FrameUpdate()
    {
        if (_isTilting)
            Tilting();
    }

    private void Tilting()
    {
        _back.color = Color.Lerp(_back.color, Constants.ColorPlainTransparent, 0.1f);
        if (Helper.FloatEqualsPrecision(_back.color.a, Constants.ColorPlainTransparent.a, 0.02f))
        {
            _back.color = Constants.ColorPlainTransparent;
            _isTilting = false;
        }
    }
}

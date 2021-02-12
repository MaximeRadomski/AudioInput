using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowsInput.Native;

public class AudioInputBhv : FrameRateBehavior
{
    private CheckBoxBhv _enabled;
    private TMPro.TextMeshPro _hzData;
    private TMPro.TextMeshPro _peaksData;
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
        _hzData = transform.Find("AudioInputHz").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _peaksData = transform.Find("AudioInputPeaks").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
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
        transform.Find("AudioInputHz").GetComponent<ButtonBhv>().EndActionDelegate = SetHzPopup;
        transform.Find("AudioInputPeaks").GetComponent<ButtonBhv>().EndActionDelegate = SetPeaksPopup;
        transform.Find("AudioInputInput").GetComponent<ButtonBhv>().EndActionDelegate = SetInputPopup;
        transform.Find("AudioInputType").GetComponent<ButtonBhv>().EndActionDelegate = SetTypePopup;
        transform.Find("AudioInputParam").GetComponent<ButtonBhv>().EndActionDelegate = SetParamPopup;
        transform.Find("AudioInputDelete").GetComponent<ButtonBhv>().EndActionDelegate = Delete;
    }

    private void LoadData()
    {
        SetEnabled(_audioInput.Enabled);
        SetHz(_audioInput.Hz);
        SetPeaks(_audioInput.Peaks);
        if (_audioInput.InputType != InputType.Mouse)
            SetInput(_audioInput.Key);
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

    private object SetHz(float value)
    {
        if (value < 0.0f)
            value = 0.0f;
        _audioInput.Hz = value;
        _hzData.text = value.ToString("F2");
        return UpdateAudioInput();
    }

    private object SetPeaks(float value)
    {
        var intValue = (int)value;
        if (intValue < 0)
            intValue = 0;
        _audioInput.Peaks = intValue;
        _peaksData.text = intValue.ToString();
        return UpdateAudioInput();
    }

    private object SetInput(VirtualKeyCode keyCode)
    {
        _audioInput.Key = keyCode;
        _inputData.text = keyCode == VirtualKeyCode.NONAME ? "none" : keyCode.ToString().ToLower();
        return UpdateAudioInput();
    }

    private object SetMouseInput(int id)
    {
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
        if (_audioInput.InputType != InputType.Mouse)
            SetInput(_audioInput.Key);
        else
            SetMouseInput(_audioInput.MouseInput.GetHashCode());
        return UpdateAudioInput();
    }

    private object SetParam(float value)
    {
        var intValue = (int)value;
        if ((_audioInput.InputType != InputType.Mouse && intValue < 0)
            || (_audioInput.InputType == InputType.Mouse && _audioInput.MouseInput == MouseInput.XButton && intValue < 0))
            intValue = 0;
        if (_audioInput.InputType == InputType.SingleTap
            || (_audioInput.InputType == InputType.Mouse && _audioInput.MouseInput == MouseInput.None))
        {
            _audioInput.Param = value;
            _paramData.text = "/";
            return false;
        }
        if (_audioInput.InputType == InputType.Holded
            || _audioInput.InputType == InputType.CustomTap
            || _audioInput.InputType == InputType.Mouse)
        {
            if (_audioInput.InputType == InputType.Holded && intValue > 1)
                intValue = 1;
            _audioInput.Param = intValue;
            _paramData.text = intValue.ToString();
        }
        else if (_audioInput.InputType == InputType.TimeHolded)
        {
            _audioInput.Param = value;
            _paramData.text = value.ToString("F2");
        }
        return UpdateAudioInput();
    }

    private void SetHzPopup()
    {
        var content = $"pick a stable one";
        _panelBhv.Instantiator.NewPopupNumber(_panelBhv.transform.position, "Frequency in Hz", content, _audioInput.Hz, 4, SetHz);
    }

    private void SetPeaksPopup()
    {
        var content = $"pick a stable one";
        _panelBhv.Instantiator.NewPopupNumber(_panelBhv.transform.position, "peaks number", content, _audioInput.Peaks, 3, SetPeaks);
    }

    private void SetInputPopup()
    {
        if (_audioInput.InputType != InputType.Mouse)
            _panelBhv.Instantiator.NewPopupInput(_panelBhv.transform.position, _audioInput.Key, SetInput);
        else
            _panelBhv.Instantiator.NewPopupEnum<MouseInput>(_panelBhv.transform.position, "mouse input", _audioInput.MouseInput.GetHashCode(), SetMouseInput, 1);
    }

    private void SetTypePopup()
    {
        _panelBhv.Instantiator.NewPopupEnum<InputType>(_panelBhv.transform.position, "input type", _audioInput.InputType.GetHashCode(), SetType);
    }
    private void SetParamPopup()
    {
        if (_audioInput.InputType == InputType.SingleTap
            || (_audioInput.InputType == InputType.Mouse && _audioInput.MouseInput == MouseInput.None))
            return;
        var maxDigit = 2;
        var content = $"linked to the input type";
        if (_audioInput.InputType == InputType.Holded)
            content = "0 = holded until level reset\n1 = holded only when listened";
        else if (_audioInput.InputType == InputType.CustomTap)
            content = "the number of times the input\nwill be sent";
        else if (_audioInput.InputType == InputType.TimeHolded)
            content = "the number of seconds the input\nwill be holded";
        else if (_audioInput.InputType == InputType.Mouse)
        {
            if (_audioInput.MouseInput == MouseInput.LeftButton
                || _audioInput.MouseInput == MouseInput.RightButton)
            {
                content = "under 1 = holded\n1 = 1 click\nover 1 = multiple clicks";
            }
            else if (_audioInput.MouseInput == MouseInput.LeftRight)
            {
                content = "1 = 1 pixel\nnegative = left\npositive = right";
                maxDigit = 4;
            }
            else if (_audioInput.MouseInput == MouseInput.UpDown)
            {
                content = "1 = 1 pixel\nnegative = down\npositive = up";
                maxDigit = 4;
            }
            else if (_audioInput.MouseInput == MouseInput.XButton)
                content = "id of the button for mouses with more than 2 buttons";
        }
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

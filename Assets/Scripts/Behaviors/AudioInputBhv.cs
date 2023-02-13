using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowsInput.Native;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class AudioInputBhv : FrameRateBehavior
{
    private CheckBoxBhv _enabled;
    private TMPro.TextMeshPro _frequenciesData;
    private TMPro.TextMeshPro _inputData;
    private TMPro.TextMeshPro _typeData;
    private TMPro.TextMeshPro _paramData;
    private SpriteRenderer _back;

    private AudioInput _audioInput;
    private Panel01Bhv _panelBhv;
    private int _id;
    private bool _isTilting;
    private bool _hasInit;

    public void Init(AudioInput audioInput, Panel01Bhv panelBhv, int id)
    {
        _hasInit = false;
        _audioInput = audioInput;
        _panelBhv = panelBhv;
        _id = id;
        _enabled = transform.Find("AudioInputEnable").GetComponent<CheckBoxBhv>();
        _frequenciesData = transform.Find("AudioInputFrequencies").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _inputData = transform.Find("AudioInputInput").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _typeData = transform.Find("AudioInputType").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _paramData = transform.Find("AudioInputParam").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();

        _back = transform.Find("Back").GetComponent<SpriteRenderer>();

        SetButtons();
        LoadData();
        _hasInit = true;
    }

    private void SetButtons()
    {
        transform.Find("AudioInputEnable").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetEnabled(!_audioInput.Enabled); };
        transform.Find("AudioInputFrequencies").GetComponent<ButtonBhv>().EndActionDelegate = SetFrequenciesPopup;
        transform.Find("AudioInputInput").GetComponent<ButtonBhv>().EndActionDelegate = SetInputPopup;
        transform.Find("AudioInputType").GetComponent<ButtonBhv>().EndActionDelegate = SetTypePopup;
        transform.Find("AudioInputParam").GetComponent<ButtonBhv>().EndActionDelegate = SetParamPopup;
        transform.Find("AudioInputDelete").GetComponent<ButtonBhv>().EndActionDelegate = Delete;
        transform.Find("RenameButton").GetComponent<ButtonBhv>().EndActionDelegate = Rename;
    }

    private void LoadData()
    {
        SetEnabled(_audioInput.Enabled);
        SetFrequencies(_audioInput.Frequencies, _audioInput.Peaks);
        if (_audioInput.GetMainDevice() == DeviceType.Keyboard)
            SetKeyboardInput(_audioInput.Key.GetHashCode(), isSecond: false);
        else
            SetMouseInput(_audioInput.Mouse.GetHashCode());
        if (_audioInput.GetSecondDevice() == DeviceType.Keyboard)
            SetKeyboardInput(_audioInput.Key2.GetHashCode(), isSecond: true);
        else if (_audioInput.GetSecondDevice() == DeviceType.Mouse)
            SetMouseInput(_audioInput.Mouse2.GetHashCode() + 100);
        SetType(_audioInput.InputType.GetHashCode());
        SetParam(_audioInput.Param);
    }

    private bool UpdateAudioInput()
    {
        if (!_hasInit)
            return false;
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

    private object SetFrequencies(List<float> frequencies, int peaksNumber)
    {
        for (int i = 0; i < 5; ++i)
        {
            _audioInput.Frequencies[i] = frequencies[i];
        }
        _audioInput.Peaks = peaksNumber;
        DisplayFrequenciesOfName();
        return UpdateAudioInput();
    }

    private object SetName(string name)
    {
        _audioInput.Name = name;
        DisplayFrequenciesOfName();
        return UpdateAudioInput();
    }

    private void DisplayFrequenciesOfName()
    {
        if (string.IsNullOrEmpty(_audioInput.Name))
        {
            _frequenciesData.alignment = TMPro.TextAlignmentOptions.Left;
            _frequenciesData.text = $"  {_audioInput.Peaks}     |     {_audioInput.Frequencies[0].ToString("F2")}";
        }
        else
        {
            _frequenciesData.alignment = TMPro.TextAlignmentOptions.Center;
            _frequenciesData.text = _audioInput.Name;
        }
    }

    private void SetKeyboardInput(int keyCodeId, bool isSecond)
    {
        VirtualKeyCode keyCode = (VirtualKeyCode)keyCodeId;
        if (isSecond)
        {
            _audioInput.Mouse2 = MouseInput.None;
            _audioInput.Key2 = keyCode;
            if (keyCode != VirtualKeyCode.NONAME)
                _audioInput.HasSecond = true;
        }
        else
        {
            _audioInput.Mouse = MouseInput.None;
            _audioInput.Key = keyCode;
        }
        UpdateInputDataText();
        SetParam(_audioInput.Param);
        UpdateAudioInput();
    }

    private object SetMouseInput(int id)
    {
        bool isSecond = id >= 100;
        if (isSecond)
        {
            id -= 100;
            _audioInput.Key2 = VirtualKeyCode.NONAME;
            _audioInput.Mouse2 = (MouseInput)id;
            if ((MouseInput)id != MouseInput.None)
                _audioInput.HasSecond = true;
        }
        else
        {
            _audioInput.Key = VirtualKeyCode.NONAME;
            _audioInput.Mouse = (MouseInput)id;
        }
        UpdateInputDataText();
        SetParam(_audioInput.Param);
        return UpdateAudioInput();
    }

    private void DeleteSecondInput()
    {
        _audioInput.Key2 = VirtualKeyCode.NONAME;
        _audioInput.Mouse2 = MouseInput.None;
        _audioInput.HasSecond = false;
        UpdateInputDataText();
        UpdateAudioInput();
    }

    private void UpdateInputDataText()
    {
        _inputData.text = _audioInput.KeyToString();
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
        if (_audioInput.InputType == InputType.Tap && !Helper.IsMouseDirection(_audioInput.Mouse))
        {
            _audioInput.Param = value;
            _paramData.text = "/";
            return false;
        }
        else if ((_audioInput.InputType == InputType.Tap && Helper.IsMouseDirection(_audioInput.Mouse))
               || _audioInput.InputType == InputType.Held
               || _audioInput.InputType == InputType.CustomTap)
        {
            if (_audioInput.InputType == InputType.Held && intValue > Constants.HeldOnlyListened)
                intValue = Constants.HeldOnlyListened;
            _audioInput.Param = intValue;
            _paramData.text = intValue.ToString();
        }
        else if (_audioInput.InputType == InputType.CustomHeld)
        {
            _audioInput.Param = value;
            _paramData.text = value.ToString("F2");
        }
        return UpdateAudioInput();
    }

    private void SetFrequenciesPopup()
    {
        _panelBhv.Instantiator.NewPopupFrequencies(_panelBhv.transform.position, _audioInput.Frequencies, _audioInput.Peaks, SetFrequencies);
    }

    private void SetInputPopup()
    {
        _panelBhv.Instantiator.NewPopupMultiInput(_panelBhv.transform.position, OnSetInputPopup);
    }

    private void OnSetInputPopup(bool isMouse, bool? isSecond)
    {
        if (isSecond == null)
        {
            DeleteSecondInput();
        }
        else
        {
            bool isSecondBool = isSecond ?? false;
            if (isMouse)
                _panelBhv.Instantiator.NewPopupEnum<MouseInput>(_panelBhv.transform.position, $"{(isSecondBool ? "2nd" : "main")} mouse input", _audioInput.Mouse.GetHashCode(), SetMouseInput);
            else
                _panelBhv.Instantiator.NewPopupInput(_panelBhv.transform.position, $"{(isSecondBool ? "2nd" : "main")} keyboard input", _audioInput.Key.GetHashCode(), SetKeyboardInput);
        }
    }

    private void SetTypePopup()
    {
        _panelBhv.Instantiator.NewPopupEnum<InputType>(_panelBhv.transform.position, "input type", _audioInput.InputType.GetHashCode(), SetType);
    }
    private void SetParamPopup()
    {
        if (_audioInput.InputType == InputType.Tap && !Helper.IsMouseDirection(_audioInput.Mouse))
            return;
        var maxDigit = 2;
        var content = $"linked to the input type.";
        if (_audioInput.InputType == InputType.Tap && Helper.IsMouseDirection(_audioInput.Mouse))
            content = "cursor offset.\n1 = 1 pixel.";
        else if (_audioInput.InputType == InputType.Held)
            content = $"{Constants.HeldUntilReset} = held until level reset.\n{Constants.HeldUntilNext} = held until next note.\n{Constants.HeldOnlyListened} = held only when listened.";
        else if (_audioInput.InputType == InputType.CustomTap)
            content = "the number of times the input\nwill be sent.";
        else if (_audioInput.InputType == InputType.CustomHeld)
            content = $"{Constants.HeldUntilCalled} = held until called again.\n over {Constants.HeldUntilCalled} = number of seconds\nthe input will be held.";
        _panelBhv.Instantiator.NewPopupNumber(_panelBhv.transform.position, "type param", content, _audioInput.Param, maxDigit, SetParam);
    }

    private void Rename()
    {
        _panelBhv.Instantiator.NewPopupText(_panelBhv.transform.position, "Rename", "Give it a name !", _audioInput.Name, 12, SetName);
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

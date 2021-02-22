using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel03Bhv : PanelBhv
{
    private Resolution _resolution;
    private Language _language;
    private int _maximumTickrate;
    public float CustomTapDelay;
    public int MouseSensitivity;
    private OnOffHeld _onOffShortcut;

    private TMPro.TextMeshPro _resolutionData;
    private TMPro.TextMeshPro _languageData;
    private TMPro.TextMeshPro _maximumTickrateData;
    private TMPro.TextMeshPro _customTapDelayData;
    private TMPro.TextMeshPro _mouseSensitivityData;
    private TMPro.TextMeshPro _onOffShortcutData;

    void Start()
    {
        Init();
        SetButtons();
        LoadData();
    }

    public override void Init()
    {
        base.Init();
        _resolution = PlayerPrefHelper.GetResolution();
        _language = PlayerPrefHelper.GetLanguage();
        _maximumTickrate = PlayerPrefHelper.GetMaximumTickrate();
        CustomTapDelay = PlayerPrefHelper.GetCustomTapDelay();
        MouseSensitivity = PlayerPrefHelper.GetMouseSensitivity();
        _onOffShortcut = PlayerPrefHelper.GetOnOffShortcut();

        _resolutionData = Helper.GetFieldData("Resolution");
        _languageData = Helper.GetFieldData("Language");
        _maximumTickrateData = Helper.GetFieldData("MaximumTickrate");
        _customTapDelayData = Helper.GetFieldData("CustomTapDelay");
        _mouseSensitivityData = Helper.GetFieldData("MouseSensitivity");
        _onOffShortcutData = Helper.GetFieldData("OnOffShortcut");
    }

    private void SetButtons()
    {
        Helper.GetFieldButton("Resolution").EndActionDelegate = SetResolutionPopup;
        Helper.GetFieldButton("Language").EndActionDelegate = SetLanguagePopup;
        Helper.GetFieldButton("MaximumTickrate").EndActionDelegate = SetMaximumTickratePopup;
        Helper.GetFieldButton("CustomTapDelay").EndActionDelegate = SetCustomTapDelayPopup;
        Helper.GetFieldButton("MouseSensitivity").EndActionDelegate = SetMouseSensitivityPopup;
        Helper.GetFieldButton("OnOffShortcut").EndActionDelegate = SetOnOffShortcutPopup;

        Helper.GetFieldButton("ResetCalibration").EndActionDelegate = ResetCalibrationPopup;
    }

    private void LoadData()
    {
        SetResolution(_resolution.GetHashCode());
        SetLanguage(_language.GetHashCode());
        SetMaximumTickrate(_maximumTickrate);
        SetCustomTapDelay(CustomTapDelay);
        SetMouseSensitivity(MouseSensitivity);
        SetOnOffShortcut(_onOffShortcut.GetHashCode());
    }

    private object SetResolution(int id)
    {
        _resolution = (Resolution)id;
        PlayerPrefHelper.SetResolution(_resolution);
        _resolutionData.text = _resolution.GetDescription().ToLower();
        var resStr = _resolution.ToString();
        int width = int.Parse(resStr.Substring(1, resStr.IndexOf('x') - 1));
        int height = int.Parse(resStr.Substring(resStr.IndexOf('x') + 1));
        Debug.Log($"{width} x {height}");
        Screen.SetResolution(width, height, false);
        return true;
    }

    private object SetLanguage(int id)
    {
        _language = (Language)id;
        PlayerPrefHelper.SetLanguage(_language);
        _languageData.text = _language.GetDescription().ToLower();
        return true;
    }

    private object SetMaximumTickrate(float value)
    {
        var intValue = (int)value;
        if (intValue < 0)
            return false;
        PlayerPrefHelper.SetMaximumTickrate(intValue);
        Constants.SetFrame();
        _maximumTickrateData.text = intValue.ToString();
        return true;
    }

    private object SetCustomTapDelay(float value)
    {
        if (value < 0.10f)
            value = 0.10f;
        CustomTapDelay = value;
        PlayerPrefHelper.SetCustomTapDelay(value);
        _customTapDelayData.text = value.ToString("F2");
        return true;
    }

    private object SetMouseSensitivity(float value)
    {
        var intValue = (int)value;
        if (intValue < 0)
            return false;
        MouseSensitivity = intValue;
        PlayerPrefHelper.SetMouseSensitivity(intValue);
        _mouseSensitivityData.text = intValue.ToString();
        return true;
    }

    private object SetOnOffShortcut(int keyCodeId)
    {
        OnOffHeld keyCode = (OnOffHeld)keyCodeId;
        _onOffShortcut = keyCode;
        Constants.OnOffShortcut = _onOffShortcut;
        PlayerPrefHelper.SetOnOffShortcut(keyCode);
        _onOffShortcutData.text = keyCode.ToString().ToLower();
        return true;
    }

    private object ResetCalibration(bool result)
    {
        if (!result)
            return false;
        GameObject.Find("Panel00").GetComponent<Panel00Bhv>().ResetDefaultCalibration();
        return true;
    }

    private void SetResolutionPopup()
    {
        Instantiator.NewPopupEnum<Resolution>(transform.position, "resolution", _resolution.GetHashCode(), SetResolution);
    }

    private void SetLanguagePopup()
    {
        Instantiator.NewPopupEnum<Language>(transform.position, "language", _language.GetHashCode(), SetLanguage);
    }

    private void SetMaximumTickratePopup()
    {
        Instantiator.NewPopupNumber(transform.position, "ticks per second", "won't go over your current\nmonitor framerate.", _maximumTickrate, 3, SetMaximumTickrate);
    }

    private void SetCustomTapDelayPopup()
    {
        var content = $"1.0 = 1 second.\nfrom 0.10 to infinity and beyond.";
        Instantiator.NewPopupNumber(transform.position, "Custom Tap Delay", content, CustomTapDelay, 2, SetCustomTapDelay);
    }

    private void SetMouseSensitivityPopup()
    {
        Instantiator.NewPopupNumber(transform.position, "mouse sensitivity", "1 = 1 pixel", MouseSensitivity, 3, SetMouseSensitivity);
    }

    private void SetOnOffShortcutPopup()
    {
        this.Instantiator.NewPopupEnum<OnOffHeld>(transform.position, "on off shortcut", _onOffShortcut.GetHashCode(), SetOnOffShortcut);
    }

    private void ResetCalibrationPopup()
    {
        Instantiator.NewPopupYesNo(transform.position, "be careful", "are you willing to reset to the default calibration values?", "nope", "yep", ResetCalibration);
    }
}

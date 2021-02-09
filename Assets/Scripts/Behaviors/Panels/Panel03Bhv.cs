using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel03Bhv : PanelBhv
{
    private Resolution _resolution;
    private Language _language;
    private int _maximumTickrate;

    private TMPro.TextMeshPro _resolutionData;
    private TMPro.TextMeshPro _languageData;
    private TMPro.TextMeshPro _maximumTickrateData;
    private TMPro.TextMeshPro _toolVersion;

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

        _resolutionData = Helper.GetFieldData("Resolution");
        _languageData = Helper.GetFieldData("Language");
        _maximumTickrateData = Helper.GetFieldData("MaximumTickrate");

        _toolVersion = transform.Find("ToolVersion").GetComponent<TMPro.TextMeshPro>();
    }

    private void SetButtons()
    {
        Helper.GetFieldButton("Resolution").EndActionDelegate = SetResolutionPopup;
        Helper.GetFieldButton("Language").EndActionDelegate = SetLanguagePopup;
        Helper.GetFieldButton("MaximumTickrate").EndActionDelegate = SetMaximumTickratePopup;

        Helper.GetFieldButton("ResetCalibration").EndActionDelegate = ResetCalibrationPopup;

        transform.Find("KeijiroTakahashi").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://github.com/keijiro/Lasp"); };
        transform.Find("Devs").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://forum.unity.com/threads/detecting-musical-notes-from-vocal-input.316698/"); };
        transform.Find("MichaelNoonan").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://github.com/michaelnoonan/inputsimulator"); };
        transform.Find("GrafxKid").GetComponent<ButtonBhv>().EndActionDelegate = () => { Application.OpenURL("https://lospec.com/palette-list/oil-6"); };
    }

    private void LoadData()
    {
        SetResolution(_resolution.GetHashCode());
        SetLanguage(_language.GetHashCode());
        SetMaximumTickrate(_maximumTickrate);
        _toolVersion.text = $"version {Application.version}";
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
        Instantiator.NewPopupNumber(transform.position, "maximum framerate", "won't go over your current\nmonitor framerate", _maximumTickrate, 3, SetMaximumTickrate);
    }

    private void ResetCalibrationPopup()
    {
        Instantiator.NewPopupYesNo(transform.position, "be careful", "are you willing to reset the default calibration values?", "nope", "yep", ResetCalibration);
    }
}

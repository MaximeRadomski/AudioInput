using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefHelper : MonoBehaviour
{
    //Panel00
    public static void SetLastSavedDeviceDefault(string name) { PlayerPrefs.SetString(Constants.PpLastSavedDevice, name); }
    public static string GetLastSavedDeviceDefault() { return PlayerPrefs.GetString(Constants.PpLastSavedDevice, Constants.PpLastSavedDeviceDefault); }

    public static void SetCurrentChannel(int channel) { PlayerPrefs.SetInt(Constants.PpCurrentChannel, channel); }
    public static int GetCurrentChannel() { return PlayerPrefs.GetInt(Constants.PpCurrentChannel, Constants.PpCurrentChannelDefault); }

    public static void SetHzOffset(float hzOffset) { PlayerPrefs.SetFloat(Constants.PpHzOffset, hzOffset); }
    public static float GetHzOffset() { return PlayerPrefs.GetFloat(Constants.PpHzOffset, Constants.PpHzOffsetDefault); }

    public static void SetRequiredFrames(int requiredFrames) { PlayerPrefs.SetInt(Constants.PpRequiredFrames, requiredFrames); }
    public static int GetRequiredFrames() { return PlayerPrefs.GetInt(Constants.PpRequiredFrames, Constants.PpRequiredFramesDefault); }

    public static void SetPeaksPriority(PeaksPriority peaksPriority) { PlayerPrefs.SetInt(Constants.PpPeaksPriority, peaksPriority.GetHashCode()); }
    public static PeaksPriority GetPeaksPriority() { return (PeaksPriority)PlayerPrefs.GetInt(Constants.PpPeaksPriority, Constants.PpPeaksPriorityDefault); }

    public static void SetHeldReset(int heldReset) { PlayerPrefs.SetInt(Constants.PpHeldReset, heldReset); }
    public static int GetHeldReset() { return PlayerPrefs.GetInt(Constants.PpHeldReset, Constants.PpHeldResetDefault); }

    public static void SetSingleTapReset(int singleTapReset) { PlayerPrefs.SetInt(Constants.PpSingleTapReset, singleTapReset); }
    public static int GetSingleTapReset() { return PlayerPrefs.GetInt(Constants.PpSingleTapReset, Constants.PpSingleTapResetDefault); }

    public static void SetCustomTapDelay(float customTapDelay) { PlayerPrefs.SetFloat(Constants.PpCustomTapDelay, customTapDelay); }
    public static float GetCustomTapDelay() { return PlayerPrefs.GetFloat(Constants.PpCustomTapDelay, Constants.PpCustomTapDelayDefault); }

    public static void SetLevelDynamicRange(int levelDynamicRange) { PlayerPrefs.SetInt(Constants.PpLevelDynamicRange, levelDynamicRange); }
    public static int GetLevelDynamicRange() { return PlayerPrefs.GetInt(Constants.PpLevelDynamicRange, Constants.PpLevelDynamicRangeDefault); }

    public static void SetLevelGain(int levelGain) { PlayerPrefs.SetInt(Constants.PpLevelGain, levelGain); }
    public static int GetLevelGain() { return PlayerPrefs.GetInt(Constants.PpLevelGain, Constants.PpLevelGainDefault); }

    public static void SetSpectrumDynamicRange(int spectrumDynamicRange) { PlayerPrefs.SetInt(Constants.PpSpectrumDynamicRange, spectrumDynamicRange); }
    public static int GetSpectrumDynamicRange() { return PlayerPrefs.GetInt(Constants.PpSpectrumDynamicRange, Constants.PpSpectrumDynamicRangeDefault); }

    public static void SetSpectrumGain(int spectrumGain) { PlayerPrefs.SetInt(Constants.PpSpectrumGain, spectrumGain); }
    public static int GetSpectrumGain() { return PlayerPrefs.GetInt(Constants.PpSpectrumGain, Constants.PpSpectrumGainDefault); }

    public static void SetSpectrumThreshold(float spectrumThreshold) { PlayerPrefs.SetFloat(Constants.PpSpectrumThreshold, spectrumThreshold); }
    public static float GetSpectrumThreshold() { return PlayerPrefs.GetFloat(Constants.PpSpectrumThreshold, Constants.PpSpectrumThresholdDefault); }

    //Panel01
    public static void SetAudioInputs(List<AudioInput> audioInputs)
    {
        for (int i = 0; i < Constants.MaxAudioInputs; ++i)
        {
            if (i >= audioInputs.Count)
            {
                PlayerPrefs.SetString($"{Constants.PpAudioInputs}[{i}]", Constants.PpAudioInputsDefault);
                continue;
            }
            var json = audioInputs[i].ToAudioInputJson();
            PlayerPrefs.SetString($"{Constants.PpAudioInputs}[{i}]", JsonUtility.ToJson(json));
        }
    }

    public static void SetAudioInput(AudioInput audioInput, int id)
    {
        var json = audioInput.ToAudioInputJson();
        PlayerPrefs.SetString($"{Constants.PpAudioInputs}[{id}]", JsonUtility.ToJson(json));
    }

    public static List<AudioInput> GetAudioInputs()
    {
        var audioInputs = new List<AudioInput>();
        for (int i = 0; i < Constants.MaxAudioInputs; ++i)
        {
            var key = $"{Constants.PpAudioInputs}[{i}]";
            if (!PlayerPrefs.HasKey(key))
                continue;
            var json = JsonUtility.FromJson<AudioInputJson>(PlayerPrefs.GetString(key, Constants.PpAudioInputsDefault));
            if (json != null)
                audioInputs.Add(json.ToAudioInput());
        }        
        return audioInputs;
    }

    //Panel03

    public static void SetResolution(Resolution resolution) { PlayerPrefs.SetInt(Constants.PpResolution, resolution.GetHashCode()); }
    public static Resolution GetResolution() { return (Resolution)PlayerPrefs.GetInt(Constants.PpResolution, Constants.PpResolutionDefault); }

    public static void SetLanguage(Language language) { PlayerPrefs.SetInt(Constants.PpLanguage, language.GetHashCode()); }
    public static Language GetLanguage() { return (Language)PlayerPrefs.GetInt(Constants.PpLanguage, Constants.PpLanguageDefault); }

    public static void SetMaximumTickrate(int maximumTickrate) { PlayerPrefs.SetInt(Constants.PpMaximumTickrate, maximumTickrate); }
    public static int GetMaximumTickrate() { return PlayerPrefs.GetInt(Constants.PpMaximumTickrate, Constants.PpMaximumTickrateDefault); }

    public static void SetMouseSensitivity(int mouseSensitivity) { PlayerPrefs.SetInt(Constants.PpMouseSensitivity, mouseSensitivity); }
    public static int GetMouseSensitivity() { return PlayerPrefs.GetInt(Constants.PpMouseSensitivity, Constants.PpMouseSensitivityDefault); }

    public static void SetOnOffShortcut(OnOffHeld onOffShortcut) { PlayerPrefs.SetInt(Constants.PpOnOffShortcut, onOffShortcut.GetHashCode()); }
    public static OnOffHeld GetOnOffShortcut() { return (OnOffHeld)PlayerPrefs.GetInt(Constants.PpOnOffShortcut, Constants.PpOnOffShortcutDefault); }

    public static void SetHoverHelpStatus(OnOffStatus hoverHelpStatus) { PlayerPrefs.SetInt(Constants.PpHoverHelpStatus, hoverHelpStatus.GetHashCode()); }
    public static OnOffStatus GetHoverHelpStatus() { return (OnOffStatus)PlayerPrefs.GetInt(Constants.PpHoverHelpStatus, Constants.PpHoverHelpStatusDefault); }

    public static void SetFont(string fontName) { PlayerPrefs.SetString(Constants.PpFont, fontName); }
    public static string GetFont() { return PlayerPrefs.GetString(Constants.PpFont, Constants.PpFontDefault); }
}

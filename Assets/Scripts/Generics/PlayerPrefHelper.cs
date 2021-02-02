using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefHelper : MonoBehaviour
{
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

    public static void SetLevelDynamicRange(int levelDynamicRange) { PlayerPrefs.SetInt(Constants.PpLevelDynamicRange, levelDynamicRange); }
    public static int GetLevelDynamicRange() { return PlayerPrefs.GetInt(Constants.PpLevelDynamicRange, Constants.PpLevelDynamicRangeDefault); }

    public static void SetLevelGain(int levelGain) { PlayerPrefs.SetInt(Constants.PpLevelGain, levelGain); }
    public static int GetLevelGain() { return PlayerPrefs.GetInt(Constants.PpLevelGain, Constants.PpLevelGainDefault); }

    public static void SetSpectrumDynamicRange(int spectrumDynamicRange) { PlayerPrefs.SetInt(Constants.PpSpectrumDynamicRange, spectrumDynamicRange); }
    public static int GetSpectrumDynamicRange() { return PlayerPrefs.GetInt(Constants.PpSpectrumDynamicRange, Constants.PpSpectrumDynamicRangeDefault); }

    public static void SetSpectrumGain(int spectrumGain) { PlayerPrefs.SetInt(Constants.PpSpectrumGain, spectrumGain); }
    public static int GetSpectrumGain() { return PlayerPrefs.GetInt(Constants.PpSpectrumGain, Constants.PpSpectrumGainDefault); }
}

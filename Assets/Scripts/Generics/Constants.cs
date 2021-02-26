using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const float Pixel = 0.14285f;
    public const int MaxAudioInputs = 256;
    public const float SingleTapDelay = 0.05f;
    public const int HeldUntilReset = 1;
    public const int HeldUntilNext = 2;
    public const int HeldOnlyListened = 3;
    public const string ExportsFolderName = "Exports";
    public const string AudioInputsJson = "AudioInputsJson";
    public static OnOffHeld OnOffShortcut;
    public static OnOffStatus HoverHelpStatus;

    private static float? _frame;
    public static float? Frame {
        get {
            if (_frame == null)
                SetFrame();
            return _frame;
        }}
    public static float? LastFrame;
    public static void SetFrame()
    {
        _frame = 1.0f / (PlayerPrefHelper.GetMaximumTickrate() + 1);
        LastFrame = Time.time;
    }

    public static bool HasInit = false;
    public static bool InputLocked = false;

    public const string AbjectAudioInputs = "AbjectAudioInputs";

    public const string TagAudioInputs = "AudioInputs";
    public const string TagPoppingText = "PoppingText";

    //  PLAYER PREFS  //
    // Panel00
    public const string PpLastSavedDevice = "LastSavedDevice";
    public const string PpLastSavedDeviceDefault = null;
    public const string PpCurrentChannel = "CurrentChannel";
    public const int PpCurrentChannelDefault = 0;
    public const string PpHzOffset = "HzOffset";
    public const float PpHzOffsetDefault = 0.5f;
    public const string PpRequiredFrames = "RequiredFrames";
    public const int PpRequiredFramesDefault = 2;
    public const string PpPeaksPriority = "PeaksPriority";
    public const int PpPeaksPriorityDefault = 0;
    public const string PpHeldReset = "HeldReset";
    public const int PpHeldResetDefault = -15;
    public const string PpSingleTapReset = "SingleTapReset";
    public const int PpSingleTapResetDefault = -30;
    public const string PpLevelDynamicRange = "LevelDynamicRange";
    public const int PpLevelDynamicRangeDefault = 30;
    public const string PpLevelGain = "LevelGain";
    public const int PpLevelGainDefault = 0;
    public const string PpSpectrumDynamicRange = "SpectrumDynamicRange";
    public const int PpSpectrumDynamicRangeDefault = 30;
    public const string PpSpectrumGain = "SpectrumGain";
    public const int PpSpectrumGainDefault = 0;
    public const string PpSpectrumThreshold = "SpectrumThreshold";
    public const float PpSpectrumThresholdDefault = 0.05f;
    //Panel01
    public const string PpAudioInputs = "AudioInputs";
    public const string PpAudioInputsDefault = null;
    //Panel03
    public const string PpResolution = "Resolution";
    public const int PpResolutionDefault = (int)Resolution.R1280x720;
    public const string PpLanguage = "Language";
    public const int PpLanguageDefault = 0;
    public const string PpMaximumTickrate = "MaximumTickrate";
    public const int PpMaximumTickrateDefault = 60;
    public const string PpCustomTapDelay = "CustomTapDelay";
    public const float PpCustomTapDelayDefault = 0.5f;
    public const string PpMouseSensitivity = "MouseSensitivity";
    public const int PpMouseSensitivityDefault = 10;
    public const string PpHoverHelpStatus = "HoverHelpStatus";
    public const int PpHoverHelpStatusDefault = (int)OnOffStatus.On;
    public const string PpOnOffShortcut = "OnOffShortcut";
    public const int PpOnOffShortcutDefault = (int)OnOffHeld.F5;


    public static Color ColorBlackTransparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    public static Color ColorPlain = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public static Color ColorPlainTransparent = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    public static Color ColorPlainSemiTransparent = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    public static Color ColorPlainQuarterTransparent = new Color(1.0f, 1.0f, 1.0f, 0.25f);
    public static Color ColorBlack = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    public static int InputLayer = 0;
    public static bool DoubleClick = false;
    public static string ClickHistory = null;
    public static string LastEndActionClickedName = null;
    public static List<string> InputTopLayerNames = null;
    public static bool EscapeOrEnterLocked = false;
    public static bool IsOn = false;

    public static void SetLastEndActionClickedName(string name)
    {
        if (!DoubleClick) //Prevents triple click
            ClickHistory = LastEndActionClickedName;
        LastEndActionClickedName = name;

        if (ClickHistory == LastEndActionClickedName)
        {
            DoubleClick = true;
            ClickHistory = string.Empty; //Prevents triple click
        }
        else
            DoubleClick = false;
    }

    public static void IncreaseInputLayer(string name)
    {
        ++InputLayer;
        if (InputTopLayerNames == null)
            InputTopLayerNames = new List<string>();
        InputTopLayerNames.Add(name);
    }

    public static void DecreaseInputLayer()
    {
        --InputLayer;
        if (InputTopLayerNames == null)
            return;
        if (InputTopLayerNames.Count <= 0)
        {
            InputLayer = 0;
            return;
        }
        InputTopLayerNames.RemoveAt(InputTopLayerNames.Count - 1);
    }
}

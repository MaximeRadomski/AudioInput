using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDrawerBhv : MonoBehaviour
{
    private Transform _levelLower;
    private Transform _levelInside;
    private Transform _dynamicRange;
    private Transform _levelHoldedReset;
    private Transform _levelSingleTapReset;
    private Transform _levelOver;

    private bool _hasInit;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _levelLower = transform.Find("LevelLower");
        _levelInside = transform.Find("LevelInside");
        _dynamicRange = transform.Find("DynamicRange");
        _levelHoldedReset = transform.Find("LevelHoldedReset");
        _levelSingleTapReset = transform.Find("LevelSingleTapReset");
        _levelOver = transform.Find("LevelOver");
        _hasInit = true;
    }

    public bool Draw(AudioLevelTracker tracker, float holdedReset, float singleTapReset)
    {
        if (!_hasInit)
            Init();

        const float kMeterRange = 60;
        var amp = 1 + tracker.inputLevel / kMeterRange;
        var peak = 1 - tracker.currentGain / kMeterRange;
        var dr = tracker.dynamicRange / kMeterRange;

        var drStart = peak - dr;
        DrawRect(drStart, peak, _dynamicRange);

        var y1 = Mathf.Min(amp, drStart);
        var y2 = Mathf.Min(peak, amp);
        DrawRect(0, y1, _levelLower);
        DrawRect(y1, y2, _levelInside);
        DrawRect(y2, amp, _levelOver);

        //var y3 = peak + dr * (tracker.normalizedLevel - 1);
        var holded = (holdedReset / 100) * dr;
        var singleTap = (singleTapReset / 100) * dr;
        DrawRect(drStart + holded, drStart + holded, _levelHoldedReset, constantsScale:true);
        DrawRect(drStart + singleTap, drStart + singleTap, _levelSingleTapReset, constantsScale: true);

        if (y2 > y1)
            return y2 < drStart + holded;
        else
            return y1 < drStart + holded;
    }

    private void DrawRect(float y1, float y2, Transform shape, bool constantsScale = false)
    {
        y1 = 100 * Mathf.Clamp01(y1);
        y2 = 100 * Mathf.Clamp01(y2);

        shape.localPosition = new Vector3(shape.localPosition.x, (((y2 - y1) * Constants.Pixel) / 2) + (y1 * Constants.Pixel), 0.0f);
        if (!constantsScale)
            shape.localScale = new Vector3(shape.localScale.x, (y2 - y1), 1.0f);
    }
}

using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDrawerBhv : MonoBehaviour
{
    private Transform _levelLower;
    private Transform _levelInside;
    private Transform _dynamicRange;
    private Transform _levelInsidePoint;
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
        _levelInsidePoint = transform.Find("LevelInsidePoint");
        _levelOver = transform.Find("LevelOver");
        _hasInit = true;
    }

    public void Draw(AudioLevelTracker tracker)
    {
        if (!_hasInit)
            Init();

        const float kMeterRange = 60;
        var amp = 1 + tracker.inputLevel / kMeterRange;
        var peak = 1 - tracker.currentGain / kMeterRange;
        var dr = tracker.dynamicRange / kMeterRange;

        DrawRect(peak - dr, peak, _dynamicRange);

        var y1 = Mathf.Min(amp, peak - dr);
        var y2 = Mathf.Min(peak, amp);
        DrawRect(0, y1, _levelLower);
        DrawRect(y1, y2, _levelInside);
        DrawRect(y2, amp, _levelOver);

        var y3 = peak + dr * (tracker.normalizedLevel - 1);
        DrawRect(y3 - 3 / 100, y3, _levelInsidePoint, constantsScale:true);
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
